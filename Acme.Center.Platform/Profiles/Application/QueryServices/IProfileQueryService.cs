using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Domain.Model.Queries;

namespace Acme.Center.Platform.Profiles.Application.QueryServices;

/// <summary>
///     Profile query service
/// </summary>
public interface IProfileQueryService
{
    Task<IEnumerable<Profile>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken);
    
    Task<Profile?> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken);
    
    Task<Profile?> Handle(GetProfileByEmailQuery query, CancellationToken cancellationToken);
}