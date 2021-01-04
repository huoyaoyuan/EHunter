namespace EHunter.Settings
{
    public interface ICommonSettingStore
    {
        string StorageRoot { get; set; }
        string ProxyAddress { get; set; }
        int ProxyPort { get; set; }
        string DbConnectionString { get; set; }
    }
}
