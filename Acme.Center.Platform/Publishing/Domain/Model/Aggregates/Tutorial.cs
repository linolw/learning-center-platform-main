using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Domain.Model.Events;
using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Model.Aggregates.Internal;

namespace Acme.Center.Platform.Publishing.Domain.Model.Aggregates;

public partial class Tutorial : AggregateRoot
{
    public TutorialId Id { get; }
    public string Title { get; private set; }
    public string Summary { get; private set; }
    public Category Category { get; internal set; }
    public CategoryId CategoryId { get; internal set; }

    public Tutorial(string title, string summary, CategoryId categoryId) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(summary);
        Title = title;
        Summary = summary;
        CategoryId = categoryId;
        AddDomainEvent(new TutorialCreatedEvent(Id, Title, CategoryId));
    }

    public Tutorial(string title, string summary, int categoryId) : this(title, summary, new CategoryId(categoryId))
    {
        
    }

    public Tutorial(CreateTutorialCommand command) : this(command.Title, command.Summary, command.CategoryId)
    {
    }
}