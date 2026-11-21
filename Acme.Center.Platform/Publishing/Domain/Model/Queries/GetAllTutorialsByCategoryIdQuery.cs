using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Queries;

public record GetAllTutorialsByCategoryIdQuery(CategoryId CategoryId)
{
    public GetAllTutorialsByCategoryIdQuery(int categoryId) : this((new CategoryId(categoryId)))
    {
        
    }
}