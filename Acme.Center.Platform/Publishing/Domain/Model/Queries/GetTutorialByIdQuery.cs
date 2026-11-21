using System.Runtime.CompilerServices;
using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Queries;

public record GetTutorialByIdQuery(TutorialId TutorialId)
{
    public GetTutorialByIdQuery(int tutorialId) : this(new TutorialId(tutorialId))
    {
        
    }
}