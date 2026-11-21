using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Humanizer;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

public static class TutorialResourceFromEntityAssembler
{
    public static TutorialResource ToResourceFromEntity(Tutorial entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "Tutorial entity cannot be null when converting to resource");
        
        if (entity.Category == null)
            throw new InvalidOperationException(
                $"Tutorial with ID {entity.Id} has a null category. Ensure Category is properly loaded");
        
        return new TutorialResource(
            entity.Id,
            entity.Title,
            entity.Summary,
            CategoryResourceFromEntityAssembler.ToResourceFromEntity(entity.Category),
            entity.Status.Humanize());
    }
}