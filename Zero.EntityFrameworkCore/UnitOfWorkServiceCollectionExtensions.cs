using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zero.Domain.Uow;
using Zero.EntityFrameworkCore.Uow;

namespace Zero.EntityFrameworkCore
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<TDbContext>>();
            return services;
        }
    }
}
