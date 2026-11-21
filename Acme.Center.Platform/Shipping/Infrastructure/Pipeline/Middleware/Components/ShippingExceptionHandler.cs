using Acme.Center.Platform.Shipping;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MySql.Data.MySqlClient;

namespace Acme.Center.Platform.Shipping.Infrastructure.Pipeline.Middleware.Components;

/// <summary>Maps shipping validation and unique-key exceptions to Problem Details.</summary>
/// <remarks>Leonardo Lino - PC2 study example.</remarks>
public sealed class ShippingExceptionHandler(
    ILogger<ShippingExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IStringLocalizer<ShippingMessages> localizer) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DbUpdateException dbException
            && dbException.InnerException is MySqlException mysql
            && mysql.Number == 1062)
        {
            logger.LogWarning(exception, "Duplicate shipment identifier detected by database");
            return await WriteProblem(context, StatusCodes.Status409Conflict,
                "Shipping.DuplicateIdentifier", exception, cancellationToken);
        }

        if (exception is not ArgumentException validationException)
            return false;

        var key = validationException.ParamName?.ToLowerInvariant() switch
        {
            "expecteddeliverydate" => "Shipping.InvalidPeriod",
            "currency" => "Shipping.InvalidCurrency",
            "packagecount" => "Shipping.InvalidPackageCount",
            "value" when exception is ArgumentOutOfRangeException => "Shipping.InvalidAmount",
            "status" => "Shipping.InvalidStatus",
            _ => "Shipping.InvalidData"
        };
        return await WriteProblem(context, StatusCodes.Status400BadRequest,
            key, exception, cancellationToken);
    }

    private async ValueTask<bool> WriteProblem(
        HttpContext context, int status, string key,
        Exception exception, CancellationToken cancellationToken)
    {
        context.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = key,
                Detail = localizer[key].Value,
                Instance = context.Request.Path
            }
        });
    }
}
