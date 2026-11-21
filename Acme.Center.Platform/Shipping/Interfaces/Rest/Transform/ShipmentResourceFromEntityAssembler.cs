using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;
using Acme.Center.Platform.Shipping.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Shipping.Interfaces.Rest.Transform;

/// <summary>Converts an aggregate to its HTTP response resource.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public static class ShipmentResourceFromEntityAssembler
{
    public static ShipmentResource ToResourceFromEntity(Shipment shipment) =>
        new(shipment.Id, shipment.ShipmentIdentifier.Value.ToString(),
            shipment.CarrierId.Value.ToString(),
            shipment.Period.StartDate.ToString("yyyy-MM-dd"),
            shipment.Period.ExpectedDeliveryDate.ToString("yyyy-MM-dd"),
            shipment.ShippingCost.Value, shipment.ShippingCost.Currency,
            shipment.Status.ToString(), shipment.PackageCount, shipment.Notes);
}
