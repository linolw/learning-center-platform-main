using Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Shipping.Domain.Model.Commands;

/// <summary>Requests the creation of one shipment.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record CreateShipmentCommand(
    Guid ShipmentIdentifier,
    Guid CarrierId,
    DateOnly PeriodStartDate,
    DateOnly PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    int PackageCount,
    ShipmentStatus? Status,
    string? Notes);
