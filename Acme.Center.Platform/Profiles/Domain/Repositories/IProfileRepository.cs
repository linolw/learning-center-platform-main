using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Repositories;

namespace Acme.Center.Platform.Profiles.Domain.Repositories;
/// <summary>
///     Profile repository interface
/// </summary>
public interface IProfileRepository : IBaseRepository<Profile>
{
    /// <summary>
    ///     Find profile by email
    /// </summary>
    /// <returns> The <see cref="Profile"/> if found, otherwise null</returns>
    Task<Profile?> FindByProfileByEmailAsync(EmailAddress email, CancellationToken cancellationToken);
    
    /// <summary>
    ///     find a profile by email without change tracking
    /// </summary>
    /// <returns> The <see cref="Profile"/> if found, otherwise null</returns>
    Task<Profile?> FindProfileByEmailAsNoTrackingAsync(EmailAddress email, CancellationToken cancellationToken);

    /// <summary>
    ///     Find a profile by id without change tracking
    /// </summary>
    /// <returns> The <see cref="Profile"/> if found, otherwise null</returns>
    Task<Profile?> FindByIdAsNoTrackingAsync(ProfileId id, CancellationToken cancellationToken);

}