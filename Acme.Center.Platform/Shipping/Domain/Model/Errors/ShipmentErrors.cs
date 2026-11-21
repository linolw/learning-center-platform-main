using Acme.Center.Platform.Shared.Domain.Model.Errors;

namespace Acme.Center.Platform.Shipping.Domain.Model.Errors;

/// <summary>Domain errors returned when shipment creation is rejected.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public static class ShipmentErrors
{
    public static readonly Error DuplicateIdentifier =
        Error.Conflict("Shipping.DuplicateIdentifier",
            "A shipment with this identifier already exists.");
}
