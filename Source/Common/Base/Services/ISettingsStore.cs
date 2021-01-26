namespace EHunter.Services
{
    public interface ISettingsStore
    {
        void ClearValue(string name);
        ISettingsStore GetSubContainer(string name);

        int? GetIntValue(string name);
        void SetIntValue(string name, int value);

        string? GetStringValue(string name);
        void SetStringValue(string name, string value);

        bool? GetBoolValue(string name);
        void SetBoolValue(string name, bool value);
    }
}
