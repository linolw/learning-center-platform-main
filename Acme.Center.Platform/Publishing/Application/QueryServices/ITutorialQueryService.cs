using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;

namespace Acme.Center.Platform.Publishing.Application.QueryServices;

public interface ITutorialQueryService
{
    Task<Tutorial?> Handle(GetTutorialByIdQuery query, CancellationToken cancellationToken);
    
    Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsQuery query, CancellationToken cancellationToken);
    
    Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsByCategoryIdQuery query, CancellationToken cancellationToken);
}