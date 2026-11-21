using Acme.Center.Platform.Publishing.Domain.Model.Events;
using Acme.Center.Platform.Shared.Application.Internal.EventHandlers;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;

namespace Acme.Center.Platform.Publishing.Application.Internal.EventHandlers;

public class CategoryCreatedEventHandler(ILogger<CategoryCreatedEventHandler> logger) : IEventHandler<CategoryCreatedEvent>
{
    public  Task Handle(CategoryCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Category created: {Name}", domainEvent.Name);
        return Task.CompletedTask;
    }
}