using Acme.Center.Platform.Shared.Domain.Repositories;
using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;

namespace Acme.Center.Platform.Shipping.Domain.Repositories;

/// <summary>Defines persistence operations for shipment aggregates.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<bool> ExistsByIdentifierAsync(Guid identifier, CancellationToken cancellationToken);
}
