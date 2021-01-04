namespace EHunter.Pixiv.Models
{
    public interface IPixivSettingStore
    {
        int MaxDownloadsInParallel { get; set; }
        string? RefreshToken { get; set; }
        bool UseProxy { get; set; }
    }
}