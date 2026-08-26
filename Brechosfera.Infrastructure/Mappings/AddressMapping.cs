using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Brechosfera.Domain.ValueObjects;
using Brechosfera.Domain.Abstractions;

namespace Brechosfera.Infrastructure.Mappings;

public static class AddressMapping
{
    public static void ConfigureProperties<TOwner>(
        OwnedNavigationBuilder<TOwner, Address> address
    ) where TOwner : Entity
    {
        address.Property(x => x.Cep)
            .HasMaxLength(8)
            .IsRequired();

        address.Property(x => x.Street)
            .HasMaxLength(150)
            .IsRequired();

        address.Property(x => x.Neighborhood)
            .HasMaxLength(100)
            .IsRequired();

        address.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        address.Property(x => x.State)
            .HasMaxLength(100)
            .IsRequired();

        address.Property(x => x.StateCode)
            .HasMaxLength(2)
            .IsRequired();

        address.Property(x => x.Complement)
            .HasMaxLength(150);
    }
}
