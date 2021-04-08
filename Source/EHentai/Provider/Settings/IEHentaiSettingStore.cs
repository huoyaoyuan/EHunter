namespace EHunter.EHentai.Settings
{
    public interface IEHentaiSettingStore
    {
        string? MemberId { get; set; }
        string? PassHash { get; set; }
        EHentaiConnectionMode ConnectionMode { get; set; }
    }
}
