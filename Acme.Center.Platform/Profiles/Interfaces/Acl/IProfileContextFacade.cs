namespace Acme.Center.Platform.Profiles.Interfaces.Acl;
/// <summary>
///     Facade for profile context.
/// </summary>
public interface IProfileContextFacade
{
    Task<int> CreateProfile(
        string firstName,
        string lastName,
        string email,
        string street,
        string number,
        string city,
        string postalCode,
        string country,
        CancellationToken cancellationToken);

    Task<int> FetchProfileIdByEmail(string email, CancellationToken cancellationToken);
}