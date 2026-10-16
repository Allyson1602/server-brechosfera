using Brechosfera.Domain.Abstractions;
using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;
using Brechosfera.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Brechosfera.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Store> Store { get; set; }
    public DbSet<Address> StoreAddress { get; set; }
    public DbSet<ProductType> ProductType { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        ApplySoftDeleteFilters(modelBuilder);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (
                !typeof(Entity).IsAssignableFrom(entityType.ClrType) ||
                entityType.IsOwned()
            )
            {
                continue;
            }

            var parameter = Expression.Parameter(
                entityType.ClrType,
                "entity"
            );

            var deletedAt = Expression.Property(
                parameter,
                nameof(Entity.DeletedAt)
            );

            var filter = Expression.Lambda(
                Expression.Equal(
                    deletedAt,
                    Expression.Constant(null, typeof(DateTimeOffset?))
                ),
                parameter
            );

            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(filter);
        }
    }
}