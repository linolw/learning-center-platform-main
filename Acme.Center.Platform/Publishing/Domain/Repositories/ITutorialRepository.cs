using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Shared.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Domain.Repositories;

public interface ITutorialRepository : IBaseRepository<Tutorial>
{
    
    Task<IEnumerable<Tutorial>> FindByCategoryIdAsync(int categoryId, CancellationToken cancellationToken);
    
    Task<IEnumerable<Tutorial>> FindByCategoryIdAsNoTrackingAsync(int categoryId, CancellationToken cancellationToken);
    
    Task<bool> ExistsByTitleAsync(string title, CancellationToken cancellationToken);
}