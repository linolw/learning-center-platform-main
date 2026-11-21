using Acme.Center.Platform.Publishing.Domain.Model.Events;
using Acme.Center.Platform.Shared.Application.Internal.EventHandlers;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace Acme.Center.Platform.Publishing.Application.Internal.EventHandlers;

public class TutorialCreatedEventHandler(ILogger<TutorialCreatedEventHandler> logger) : IEventHandler<TutorialCreatedEvent>
{
    public  Task Handle(TutorialCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Tutorial created: {Title} (ID: {TutorialId}, CategoryID: {CategoryId})", 
            domainEvent.Title, domainEvent.TutorialId.Value, domainEvent.CategoryId.Value);
        return Task.CompletedTask;
    }
}