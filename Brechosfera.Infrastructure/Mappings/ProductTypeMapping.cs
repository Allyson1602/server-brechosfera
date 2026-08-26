using Brechosfera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brechosfera.Infrastructure.Mappings;

public class ProductTypeMapping : EntityMapping<ProductType>
{
    public override void Configure(EntityTypeBuilder<ProductType> builder)
    {
        base.Configure(builder);

        builder.ToTable("producttypes");

        builder.Property(x => x.Key)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Key)
            .IsUnique();

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Category)
            .IsRequired();
    }
}
