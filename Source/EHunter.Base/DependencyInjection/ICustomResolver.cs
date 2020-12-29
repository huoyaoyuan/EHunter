namespace EHunter.DependencyInjection
{
    public interface ICustomResolver<out T> where T : class?
    {
        T Resolve();
    }
}
