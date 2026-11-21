namespace Acme.Center.Platform.Shipping.Domain.Model.ValueObjects;

/// <summary>Identifies the carrier in an external bounded context.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record CarrierId
{
    private CarrierId() { } // Used by EF Core
    public Guid Value { get; private set; }

    public CarrierId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Carrier identifier cannot be empty.", nameof(value));
        Value = value;
    }
}
