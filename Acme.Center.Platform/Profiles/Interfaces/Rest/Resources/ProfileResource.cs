namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

/// <summary>
///     Profile resource for Rest API
/// </summary>
public record ProfileResource(int Id, string FullName, string Email, string StreetAddress);