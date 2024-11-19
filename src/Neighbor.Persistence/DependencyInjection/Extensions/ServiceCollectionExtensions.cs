using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Neighbor.Domain.Abstraction.EntitiyFramework;
using Neighbor.Domain.Abstraction.EntitiyFramework.Repositories;
using Neighbor.Persistence.DependencyInjection.Options;
using Neighbor.Persistence.Repositories;

namespace Neighbor.Persistence.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSqlConfiguration(this IServiceCollection services)
    {
        services.AddDbContextPool<DbContext, ApplicationDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var options = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();

            builder
            .EnableDetailedErrors(true)
            .EnableSensitiveDataLogging(true)
            .UseLazyLoadingProxies(true) // => If UseLazyLoadingProxies, all of the navigation fields should be VIRTUAL
            .UseSqlServer(
                connectionString: configuration.GetConnectionString("ConnectionStrings"),
                    sqlServerOptionsAction: optionsBuilder
                        => optionsBuilder
                        .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));
        });
    }

    public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOptions
       (this IServiceCollection services, IConfigurationSection section)
       => services
           .AddOptions<SqlServerRetryOptions>()
           .Bind(section)
           .ValidateDataAnnotations()
           .ValidateOnStart();

    public static void AddRepositoryBaseConfiguration(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IEFUnitOfWork), typeof(EFUnitOfWork))
            .AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddTransient<IAccountRepository, AccountRepository>()
            .AddTransient<ISurchargeRepository, SurchargeRepository>()
            .AddTransient<IProductRepository, ProductRepository>();
    }
}
