using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;

public static class AddVideoAssetToTutorialCommandFromResourceAssembler
{
    public static AddVideoAssetToTutorialCommand ToCommandFromResource(
        AddVideoAssetToTutorialResource resource, int tutorialId)
    {
        if (resource == null)
            throw new ArgumentException(nameof(resource),
                "AddVideoAssetTutorialResource cannot be null when converting to command");
        
        return new AddVideoAssetToTutorialCommand(resource.VideoUrl, tutorialId); 
                
    }
}