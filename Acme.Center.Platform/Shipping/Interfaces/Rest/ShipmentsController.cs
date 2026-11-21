using Acme.Center.Platform.Shared.Domain.Model.Errors;
using Acme.Center.Platform.Shipping.Application.CommandServices;
using Acme.Center.Platform.Shipping.Interfaces.Rest.Resources;
using Acme.Center.Platform.Shipping.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Acme.Center.Platform.Shipping.Interfaces.Rest;

/// <summary>Exposes the shipment creation REST endpoint.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
[ApiController]
[Route("api/v1/shipments")]
[Produces("application/json")]
[Tags("Shipments")]
public class ShipmentsController(
    IShipmentCommandService commandService,
    IStringLocalizer<ShippingMessages> localizer) : ControllerBase
{
    /// <summary>Creates a new shipment.</summary>
    /// <param name="resource">The shipment data, without an internal ID.</param>
    /// <param name="cancellationToken">Cancellation token for persistence.</param>
    /// <returns>201 Created with a resource, or a Problem Details error.</returns>
    /// <remarks>Leonardo Lino</remarks>
    [HttpPost]
    [EndpointSummary("Create shipment")]
    [EndpointDescription("Create a shipment using primitive request values")]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateShipment(
        [FromBody] CreateShipmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateShipmentCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await commandService.Handle(command, cancellationToken);

        if (result.IsSuccess)
            return StatusCode(StatusCodes.Status201Created,
                ShipmentResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));

        var error = result.DomainError;
        var status = error.Type switch
        {
            EErrorType.Validation => StatusCodes.Status400BadRequest,
            EErrorType.Conflict => StatusCodes.Status409Conflict,
            EErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
        return Problem(statusCode: status, title: error.Code,
            detail: localizer[error.Code].Value);
    }
}
