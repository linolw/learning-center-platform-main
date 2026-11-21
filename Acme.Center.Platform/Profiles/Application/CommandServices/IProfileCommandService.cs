using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Domain.Model.Commands;
using Acme.Center.Platform.Shared.Application.Model;

namespace Acme.Center.Platform.Profiles.Application.CommandServices;

/// <summary>
///     Profile command service
/// </summary>
public interface IProfileCommandService
{
    Task<Result<Profile>> Handle(CreateProfileCommand command, CancellationToken cancellationToken);
}