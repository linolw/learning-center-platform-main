using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Shared.Application.Model;
using Acme.Center.Platform.Shared.Domain.Repositories;

namespace Acme.Center.Platform.Publishing.Application.Internal.CommandServices;

public class CategoryCommandService(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ICategoryCommandService
{
    public async Task<Result<Category>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var category = new Category(command);
        await categoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        return Result<Category>.Success(category);
    }
}