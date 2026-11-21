using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Commands;

public record CreateTutorialCommand(string Title, string Summary, CategoryId CategoryId)
{
    public CreateTutorialCommand(string title, string summary, int categoryId)
        : this(title, summary, new CategoryId(categoryId))
    {}
}