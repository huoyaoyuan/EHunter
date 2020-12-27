namespace EHunter.Data.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var factory = new EHunterDbContextFactory();
            using var dbContext = factory.CreateDbContext(args);
        }
    }
}
