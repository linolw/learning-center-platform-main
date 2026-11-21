using Acme.Center.Platform.Shared.Domain.Model.Aggregates.Internal;
using Acme.Center.Platform.Shared.Domain.Model.Entities;
using Acme.Center.Platform.Shared.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shipping.Domain.Model.Commands;
using Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Shipping.Domain.Model.Aggregates;

/// <summary>Aggregate root for a shipment, protecting its invariants.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public partial class Shipment : AggregateRoot, IAuditableEntity
{
    private Shipment() { } // Used by EF Core

    public int Id { get; private set; }
    public ShipmentIdentifier ShipmentIdentifier { get; private set; } = null!;
    public CarrierId CarrierId { get; private set; } = null!;
    public TransitPeriod Period { get; private set; } = null!;
    public MonetaryAmount ShippingCost { get; private set; } = null!;
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Pending;
    public int PackageCount { get; private set; }
    public string? Notes { get; private set; }

    // Exact IAuditableEntity contract from Learning Center
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Shipment(CreateShipmentCommand command)
    {
        if (command.PackageCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(command.PackageCount),
                "Package count must be greater than zero.");
        if (command.Status.HasValue && !Enum.IsDefined(command.Status.Value))
            throw new ArgumentException("Invalid shipment status.", nameof(command.Status));

        ShipmentIdentifier = new ShipmentIdentifier(command.ShipmentIdentifier);
        CarrierId = new CarrierId(command.CarrierId);
        Period = new TransitPeriod(command.PeriodStartDate, command.PeriodExpectedDeliveryDate);
        ShippingCost = new MonetaryAmount(command.ShippingCostValue, command.ShippingCostCurrency);
        Status = command.Status ?? ShipmentStatus.Pending;
        PackageCount = command.PackageCount;
        Notes = command.Notes;
    }
}
