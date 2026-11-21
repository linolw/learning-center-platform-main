using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

public static class CreateCategoryCommandFromResourceAssembler
{
    public static CreateCategoryCommand ToCommandFromResource(CreateCategoryResource resource)
    {
        if (resource == null)
            throw new ArgumentException(nameof(resource),
                "CreateCategoryCommand cannot be null when converting to command");
        
        return new CreateCategoryCommand(resource.Name); 
    }
}