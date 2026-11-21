using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;

namespace Acme.Center.Platform.Publishing.Application.QueryServices;

public interface ICategoryQueryService
{
    Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken);
    
    
    Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken);
}