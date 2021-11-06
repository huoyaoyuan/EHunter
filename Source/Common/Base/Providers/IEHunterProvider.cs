namespace EHunter.Providers
{
    public interface IEHunterProvider
    {
        string Title { get; }
        Uri IconUri { get; }
        object CreateUIRoot();
    }
}
