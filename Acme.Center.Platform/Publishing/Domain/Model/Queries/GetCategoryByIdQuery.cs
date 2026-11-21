using System.Runtime.CompilerServices;
using Acme.Center.Platform.Publishing.Domain.Model.ValueObjects;

namespace Acme.Center.Platform.Publishing.Domain.Model.Queries;

public record GetCategoryByIdQuery(CategoryId CategoryId)
{
    public GetCategoryByIdQuery(int categoryId) : this(new CategoryId(categoryId))
    {
        
    }
}