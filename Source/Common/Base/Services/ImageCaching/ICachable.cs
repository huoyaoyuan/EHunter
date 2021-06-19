namespace EHunter.Services.ImageCaching
{
    public interface ICachable
    {
        object GetCacheKey();
    }
}
