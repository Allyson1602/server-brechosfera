using Brechosfera.Domain.Entities;
using Brechosfera.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Brechosfera.Infrastructure.Mappings;

public class UserCredentialMapping : EntityMapping<UserCredential>
{
    public override void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserCredentials");

        builder.Property(x => x.Username)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Username)
            .IsUnicode();

        builder.Property(x => x.Email)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.IsEmailConfirmed)
            .HasDefaultValue(false);

        builder.Property(x => x.Roles)
            .HasColumnName("Roles")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<ERole>>(v, (JsonSerializerOptions?)null) ?? new List<ERole>()
            )
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}