using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Acme.Center.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

/// <summary>Defines EF Core mappings for the Shipping bounded context.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public static class ModelBuilderExtensions
{
    public static void ApplyShippingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Shipment>(entity =>
        {
            entity.ToTable("shipments");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Id).ValueGeneratedOnAdd();

            entity.OwnsOne(s => s.ShipmentIdentifier, identifier =>
            {
                identifier.Property(x => x.Value)
                    .HasColumnName("shipment_identifier").IsRequired();
                identifier.HasIndex(x => x.Value).IsUnique();
            });
            entity.Navigation(s => s.ShipmentIdentifier).IsRequired();

            entity.OwnsOne(s => s.CarrierId, carrier =>
            {
                carrier.Property(x => x.Value)
                    .HasColumnName("carrier_id").IsRequired();
            });
            entity.Navigation(s => s.CarrierId).IsRequired();

            entity.OwnsOne(s => s.Period, period =>
            {
                period.Property(x => x.StartDate)
                    .HasColumnName("period_start_date").IsRequired();
                period.Property(x => x.ExpectedDeliveryDate)
                    .HasColumnName("period_expected_delivery_date").IsRequired();
            });
            entity.Navigation(s => s.Period).IsRequired();

            entity.OwnsOne(s => s.ShippingCost, amount =>
            {
                amount.Property(x => x.Value)
                    .HasColumnName("shipping_cost_value").HasPrecision(18, 2).IsRequired();
                amount.Property(x => x.Currency)
                    .HasColumnName("shipping_cost_currency").HasMaxLength(3).IsRequired();
            });
            entity.Navigation(s => s.ShippingCost).IsRequired();

            entity.Property(s => s.Status).HasConversion<string>()
                .HasMaxLength(20).IsRequired();
            entity.Property(s => s.PackageCount).IsRequired();
            entity.Property(s => s.Notes).IsRequired(false);
            entity.Property(s => s.CreatedAt);
            entity.Property(s => s.UpdatedAt);
        });
    }
}
