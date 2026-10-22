using Brechosfera.Domain.Entities;
using Brechosfera.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brechosfera.Infrastructure.Mappings;

public class StoreMapping : EntityMapping<Store>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        base.Configure(builder);

        builder.ToTable("Stores");

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        // Nome único apenas entre lojas ativas: lojas excluídas têm ActiveName = NULL,
        // e o índice único do MariaDB aceita vários NULLs.
        builder.Property<string?>("ActiveName")
            .HasMaxLength(150)
            .HasComputedColumnSql("CASE WHEN `DeletedAt` IS NULL THEN `Name` END", stored: true);

        builder.HasIndex("ActiveName").IsUnique();

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.AveragePrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.LocalType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.AverageQuantity);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.ProductTypes)
            .WithMany()
            .UsingEntity(
                "StoreProductType",
                x => x.HasOne(typeof(ProductType)).WithMany().HasForeignKey("ProductTypesId").HasPrincipalKey(nameof(ProductType.Id)),
                x => x.HasOne(typeof(Store)).WithMany().HasForeignKey("StoresId").HasPrincipalKey(nameof(Store.Id)).OnDelete(DeleteBehavior.Cascade),
                x => x.HasKey("ProductTypesId", "StoresId")
            );

        builder.OwnsOne(x => x.Address, address =>
        {
            address.ToTable("StoreAddresses");

            address.WithOwner()
                .HasForeignKey("StoreId");

            AddressMapping.ConfigureProperties(address);
        });

        builder.OwnsMany(x => x.Phones, phone =>
        {
            phone.ToTable("StorePhones");

            phone.WithOwner()
                .HasForeignKey("StoreId");

            phone.Property<int>("Id");
            phone.HasKey("StoreId", "Id");

            phone.Property(x => x.Number)
                .HasMaxLength(20)
                .IsRequired();

            phone.Property(x => x.IsPrimary)
                .IsRequired();

            phone.HasIndex("StoreId", "Number")
                .IsUnique();
        });

        builder.OwnsMany(x => x.SocialMedia, socialMedia =>
        {
            socialMedia.ToTable("StoreSocialMedia");

            socialMedia.WithOwner()
                .HasForeignKey("StoreId");

            socialMedia.Property<int>("Id");
            socialMedia.HasKey("StoreId", "Id");

            socialMedia.Property(x => x.Link)
                .HasMaxLength(500)
                .IsRequired();

            socialMedia.Property(x => x.Platform)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
        });
    }
}
