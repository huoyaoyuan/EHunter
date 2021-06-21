using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace EHunter.Services
{
    public class JsonSettingStore : ISettingsStore
    {
        private readonly FileInfo _fileInfo;
        private readonly object _writeLock = new();

        private readonly JsonObject _jObject;
        private readonly JsonSettingStore? _rootStore;

        public JsonSettingStore(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;

            try
            {
                using (var file = fileInfo.OpenRead())
                    _jObject = JsonSerializer.DeserializeAsync<JsonObject>(file).AsTask().Result ?? new();
            }
            catch
            {
                _jObject = new();
            }
        }

        private JsonSettingStore(FileInfo fileInfo, JsonSettingStore rootStore, JsonObject jObject)
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
                using (var file = _fileInfo.OpenWrite())
                    JsonSerializer.SerializeAsync(file, _jObject).Wait();
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
                if (_jObject[name] is not JsonObject @object)
                {
                    _jObject[name] = @object = new();
                    Save();
                }

                return new JsonSettingStore(_fileInfo, _rootStore ?? this, @object);
            }
        }

        public int? GetIntValue(string name)
            => _jObject[name] is JsonValue value && value.TryGetValue(out int v)
            ? v : null;
        public void SetIntValue(string name, int value)
        {
            lock (_writeLock)
            {
                _jObject[name] = value;
                Save();
            }
        }

        public string? GetStringValue(string name)
            => _jObject[name] is JsonValue value && value.TryGetValue(out string? str)
            ? str : null;
        public void SetStringValue(string name, string? value)
        {
            lock (_writeLock)
            {
                _jObject[name] = value;
                Save();
            }
        }

        public bool? GetBoolValue(string name)
            => _jObject[name] is JsonValue value && value.TryGetValue(out bool b)
            ? b : null;
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
