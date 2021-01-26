using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EHunter.Services
{
    public class JsonSettingStore : ISettingsStore
    {
        private readonly FileInfo _fileInfo;
        private readonly object _writeLock = new();

        private readonly JObject _jObject;
        private readonly JsonSettingStore? _rootStore;

        public JsonSettingStore(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;

            try
            {
                using (var file = fileInfo.OpenText())
                using (var jsonReader = new JsonTextReader(file))
                    _jObject = JObject.Load(jsonReader);
            }
            catch
            {
                _jObject = new();
            }
        }

        private JsonSettingStore(FileInfo fileInfo, JsonSettingStore rootStore, JObject jObject)
        {
            _fileInfo = fileInfo;
            _rootStore = rootStore;
            _jObject = jObject;
        }

        private void Save()
        {
            if (_rootStore != null)
            {
                _rootStore.Save();
                return;
            }

            try
            {
                using (var file = _fileInfo.CreateText())
                using (var jsonWriter = new JsonTextWriter(file))
                    _jObject.WriteTo(jsonWriter);
            }
            catch
            {
                // TODO: log
            }
        }

        public void ClearValue(string name)
        {
            lock (_writeLock)
            {
                _jObject[name] = null;
                Save();
            }
        }

        public ISettingsStore GetSubContainer(string name)
        {
            lock (_writeLock)
            {
                if (_jObject[name] is not JObject @object)
                {
                    _jObject[name] = @object = new();
                    Save();
                }

                return new JsonSettingStore(_fileInfo, _rootStore ?? this, @object);
            }
        }

        public int? GetIntValue(string name)
            => _jObject[name] is JValue { Type: JTokenType.Null or JTokenType.Integer or JTokenType.Float } value
            ? (int?)value : null;
        public void SetIntValue(string name, int value)
        {
            lock (_writeLock)
            {
                _jObject[name] = value;
                Save();
            }
        }

        public string? GetStringValue(string name)
            => _jObject[name] is JValue { Type: JTokenType.Null or JTokenType.String } value
            ? (string?)value : null;
        public void SetStringValue(string name, string? value)
        {
            lock (_writeLock)
            {
                _jObject[name] = value;
                Save();
            }
        }

        public bool? GetBoolValue(string name)
            => _jObject[name] is JValue { Type: JTokenType.Null or JTokenType.Boolean } value
            ? (bool?)value : null;
        public void SetBoolValue(string name, bool value)
        {
            lock (_writeLock)
            {
                _jObject[name] = value;
                Save();
            }
        }
    }
}
