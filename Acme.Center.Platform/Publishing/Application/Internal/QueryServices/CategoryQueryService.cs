using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Domain.Model.Queries;
using Acme.Center.Platform.Publishing.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Application.Internal.QueryServices;

public class CategoryQueryService(ICategoryRepository categoryRepository)  : ICategoryQueryService
{
    public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
       return await categoryRepository.FindByIdAsNoTrackingAsync(query.CategoryId, cancellationToken);
                
    }

    public async Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
    {
      return await categoryRepository.ListAsNoTrackingAsync(cancellationToken);
    }
}