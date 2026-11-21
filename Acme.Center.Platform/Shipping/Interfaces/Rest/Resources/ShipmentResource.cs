namespace Acme.Center.Platform.Shipping.Interfaces.Rest.Resources;

/// <summary>HTTP response after a shipment has been created.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record ShipmentResource(
    int Id,
    string ShipmentId,
    string CarrierId,
    string PeriodStartDate,
    string PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    string Status,
    int PackageCount,
    string? Notes);
