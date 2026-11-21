using Acme.Center.Platform.Profiles.Domain.Model.Aggregates;
using Acme.Center.Platform.Profiles.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Profiles.Interfaces.Rest.Transform;
/// <summary>
///     Assembler responsible for transforming Profile entity to ProfileResource
/// </summary>
public static class ProfileResourceFromEntityAssembler
{
    public static ProfileResource ToResourceFromEntity(Profile entity)
    {
        if (entity == null)
            throw new ArgumentException(nameof(entity), "Profile entity cannot be null when converting to resource");

        return new ProfileResource(entity.Id, entity.FullName, entity.EmailAddress, entity.StreetAddress);
    }
}