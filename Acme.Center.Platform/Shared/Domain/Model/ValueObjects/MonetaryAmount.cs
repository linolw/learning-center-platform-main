namespace Acme.Center.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Represents a nonnegative monetary amount and its currency.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record MonetaryAmount
{
    private MonetaryAmount() { } // Used by EF Core
    public decimal Value { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    public MonetaryAmount(decimal value, string currency)
    {
        if (value < decimal.Zero)
            throw new ArgumentOutOfRangeException(nameof(value),
                "Monetary value cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or blank.",
                nameof(currency));

        Value = value;
        Currency = currency;
    }

    public MonetaryAmount Multiply(decimal factor) =>
        new(Value * factor, Currency);
}
