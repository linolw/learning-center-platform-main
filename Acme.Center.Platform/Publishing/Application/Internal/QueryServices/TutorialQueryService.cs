using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Application.Internal.QueryServices;

public class TutorialQueryService(ITutorialRepository tutorialRepository) : ITutorialQueryService
{
    public async Task<Tutorial?> Handle(GetTutorialByIdQuery query, CancellationToken cancellationToken)
    {
        return await tutorialRepository.FindByIdAsNoTrackingAsync(query.TutorialId, cancellationToken);
    }

    public async Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsQuery query, CancellationToken cancellationToken)
    {
        return await tutorialRepository.ListAsNoTrackingAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tutorial>> Handle(GetAllTutorialsByCategoryIdQuery query, CancellationToken cancellationToken)
    {
        return await tutorialRepository.FindByCategoryIdAsNoTrackingAsync(query.CategoryId, cancellationToken);
    }
}