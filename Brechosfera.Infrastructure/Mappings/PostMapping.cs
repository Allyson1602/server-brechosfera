using Brechosfera.Domain.Entities;
using Brechosfera.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brechosfera.Infrastructure.Mappings;

public class PostMapping : EntityMapping<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);

        builder.ToTable("Post");

        builder.Property(x => x.Image)
            .HasColumnType("longtext");

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasOne<UserCredential>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(x => x.Likes, like =>
        {
            like.ToTable("PostLikes");

            like.WithOwner()
                .HasForeignKey("PostId");

            like.Property(x => x.UserId)
                .IsRequired();

            like.HasKey("PostId", nameof(PostLike.UserId));

            like.HasOne<UserCredential>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
