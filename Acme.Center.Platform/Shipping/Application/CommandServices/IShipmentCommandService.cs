using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;
using Acme.Center.Platform.Shipping.Domain.Model.Commands;

namespace Acme.Center.Platform.Shipping.Application.CommandServices;

/// <summary>Defines write operations for shipments.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public interface IShipmentCommandService
{
    Task<Result<Shipment>> Handle(CreateShipmentCommand command, CancellationToken cancellationToken);
}
