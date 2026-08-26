using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Brechosfera.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserCredential> UserCredentials { get; set; }
    public DbSet<Store> Store { get; set; }
    public DbSet<Address> StoreAddress { get; set; }
    public DbSet<ProductType> ProductType { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}