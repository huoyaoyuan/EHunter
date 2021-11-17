using Microsoft.EntityFrameworkCore;

namespace EHunter.Data.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var factory = new EHunterDbContextFactory();
            using var dbContext = factory.CreateDbContext(args);
            var query = dbContext.Set<GalleryTag>().Select(x => new { x.TagScopeName, x.TagName })
                .Concat(dbContext.Set<ImageTag>().Select(x => new { x.TagScopeName, x.TagName }))
                .Distinct();
            Console.WriteLine(query.ToQueryString());
        }
    }
}
