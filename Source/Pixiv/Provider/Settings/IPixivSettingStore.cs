namespace EHunter.Pixiv.Settings
{
    public interface IPixivSettingStore
    {
        int MaxDownloadsInParallel { get; set; }
        string? RefreshToken { get; set; }
        bool UseProxy { get; set; }
        PixivConnectionMode ConnectionMode { get; set; }
    }
}
