using Brechosfera.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brechosfera.Infrastructure.Mappings;

public class EventMapping : EntityMapping<Event>
{
    public override void Configure(EntityTypeBuilder<Event> builder)
    {
        base.Configure(builder);

        builder.ToTable("Events");

        builder.Property(x => x.Banner)
            .HasColumnType("longtext")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.OrganizerName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.StartDate)
            .IsRequired();

        builder.Property(x => x.EndDate)
            .IsRequired();

        builder.Property(x => x.ResponsibleStoreId)
            .IsRequired();

        builder.OwnsOne(x => x.Address, address =>
        {
            address.ToTable("EventAddresses");

            address.WithOwner()
                .HasForeignKey("EventId");


            AddressMapping.ConfigureProperties(address);
        });
            

        builder.HasOne<Store>()
            .WithMany()
            .HasForeignKey(x => x.ResponsibleStoreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
