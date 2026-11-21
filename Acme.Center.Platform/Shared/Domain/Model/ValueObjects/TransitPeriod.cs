namespace Acme.Center.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Defines the start and expected delivery dates of a shipment.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public record TransitPeriod
{
    private TransitPeriod() { } // Used by EF Core
    public DateOnly StartDate { get; private set; }
    public DateOnly ExpectedDeliveryDate { get; private set; }

    public TransitPeriod(DateOnly startDate, DateOnly expectedDeliveryDate)
    {
        if (expectedDeliveryDate <= startDate)
            throw new ArgumentException("Expected delivery date must be after start date.",
                nameof(expectedDeliveryDate));

        StartDate = startDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
    }

    public bool IsOverdue(DateOnly checkDate) => checkDate > ExpectedDeliveryDate;
}
