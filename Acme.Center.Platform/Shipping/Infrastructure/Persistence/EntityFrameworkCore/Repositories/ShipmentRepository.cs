using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Shipping.Domain.Model.Aggregates;
using Acme.Center.Platform.Shipping.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Acme.Center.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Implements shipment storage and identifier uniqueness checks.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public class ShipmentRepository(AppDbContext context)
    : BaseRepository<Shipment>(context), IShipmentRepository
{
    public Task<bool> ExistsByIdentifierAsync(
        Guid identifier, CancellationToken cancellationToken) =>
        Context.Set<Shipment>().AnyAsync(
            s => s.ShipmentIdentifier.Value == identifier, cancellationToken);
}
