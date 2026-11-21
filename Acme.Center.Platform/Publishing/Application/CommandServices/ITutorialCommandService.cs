using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Shared.Application.Model;

namespace Acme.Center.Platform.Publishing.Application.CommandServices;

public interface ITutorialCommandService
{
    Task<Result<Tutorial>> Handle(CreateTutorialCommand command, CancellationToken cancellationToken);
    
    
    Task<Result<Tutorial>> Handle(AddVideoAssetToTutorialCommand command, CancellationToken cancellationToken);
}