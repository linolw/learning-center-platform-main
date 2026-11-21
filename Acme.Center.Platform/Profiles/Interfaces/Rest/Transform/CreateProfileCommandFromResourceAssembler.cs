using Acme.Center.Platform.Profiles.Domain.Model.Commands;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming CreateProfileResource to CreateProfileCommand
/// </summary>
public static class CreateProfileCommandFromResourceAssembler
{
    public static CreateProfileCommand ToCommandFromResource(CreateProfileResource resource)
    {
        if (resource == null)
            throw new ArgumentException(nameof(resource),
                "CreateProfileResource cannot be null when converting to command");
        return new CreateProfileCommand(
            resource.FirstName,
            resource.LastName,
            resource.Email,
            resource.Street,
            resource.Number,
            resource.City,
            resource.PostalCode,
            resource.Country
            );
    }
}