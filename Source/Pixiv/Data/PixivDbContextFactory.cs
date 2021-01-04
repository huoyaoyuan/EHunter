using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

[assembly: InternalsVisibleTo("EHunter.Data.Test")]

namespace EHunter.Data.Pixiv
{
    internal class PixivDbContextFactory : IDesignTimeDbContextFactory<PixivDbContext>
    {
        public PixivDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PixivDbContext>();
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true");
            return new PixivDbContext(optionsBuilder.Options);
        }
    }
}
