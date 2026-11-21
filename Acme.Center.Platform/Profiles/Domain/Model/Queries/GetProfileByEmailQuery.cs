using Acme.Center.Platform.Profiles.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Profiles.Domain.Model.Queries;

/// <summary>
/// Get profile by email query.
/// </summary>
public record GetProfileByEmailQuery(EmailAddress Email);