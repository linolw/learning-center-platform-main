namespace Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

/// <summary>Possible states of a shipment.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public enum ShipmentStatus
{
    Pending,
    InTransit,
    Delivered,
    Cancelled
}
