using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Domain.Model.Entities;
using Acme.Center.Platform.Shared.Application.Model;

namespace Acme.Center.Platform.Publishing.Application.CommandServices;

public interface ICategoryCommandService
{
    public Task<Result<Category>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken = default);
}