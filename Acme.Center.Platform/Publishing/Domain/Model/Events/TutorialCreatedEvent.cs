using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Model.Events;

namespace Acme.Center.Platform.Publishing.Domain.Model.Events;

public record TutorialCreatedEvent(TutorialId TutorialId, string Title, CategoryId CategoryId) : IEvent;