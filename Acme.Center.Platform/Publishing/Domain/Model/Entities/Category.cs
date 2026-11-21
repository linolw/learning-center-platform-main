using System.Runtime.CompilerServices;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Model.Events;
using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;
using Acme.Center.Platform.Shared.Domain.Model.Aggregates.Internal;

namespace Acme.Center.Platform.Publishing.Domain.Model.Entities;

public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public CategoryId Id { get; private set; }


    public Category()
    {
        Name = string.Empty;
    }

    public Category(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        AddDomainEvent(new CategoryCreatedEvent(name));
    }

    public Category(CreateCategoryCommand command) : this(command.Name)
    {
    }

    public Category UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        return this;
    }
}