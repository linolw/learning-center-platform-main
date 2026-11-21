using Acme.Center.Platform.Publishing.Domain.Model.Aggregates;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Humanizer;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

public static class CategoryResourceFromEntityAssembler
{
    public static CategoryResource ToResourceFromEntity(Category entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity),
                "CategoryResource entity cannot be null when converting to resource");

        return new CategoryResource(entity.Id, entity.Name);
    }
}