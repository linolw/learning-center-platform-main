using System.Net.Mime;
using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Model.Commands;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Resources;
using Acme.Center.Platform.Publishing.Interfaces.Rest.Transform;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Tls.Crypto.Impl;

namespace Acme.Center.Platform.Publishing.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Category Endpoints")]
public class CategoriesController(
    ICategoryCommandService categoryCommandService,
    ICategoryQueryService categoryQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory) : ControllerBase
{
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryResource resource,
        CancellationToken cancellationToken)
    {
        var createCategoryCommand = CreateCategoryCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await categoryCommandService.Handle(createCategoryCommand, cancellationToken);
        return PublishingActionResultAssembler.ToActionResultFromCreateCategoryResult(
            this,
            result,
            _errorLocalizer,
            _problemDetailsFactory,
            createdCategory => CreatedAtAction(nameof(GetCategoryById), new { categoryId = createdCategory.Id},
                CategoryResourceFromEntityAssembler.ToResourceFromEntity(createdCategory)));
    }
}