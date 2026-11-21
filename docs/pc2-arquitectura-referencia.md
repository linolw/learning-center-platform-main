# Arquitectura completa de referencia para PC2 — POST + DDD

**Autor de la adaptación didáctica:** Leonardo Lino.  
**Referencia:** patrones verificados en `Acme.Center.Platform/` del repositorio Learning Center.  
**Caso utilizado:** Shipment de **Shipping**, tomado del simulacro; no es código original del profesor ni parte del ejecutable Learning Center.

> **Importante:** este es un **blueprint por archivos** pensado para copiar/adaptar en una solución **separada** de Rider. Los bloques no son un proyecto compilado ni sustituyen la verificación de reglas, mappings, paquetes, localización y respuestas exigidas por cada PDF. Reemplaza `Exam.Platform` por el namespace raíz de tu solución. No pegues estos archivos directamente dentro del proyecto Learning Center de referencia.

## 1. Dependencias y capas

```text
Cliente HTTP
    |
    v
Shipping/Interfaces/Rest/ShipmentsController
    |  ↔ CreateShipmentResource / ShipmentResource / Assemblers
    v
Shipping/Application/Internal/CommandServices/ShipmentCommandService
    | ↔ Result<Shipment>, IUnitOfWork, IShipmentRepository
    v
Shipping/Domain/Model/Aggregates/Shipment (AggregateRoot, IAuditableEntity)
    | ↔ Commands, ValueObjects, Errors
    v
Shipping/Infrastructure/Persistence/EntityFrameworkCore/Repositories/ShipmentRepository
    |
    v
Shared/Infrastructure/Persistence/EntityFrameworkCore/Configuration/AppDbContext
    | ↔ Fluent API, MySQL, interceptores y migrations
    v
MySQL
```

Las **dependencias de código** no deben representar exactamente este flujo de ejecución: `Domain` no conoce `Infrastructure` ni `Interfaces`. Infrastructure **implementa** interfaces declaradas en Domain. `Program.cs` compone las dependencias.

### Estructura objetivo

```text
Exam.Platform/
├── Shipping/
│   ├── Domain/
│   │   ├── Model/
│   │   │   ├── Aggregates/Shipment.cs
│   │   │   ├── Commands/CreateShipmentCommand.cs
│   │   │   ├── Errors/ShipmentErrors.cs
│   │   │   └── ValueObjects/
│   │   │       ├── ShipmentIdentifier.cs
│   │   │       ├── CarrierId.cs
│   │   │       └── ShipmentStatus.cs
│   │   └── Repositories/IShipmentRepository.cs
│   ├── Application/
│   │   ├── CommandServices/IShipmentCommandService.cs
│   │   └── Internal/CommandServices/ShipmentCommandService.cs
│   ├── Infrastructure/
│   │   └── Persistence/EntityFrameworkCore/
│   │       ├── Configuration/Extensions/ModelBuilderExtensions.cs
│   │       └── Repositories/ShipmentRepository.cs
│   └── Interfaces/
│       └── Rest/
│           ├── ShipmentsController.cs
│           ├── Resources/CreateShipmentResource.cs
│           ├── Resources/ShipmentResource.cs
│           ├── Transform/CreateShipmentCommandFromResourceAssembler.cs
│           └── Transform/ShipmentResourceFromEntityAssembler.cs
├── Shared/
│   ├── Application/Model/Result.cs
│   ├── Domain/Model/Aggregates/{IAggregateRoot.cs,Internal/AggregateRoot.cs}
│   ├── Domain/Model/Entities/IAuditableEntity.cs
│   ├── Domain/Model/Events/IEvent.cs
│   ├── Domain/Model/Errors/Error.cs
│   ├── Domain/Model/ValueObjects/{TransitPeriod.cs,MonetaryAmount.cs}
│   ├── Domain/Repositories/{IBaseRepository.cs,IUnitOfWork.cs}
│   └── Infrastructure/Persistence/EntityFrameworkCore/...
├── Program.cs
└── appsettings.json
```

**Qué se copia:** contratos originales de Shared (Result, Error, AggregateRoot, repositorios y auditoría) manteniendo firmas; **qué se adapta:** el namespace, las dependencias, la configuración EF, los Value Objects del caso, errores y recursos locales.

## 2. Domain: identificadores externos

Ruta: `Shipping/Domain/Model/ValueObjects/ShipmentIdentifier.cs`

```csharp
namespace Exam.Platform.Shipping.Domain.Model.ValueObjects;

/// <summary>Identifies an external shipment document.</summary>
/// <remarks>Leonardo Lino</remarks>
public record ShipmentIdentifier
{
    private ShipmentIdentifier() { } // EF Core
    public Guid Value { get; private set; }

    public ShipmentIdentifier(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Shipment identifier cannot be empty.", nameof(value));
        Value = value;
    }
}
```

Ruta: `Shipping/Domain/Model/ValueObjects/CarrierId.cs`

```csharp
namespace Exam.Platform.Shipping.Domain.Model.ValueObjects;

/// <summary>Identifies a carrier in an external context.</summary>
/// <remarks>Leonardo Lino</remarks>
public record CarrierId
{
    private CarrierId() { } // EF Core
    public Guid Value { get; private set; }

    public CarrierId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Carrier identifier cannot be empty.", nameof(value));
        Value = value;
    }
}
```

**Regla de interpretación:** `ShipmentIdentifier (ShipmentIdentifier)` y `CarrierId (CarrierId)` significan **nombre de propiedad (tipo C#)**. El `int Id` de la fila MySQL es otro identificador distinto.

## 3. Shared: periodo y dinero específicos

Aunque se ubiquen en Shared por el enunciado, **no provienen del Shared original**: son Value Objects nuevos del caso.

Ruta: `Shared/Domain/Model/ValueObjects/TransitPeriod.cs`

```csharp
namespace Exam.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Defines the valid transit date range.</summary>
/// <remarks>Leonardo Lino</remarks>
public record TransitPeriod
{
    private TransitPeriod() { } // EF Core
    public DateOnly StartDate { get; private set; }
    public DateOnly ExpectedDeliveryDate { get; private set; }

    public TransitPeriod(DateOnly startDate, DateOnly expectedDeliveryDate)
    {
        if (expectedDeliveryDate <= startDate)
            throw new ArgumentException("Expected delivery date must be later than start date.");
        StartDate = startDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
    }

    public bool IsOverdue(DateOnly checkDate) => checkDate > ExpectedDeliveryDate;
}
```

Ruta: `Shared/Domain/Model/ValueObjects/MonetaryAmount.cs`

```csharp
namespace Exam.Platform.Shared.Domain.Model.ValueObjects;

/// <summary>Represents a non-negative monetary amount in one currency.</summary>
/// <remarks>Leonardo Lino</remarks>
public record MonetaryAmount
{
    private MonetaryAmount() { } // EF Core
    public decimal Value { get; private set; }
    public string Currency { get; private set; } = string.Empty;

    public MonetaryAmount(decimal value, string currency)
    {
        if (value < decimal.Zero)
            throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));
        Value = value;
        Currency = currency;
    }

    public MonetaryAmount Multiply(decimal factor) => new(Value * factor, Currency);
}
```

**Comprensión:** el constructor protege invariantes; `Multiply` vuelve a pasar por el constructor, por lo que un resultado negativo no queda permitido. En Capbase/CUTS se reutiliza la idea, pero el periodo y los nombres cambian según PDF.

## 4. Domain: enum y command

Ruta: `Shipping/Domain/Model/ValueObjects/ShipmentStatus.cs`

```csharp
namespace Exam.Platform.Shipping.Domain.Model.ValueObjects;

public enum ShipmentStatus { Pending, InTransit, Delivered, Cancelled }
```

Ruta: `Shipping/Domain/Model/Commands/CreateShipmentCommand.cs`

```csharp
using Exam.Platform.Shipping.Domain.Model.ValueObjects;
namespace Exam.Platform.Shipping.Domain.Model.Commands;

/// <summary>Request to create a shipment.</summary>
/// <remarks>Leonardo Lino</remarks>
public record CreateShipmentCommand(
    Guid ShipmentIdentifier,
    Guid CarrierId,
    DateOnly PeriodStartDate,
    DateOnly PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    int PackageCount,
    ShipmentStatus? Status,
    string? Notes
);
```

**Regla:** el Command es un `record` que representa la intención; no guarda en MySQL ni construye respuestas HTTP. Aquí no hay `Id` porque lo genera la BD.

## 5. Domain: Aggregate Root auditable

Ruta: `Shipping/Domain/Model/Aggregates/Shipment.cs`

```csharp
using Exam.Platform.Shared.Domain.Model.Aggregates.Internal;
using Exam.Platform.Shared.Domain.Model.Entities;
using Exam.Platform.Shared.Domain.Model.ValueObjects;
using Exam.Platform.Shipping.Domain.Model.Commands;
using Exam.Platform.Shipping.Domain.Model.ValueObjects;

namespace Exam.Platform.Shipping.Domain.Model.Aggregates;

/// <summary>The shipment aggregate root enforcing business invariants.</summary>
/// <remarks>Leonardo Lino</remarks>
public partial class Shipment : AggregateRoot, IAuditableEntity
{
    private Shipment() { } // EF Core

    public int Id { get; private set; }
    public ShipmentIdentifier ShipmentIdentifier { get; private set; } = null!;
    public CarrierId CarrierId { get; private set; } = null!;
    public TransitPeriod Period { get; private set; } = null!;
    public MonetaryAmount ShippingCost { get; private set; } = null!;
    public ShipmentStatus Status { get; private set; } = ShipmentStatus.Pending;
    public int PackageCount { get; private set; }
    public string? Notes { get; private set; }

    // EXACT types required by the professor's Shared:
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Shipment(CreateShipmentCommand command)
    {
        if (command.PackageCount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(command.PackageCount), "Package count must be greater than zero.");

        ShipmentIdentifier = new ShipmentIdentifier(command.ShipmentIdentifier);
        CarrierId = new CarrierId(command.CarrierId);
        Period = new TransitPeriod(command.PeriodStartDate, command.PeriodExpectedDeliveryDate);
        ShippingCost = new MonetaryAmount(command.ShippingCostValue, command.ShippingCostCurrency);
        Status = command.Status ?? ShipmentStatus.Pending;
        PackageCount = command.PackageCount;
        Notes = command.Notes;
    }
}
```

**Importante:** `AggregateRoot` del profesor NO aporta `Id` ni los campos de auditoría, así que se agregan a `Shipment`. Las fechas no se establecen a mano en el service: el interceptor de Shared las completa al persistir.

## 6. Domain: errores y repositorio

Ruta: `Shipping/Domain/Model/Errors/ShipmentErrors.cs`

```csharp
using Exam.Platform.Shared.Domain.Model.Errors;
namespace Exam.Platform.Shipping.Domain.Model.Errors;

public static class ShipmentErrors
{
    public static readonly Error DuplicateShipmentIdentifier =
        Error.Conflict("Shipping.DuplicateShipmentIdentifier",
                       "A shipment with this identifier already exists.");
}
```

Ruta: `Shipping/Domain/Repositories/IShipmentRepository.cs`

```csharp
using Exam.Platform.Shared.Domain.Repositories;
using Exam.Platform.Shipping.Domain.Model.Aggregates;
namespace Exam.Platform.Shipping.Domain.Repositories;

public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<bool> ExistsByIdentifierAsync(Guid identifier, CancellationToken cancellationToken);
}
```

La consulta `ExistsByIdentifierAsync` se deriva directamente de “no registrar dos envíos con el mismo identificador”. La duplicidad necesita **comprobación en el Service y restricción única en MySQL**.

## 7. Application: interfaz y CommandService

Ruta: `Shipping/Application/CommandServices/IShipmentCommandService.cs`

```csharp
using Exam.Platform.Shared.Application.Model;
using Exam.Platform.Shipping.Domain.Model.Aggregates;
using Exam.Platform.Shipping.Domain.Model.Commands;
namespace Exam.Platform.Shipping.Application.CommandServices;

public interface IShipmentCommandService
{
    Task<Result<Shipment>> Handle(
        CreateShipmentCommand command, CancellationToken cancellationToken);
}
```

Ruta: `Shipping/Application/Internal/CommandServices/ShipmentCommandService.cs`

```csharp
using Exam.Platform.Shared.Application.Model;
using Exam.Platform.Shared.Domain.Repositories;
using Exam.Platform.Shipping.Application.CommandServices;
using Exam.Platform.Shipping.Domain.Model.Aggregates;
using Exam.Platform.Shipping.Domain.Model.Commands;
using Exam.Platform.Shipping.Domain.Model.Errors;
using Exam.Platform.Shipping.Domain.Repositories;
namespace Exam.Platform.Shipping.Application.Internal.CommandServices;

/// <summary>Handles the shipment creation command.</summary>
/// <remarks>Leonardo Lino</remarks>
public class ShipmentCommandService(
    IShipmentRepository shipmentRepository,
    IUnitOfWork unitOfWork) : IShipmentCommandService
{
    public async Task<Result<Shipment>> Handle(
        CreateShipmentCommand command, CancellationToken cancellationToken)
    {
        if (await shipmentRepository.ExistsByIdentifierAsync(
                command.ShipmentIdentifier, cancellationToken))
            return Result<Shipment>.Failure(ShipmentErrors.DuplicateShipmentIdentifier);

        var shipment = new Shipment(command);
        await shipmentRepository.AddAsync(shipment, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Shipment>.Success(shipment);
    }
}
```

Este ejemplo conserva las **firmas reales** `Result<T>.Failure(Error)`, `AddAsync` y `IUnitOfWork.CompleteAsync` de Learning Center. Las excepciones `ArgumentException` producidas por Value Objects todavía deben mapearse a HTTP 400.

## 8. Infrastructure: Repository EF Core

Ruta: `Shipping/Infrastructure/Persistence/EntityFrameworkCore/Repositories/ShipmentRepository.cs`

```csharp
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Exam.Platform.Shipping.Domain.Model.Aggregates;
using Exam.Platform.Shipping.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Exam.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ShipmentRepository(AppDbContext context)
    : BaseRepository<Shipment>(context), IShipmentRepository
{
    public Task<bool> ExistsByIdentifierAsync(
        Guid identifier, CancellationToken cancellationToken)
    {
        return Context.Set<Shipment>().AnyAsync(
            s => s.ShipmentIdentifier.Value == identifier,
            cancellationToken);
    }
}
```

**Qué es propio de Infrastructure:** `DbContext`, LINQ traducido a SQL y EF Core. Domain solo conoce la interfaz `IShipmentRepository`.

## 9. Infrastructure: Fluent API

Ruta: `Shipping/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs`

```csharp
using Exam.Platform.Shipping.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
namespace Exam.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyShippingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Shipment>(entity =>
        {
            entity.ToTable("shipments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();

            entity.OwnsOne(x => x.ShipmentIdentifier, owned =>
            {
                owned.Property(p => p.Value)
                    .HasColumnName("shipment_identifier").IsRequired();
                owned.HasIndex(p => p.Value).IsUnique();
            });
            entity.Navigation(x => x.ShipmentIdentifier).IsRequired();

            entity.OwnsOne(x => x.CarrierId, owned =>
            {
                owned.Property(p => p.Value)
                    .HasColumnName("carrier_id").IsRequired();
            });
            entity.Navigation(x => x.CarrierId).IsRequired();

            entity.OwnsOne(x => x.Period, owned =>
            {
                owned.Property(p => p.StartDate)
                    .HasColumnName("period_start_date").IsRequired();
                owned.Property(p => p.ExpectedDeliveryDate)
                    .HasColumnName("expected_delivery_date").IsRequired();
            });
            entity.Navigation(x => x.Period).IsRequired();

            entity.OwnsOne(x => x.ShippingCost, owned =>
            {
                owned.Property(p => p.Value)
                    .HasColumnName("shipping_cost_value").HasPrecision(18, 2).IsRequired();
                owned.Property(p => p.Currency)
                    .HasColumnName("shipping_cost_currency").IsRequired();
            });
            entity.Navigation(x => x.ShippingCost).IsRequired();

            entity.Property(x => x.Status).HasConversion<string>().IsRequired();
            entity.Property(x => x.PackageCount).IsRequired();
            entity.Property(x => x.Notes).IsRequired(false);
            entity.Property(x => x.CreatedAt);
            entity.Property(x => x.UpdatedAt);
        });
    }
}
```

**Verificación de EF:** este bloque es un ejemplo de mapping, no el resultado de una migración ejecutada. Tras correr `dotnet ef migrations add`, abre la migración generada: confirma el nombre físico `shipments`, PK `id`, GUID externo y **índice único real**. Dependiendo del proveedor y versión de EF, ajusta mapping y nombres para asegurar el resultado esperado.

### Adaptación de AppDbContext

En una **nueva solución PC2** basada en Shared, registra Shipping y conserva el interceptor de auditoría; elimina referencias a mapeos Profiles/Publishing/IAM que no existen:

```csharp
using Exam.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyShippingConfiguration();
        builder.UseSnakeCaseNamingConvention();
    }
}
```

Copia y adapta `BaseRepository.cs` y `UnitOfWork.cs` del Shared del profesor; sus contratos y firmas deben permanecer iguales. No reutilices las migraciones originales de Profiles para Shipment.

## 10. Interfaces: request y response Resources

Ruta: `Shipping/Interfaces/Rest/Resources/CreateShipmentResource.cs`

```csharp
using Exam.Platform.Shipping.Domain.Model.ValueObjects;
namespace Exam.Platform.Shipping.Interfaces.Rest.Resources;

public record CreateShipmentResource(
    Guid ShipmentIdentifier,
    Guid CarrierId,
    DateOnly PeriodStartDate,
    DateOnly PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    int PackageCount,
    ShipmentStatus? Status,
    string? Notes
);
```

Ruta: `Shipping/Interfaces/Rest/Resources/ShipmentResource.cs`

```csharp
namespace Exam.Platform.Shipping.Interfaces.Rest.Resources;

public record ShipmentResource(
    int Id,
    string ShipmentId,
    string CarrierId,
    string PeriodStartDate,
    string PeriodExpectedDeliveryDate,
    decimal ShippingCostValue,
    string ShippingCostCurrency,
    string Status,
    int PackageCount,
    string? Notes
);
```

`ShipmentResource` no expone Value Objects como tipos complejos: aplanamos al contrato JSON solicitado. El request **no** recibe `Id` autogenerado.

## 11. Interfaces: Assemblers

Ruta: `Shipping/Interfaces/Rest/Transform/CreateShipmentCommandFromResourceAssembler.cs`

```csharp
using Exam.Platform.Shipping.Domain.Model.Commands;
using Exam.Platform.Shipping.Interfaces.Rest.Resources;
namespace Exam.Platform.Shipping.Interfaces.Rest.Transform;

public static class CreateShipmentCommandFromResourceAssembler
{
    public static CreateShipmentCommand ToCommandFromResource(CreateShipmentResource resource) =>
        new(resource.ShipmentIdentifier, resource.CarrierId,
            resource.PeriodStartDate, resource.PeriodExpectedDeliveryDate,
            resource.ShippingCostValue, resource.ShippingCostCurrency,
            resource.PackageCount, resource.Status, resource.Notes);
}
```

Ruta: `Shipping/Interfaces/Rest/Transform/ShipmentResourceFromEntityAssembler.cs`

```csharp
using Exam.Platform.Shipping.Domain.Model.Aggregates;
using Exam.Platform.Shipping.Interfaces.Rest.Resources;
namespace Exam.Platform.Shipping.Interfaces.Rest.Transform;

public static class ShipmentResourceFromEntityAssembler
{
    public static ShipmentResource ToResourceFromEntity(Shipment entity) =>
        new(entity.Id, entity.ShipmentIdentifier.Value.ToString(),
            entity.CarrierId.Value.ToString(),
            entity.Period.StartDate.ToString("yyyy-MM-dd"),
            entity.Period.ExpectedDeliveryDate.ToString("yyyy-MM-dd"),
            entity.ShippingCost.Value, entity.ShippingCost.Currency,
            entity.Status.ToString(), entity.PackageCount, entity.Notes);
}
```

## 12. Interfaces: POST Controller y errores

Ruta: `Shipping/Interfaces/Rest/ShipmentsController.cs`

```csharp
using Exam.Platform.Shared.Domain.Model.Errors;
using Exam.Platform.Shipping.Application.CommandServices;
using Exam.Platform.Shipping.Interfaces.Rest.Resources;
using Exam.Platform.Shipping.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace Exam.Platform.Shipping.Interfaces.Rest;

[ApiController]
[Route("api/v1/shipments")]
[Produces("application/json")]
[Tags("Shipments")]
public class ShipmentsController(IShipmentCommandService commandService) : ControllerBase
{
    /// <summary>Creates a shipment.</summary>
    /// <remarks>Leonardo Lino</remarks>
    [HttpPost]
    [EndpointSummary("Create shipment")]
    [EndpointDescription("Create a shipment from the supplied request")]
    [ProducesResponseType(typeof(ShipmentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateShipment(
        [FromBody] CreateShipmentResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateShipmentCommandFromResourceAssembler
            .ToCommandFromResource(resource);
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
        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}
```

Este Controller cumple el **esquema** de POST → CommandService → Result → Resource. Para PC2 con i18n, no basta con los mensajes English hardcodeados del ejemplo: traduce por claves mediante `IStringLocalizer` / recursos `.resx`. También mapea `ArgumentException` / `ArgumentOutOfRangeException` a 400 en un `IExceptionHandler`. **No expongas excepciones internas 500 al cliente**.

## 13. Program.cs: composición de dependencias

Referencias reales: `Acme.Center.Platform/Program.cs` y `Acme.Center.Platform/Shared/Infrastructure/Persistence/EntityFrameworkCore/Repositories/UnitOfWork.cs`.

Ejemplo de las conexiones esenciales (integrar en el `Program.cs` de la nueva solución, **no** pegar como programa completo):

```csharp
using Exam.Platform.Shared.Domain.Repositories;
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Exam.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Exam.Platform.Shipping.Application.CommandServices;
using Exam.Platform.Shipping.Application.Internal.CommandServices;
using Exam.Platform.Shipping.Domain.Repositories;
using Exam.Platform.Shipping.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var template = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Database connection string missing.");
    options.UseMySQL(Environment.ExpandEnvironmentVariables(template));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentCommandService, ShipmentCommandService>();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

// During app pipeline setup, register exception handling / localization,
// app.MapOpenApi(), app.MapControllers(), and app.Run().
```

**No agregues `UseCors`, JWT ni módulos Publishing/IAM en una PC que no los solicita.** Para localización real, configura `AddLocalization` y `UseRequestLocalization` con `en`, `en-US`, `es`, `es-PE` y recursos adecuados.

## 14. Ejecutar MySQL, migrar, probar

1. Crea schema, usuario y permisos para `cargopulse_wa`; no pongas secretos en Git.
2. Instala paquetes EF Core, provider MySQL y herramientas de migración **compatibles** con el SDK.
3. Revisa `appsettings.json`: el código original utiliza `Environment.ExpandEnvironmentVariables`, por lo que la referencia usa `password=%DATABASE_PASSWORD%`.
4. `dotnet restore`, `dotnet build`, `dotnet ef migrations add InitialShipping`, `dotnet ef database update`.
5. `dotnet run`; revisa OpenAPI y realiza POST real con JSON del enunciado.
6. Comprueba en MySQL la tabla `shipments` y la columna PK `id`, campos `snake_case`, índice único y fechas `CreatedAt/UpdatedAt`.
7. Prueba error 409 duplicado, 400 fecha igual/menor, 400 moneda vacía, 400 monto negativo, 400 cantidad 0, 400 GUID inválido, en/es y 500 seguro.

### JSON de ejemplo

```json
{
  "shipmentIdentifier": "550e8400-e29b-41d4-a716-446655440000",
  "carrierId": "c5a2e7d1-1947-4b82-9cc7-839ab72fb2d4",
  "periodStartDate": "2026-11-20",
  "periodExpectedDeliveryDate": "2026-11-25",
  "shippingCostValue": 150.00,
  "shippingCostCurrency": "USD",
  "packageCount": 2,
  "status": "Pending",
  "notes": "Simulation"
}
```

Con `JsonStringEnumConverter` el estado puede viajar como string; sin converter `System.Text.Json` puede esperar un número. Si el status no se envía, `Status ?? Pending` fija el predeterminado. En el enunciado del simulacro, status es opcional.

## 15. Cambiar los nombres para otra PC

| Lugar | CargoPulse | Capbase | CUTS |
| --- | --- | --- | --- |
| Bounded Context | Shipping | Paperwork | Order |
| Aggregate | Shipment | Covenant | PurchaseOrder |
| POST | shipments | covenants | purchase-orders |
| Command | CreateShipmentCommand | CreateCovenantCommand | CreatePurchaseOrderCommand |
| External ID type | ShipmentIdentifier | CapbaseIdentifier | OrderIdentifier |
| Related ID type | CarrierId | PartyId | SupplierId |
| Period type | TransitPeriod | LegalityPeriod | DeliveryPeriod |
| Monetary type | MonetaryAmount | MonetaryAmount | MonetaryAmount |
| Enum/default | ShipmentStatus/Pending | CovenantStatus/Draft | PurchaseOrderStatus/Pending |

**No hagas un Replace All a ciegas:** el nombre de una propiedad puede diferir del nombre de su tipo. Ejemplo: `DocumentIdentifier (CapbaseIdentifier)` → `public CapbaseIdentifier DocumentIdentifier { get; private set; }`.

Las reglas cambian: CUTS tiene una redacción ambigua alrededor de `IsDelayed`; el periodo de Capbase es `StartDate/EndDate`; CargoPulse usa `ExpectedDeliveryDate`. Revisa siempre la literalidad del PDF.

## 16. Qué no debes confundir con la referencia

- No es lo mismo **`ProfileErrors` (objetos `Error`)** que **`ProfilesError` (enum)**: el profesor usa ambas formas en contextos distintos; no eliminar sin actualizar consumidores.
- `Profiles.ProfileId` y `Publishing.ProfileId` tienen namespaces distintos. Puede ser una decisión de modelado/ACL y **no** implica duplicación dañina.
- `GlobalExceptionHandler` moderno usa `IExceptionHandler`. La implementación antigua `GlobalExceptionHandlerMiddleware` estaba marcada obsoleta y se retiró de esta rama de estudio; un error 400 sigue requiriendo lógica específica.
- El original tiene `ApplyProfilesConfiguration()` habilitado; registrar Shipping exige una implementación propia.
- La guía original grande puede mencionar clases que no están en esta copia: verifica siempre el árbol de fuentes.
- Código escrito en una guía no equivale a código compilado, migrado y verificado.

---

**Guía y adaptación de ejemplos:** Leonardo Lino. **Patrón de código:** Learning Center Platform, material académico.  
**Siguiente acción de estudio:** reproduce la estructura en Rider, compila por capas y verifica el POST y MySQL.
