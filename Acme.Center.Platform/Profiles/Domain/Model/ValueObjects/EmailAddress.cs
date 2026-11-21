namespace Acme.Center.Platform.Profiles.Domain.Model.ValueObjects;
/// <summary>
///     Email address value object.
/// </summary>
/// <param name="Address"></param>
public record EmailAddress(string Address)
{
    public EmailAddress() : this(string.Empty)
    {
    }
}