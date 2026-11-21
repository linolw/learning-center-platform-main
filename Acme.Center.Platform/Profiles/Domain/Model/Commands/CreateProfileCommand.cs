namespace Acme.Center.Platform.Profiles.Domain.Model.Commands;

/// <summary>
///     Create a new profile
/// </summary>

public record CreateProfileCommand(
    string FirstName,
    string LastName,
    string Email,
    string Street,
    string Number,
    string City,
    string PostalCode,
    string Country);