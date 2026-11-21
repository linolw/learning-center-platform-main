namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

/// <summary>
///     Resource for creating a profile
/// </summary>
public record CreateProfileResource(
    string FirstName,
    string LastName,
    string Email,
    string Street,
    string Number,
    string City,
    string PostalCode, 
    string Country);