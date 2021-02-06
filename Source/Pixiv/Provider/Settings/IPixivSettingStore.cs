namespace EHunter.Pixiv.Settings
{
    public interface IPixivSettingStore
    {
        int MaxDownloadsInParallel { get; set; }
        string? RefreshToken { get; set; }
        PixivConnectionMode ConnectionMode { get; set; }
    }
}
