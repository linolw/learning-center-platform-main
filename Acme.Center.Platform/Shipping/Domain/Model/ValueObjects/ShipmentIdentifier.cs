namespace Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

/// <summary>Identifies a shipment created in an external bounded context.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record ShipmentIdentifier
{
    private ShipmentIdentifier() { } // Used by EF Core
    public Guid Value { get; private set; }

    public ShipmentIdentifier(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Shipment identifier cannot be empty.", nameof(value));
        Value = value;
    }
}
