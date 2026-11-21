using Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Shipping.Interfaces.Rest.Resources;

/// <summary>HTTP request data for creating a shipment.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record CreateShipmentResource(
    Guid ShipmentIdentifier,
    Guid CarrierId,
    DateOnly PeriodStartDate,
    DateOnly PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    int PackageCount,
    ShipmentStatus? Status,
    string? Notes);
