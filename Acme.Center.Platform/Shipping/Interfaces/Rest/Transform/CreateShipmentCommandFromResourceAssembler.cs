using Acme.Center.Platform.Shipping.Domain.Model.Commands;
using Acme.Center.Platform.Shipping.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Shipping.Interfaces.Rest.Transform;

/// <summary>Converts an HTTP request resource to a domain command.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public static class CreateShipmentCommandFromResourceAssembler
{
    public static CreateShipmentCommand ToCommandFromResource(CreateShipmentResource resource) =>
        new(resource.ShipmentIdentifier, resource.CarrierId, resource.PeriodStartDate,
            resource.PeriodExpectedDeliveryDate, resource.ShippingCostValue,
            resource.ShippingCostCurrency, resource.PackageCount, resource.Status, resource.Notes);
}
