using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brechosfera.Infrastructure.Mappings;

public class StoreReviewMapping : EntityMapping<StoreReview>
{
    public override void Configure(EntityTypeBuilder<StoreReview> builder)
    {
        base.Configure(builder);

        builder.ToTable("Storereviews");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.StoreId);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.StoreId).IsRequired();

        builder.Property(x => x.Rating)
            .HasConversion(
                rating => rating.Value,
                value => Rating.Create(value)
            )
            .HasColumnName("Rating")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}
