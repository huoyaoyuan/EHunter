using EHunter.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace EHunter.Data
{
    public interface IDbContextFactoryResolver<TContext>
        : ICustomResolver<IDbContextFactory<TContext>?>
        where TContext : DbContext
    {
    }
}
