using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Commands;

public record AddVideoAssetToTutorialCommand(string VideoUrl, TutorialId TutorialId)
{
    public AddVideoAssetToTutorialCommand(string videoUrl, int tutorialId)
        : this(videoUrl, new TutorialId(tutorialId))
    {
    }
}