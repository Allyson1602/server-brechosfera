using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Brechosfera.Infrastructure.Data;
using Brechosfera.Domain.Abstractions;
using Brechosfera.Infrastructure.Repositories;

namespace Brechosfera.Infrastructure;

[ExcludeFromCodeCoverage]
public static class PersistenceExtensions
{
    private const int DbMaxRetryCount = 3;
    private const int DbCommandTimeout = 35;
    private const string DbMigrationAssemblyName = "Brechosfera.Infrastructure";

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql
            (
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mySqlOptions =>
                {
                    mySqlOptions.MigrationsAssembly(DbMigrationAssemblyName);
                    mySqlOptions.CommandTimeout(DbCommandTimeout);
                    mySqlOptions.EnableRetryOnFailure(DbMaxRetryCount);
                }
            );
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Scan(scan => scan
            .FromAssemblyOf<AppDbContext>()
            .AddClasses(classes => classes.AssignableTo(typeof(Repository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }
}
