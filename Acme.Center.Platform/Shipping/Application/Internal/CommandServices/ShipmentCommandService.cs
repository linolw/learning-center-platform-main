using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shared.Domain.Repositories;
using Acme.Center.Platform.Shipping.Application.CommandServices;
using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;
using Acme.Center.Platform.Shipping.Domain.Model.Commands;
using Acme.Center.Platform.Shipping.Domain.Model.Errors;
using Acme.Center.Platform.Shipping.Domain.Repositories;

namespace Acme.Center.Platform.Shipping.Application.Internal.CommandServices;

/// <summary>Creates shipments, checks uniqueness and commits changes.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public class ShipmentCommandService(
    IShipmentRepository shipmentRepository,
    IUnitOfWork unitOfWork) : IShipmentCommandService
{
    public async Task<Result<Shipment>> Handle(
        CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        if (await shipmentRepository.ExistsByIdentifierAsync(
                command.ShipmentIdentifier, cancellationToken))
            return Result<Shipment>.Failure(ShipmentErrors.DuplicateIdentifier);

        var shipment = new Shipment(command);
        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Shipment>.Success(shipment);
    }
}
