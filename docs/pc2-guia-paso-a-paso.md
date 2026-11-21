# PC2 de Aplicaciones Web — guía de ejecución paso a paso

**Preparado por:** Leonardo Lino  
**Alcance:** prácticas de **una operación POST** con .NET 10, C# 14, ASP.NET Core, EF Core, MySQL, DDD y CQRS, siguiendo el estilo observado en Learning Center. Esta guía es para **estudiar y practicar**; no reemplaza el enunciado real.

## 0. Cómo aprovechar el proyecto del profesor

El proyecto original tiene capas **Domain**, **Application**, **Infrastructure** e **Interfaces** dentro de cada bounded context. El Shared contiene contratos, resultados, errores, persistencia común y middleware. No existe un único Shared que compile en cualquier PC simplemente pegándolo: su configuración depende de la versión de NuGet, de los namespaces y de los contextos activos.

**Referencias de verdad en la copia actual:**

| Responsabilidad | Archivo real relativo a `Acme.Center.Platform/` |
| --- | --- |
| Aggregate y eventos | `Shared/Domain/Model/Aggregates/Internal/AggregateRoot.cs` |
| Contrato de auditoría | `Shared/Domain/Model/Entities/IAuditableEntity.cs` |
| Error tipado | `Shared/Domain/Model/Errors/Error.cs` |
| Resultado tipado | `Shared/Application/Model/Result.cs` |
| Repository base / Unit of Work | `Shared/Domain/Repositories/IBaseRepository.cs`; `Shared/Domain/Repositories/IUnitOfWork.cs` |
| Ejemplo CommandService | `Profiles/Application/Internal/CommandServices/ProfileCommandService.cs` |
| Duplicados y errores | `Publishing/Application/Internal/CommandServices/TutorialCommandService.cs` |
| Convertir JSON a Command | `Profiles/Interfaces/Rest/Transform/CreateProfileCommandFromResourceAssembler.cs` |
| Resource de respuesta | `Profiles/Interfaces/Rest/Transform/ProfileResourceFromEntityAssembler.cs` |
| POST Controller | `Profiles/Interfaces/Rest/ProfilesController.cs` |
| Mapeo EF Core | `Profiles/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs` |
| Generalización de snake_case | `Shared/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs` |
| Auditoría real | `Shared/Infrastructure/Persistence/EntityFrameworkCore/Interceptors/AuditableEntityInterceptor.cs` |

**Advertencia:** `Program.cs` de esta copia registra Profiles. Los mapeos Publishing e IAM se encuentran comentados en `AppDbContext.cs`. No concluyas que están activos solo porque existen carpetas.

## 1. Los primeros 10 minutos: interpretar el PDF

Subraya frases clave:
- **Aggregate Root:** “X es un aggregate root” → archivo `X.cs` en `Y/Domain/Model/Aggregates/`. En Capbase es `Covenant`, en CUTS `PurchaseOrder`, en el simulacro `Shipment`.
- **Bounded context:** “pertenece a Y” → directorio principal `Y/`; son `Paperwork`, `Order` y `Shipping`, respectivamente. No se deduce siempre del plural del aggregate.
- **Atributos:** “ClientId (PartyId)” → **nombre de propiedad** `ClientId`, **tipo** `PartyId`; la sintaxis es `public PartyId ClientId { get; private set; }`.
- **Identificador externo:** si dice GUID/UUID de otro contexto, crea el Value Object especificado (`CapbaseIdentifier`, `OrderIdentifier`, `ShipmentIdentifier`). No conviertas todo automáticamente a `string`.
- **ID interno:** `Id (int), autogenerado` → propiedad e índice PK en Fluent API, **NO** forma parte del request.
- **Value Objects:** intervalo de fechas, monto y moneda, ID tipado: encapsulan **invariantes**.
- **Enum:** copia los nombres y defaults exactos; no inventes otros valores.
- **Endpoint:** copia la ruta literal. Para clases compuestas se suele usar plural kebab-case (`purchase-orders`).
- **Errores:** “no duplicados” → query `ExistsBy…Async` + índice único + HTTP 409; “fecha inválida” → validación de dominio y HTTP 400.
- **Respuesta:** anota exactamente los nombres, formatos y tipos pedidos, incluso si no son iguales a las propiedades internas del aggregate.
- **Infraestructura:** provider, schema, puertos, localización, XML docs, README, ZIP de entrega.

### Plantilla de extracción

```text
Nombre de solución y proyecto:
Stack y paquetes exigidos:
Bounded Context:
Aggregate Root:
Campos:  NombrePropiedad (Tipo) [obligatorio / opcional]
Value Objects y reglas:
Enum y estado inicial:
Campo único:
Ruta HTTP / verbo:
Request JSON:
Response JSON / status:
Schema MySQL:
Locales requeridos:
Exclusiones: security / CORS / tests / etc.
```

### Casos comparados

| Concepto | Capbase | CUTS | CargoPulse (simulacro) |
| --- | --- | --- | --- |
| Contexto | Paperwork | Order | Shipping |
| Aggregate | Covenant | PurchaseOrder | Shipment |
| Comando | CreateCovenantCommand | CreatePurchaseOrderCommand | CreateShipmentCommand |
| Identificador externo | CapbaseIdentifier | OrderIdentifier | ShipmentIdentifier |
| Relación externa | PartyId | SupplierId | CarrierId |
| Intervalo | LegalityPeriod | DeliveryPeriod | TransitPeriod |
| Dinero | MonetaryAmount | MonetaryAmount | MonetaryAmount |
| Default | Draft | Pending | Pending |
| Tabla sugerida | covenants | purchase_orders | shipments |
| Endpoint | `POST /api/v1/covenants` | `POST /api/v1/purchase-orders` | `POST /api/v1/shipments` |
| Schema | clerky_wa | cuts_wa | cargopulse_wa |

**No olvides ambigüedades del PDF:** la PC de CUTS menciona tanto `purchase-orders` como `purchaseorder`; también define `DeliveryPeriod` con dos fechas y luego hace referencia a `EndDate`. Anota estas contradicciones y justifica explícitamente las decisiones; no inventes atributos en silencio.

## 2. Iniciar solución y proyecto en Rider

**Rider → New Solution → Web → Web API**: framework `net10.0`, C#, controllers habilitados, Minimal APIs desactivado. Para examenes con límite de tiempo conviene confirmar compilación antes de añadir paquetes:

```powershell
dotnet --version
dotnet restore
dotnet build
```

Si estás parado en la carpeta de la solución, `dotnet add package` puede responder “No se encuentra ningún proyecto”. Entra a la carpeta del `.csproj`, o indica la ruta del proyecto explícitamente:

```powershell
dotnet add .\NombreDelProyecto\NombreDelProyecto.csproj package Cortex.Mediator --version 3.1.2
```

**No añadas todo el NuGet original por reflejo:** usa los paquetes que pide la implementación. Las versiones de referencia en `Acme.Center.Platform.csproj` incluyen `Microsoft.EntityFrameworkCore 10.0.14`, `MySql.EntityFrameworkCore 10.0.12`, `Microsoft.AspNetCore.OpenApi 10.0.14`, `Scalar.AspNetCore 2.1.2` y `Cortex.Mediator 3.1.2`. Verifica compatibilidad del provider y versiones de tu SDK. Los paquetes de JWT, BCrypt y seguridad pueden ser innecesarios.

## 3. Crear estructura DDD (no una carpeta Controllers global)

Consulta el árbol ampliado de [arquitectura de referencia](pc2-arquitectura-referencia.md).

```text
Project/
├── XContext/
│   ├── Domain/
│   │   ├── Model/{Aggregates,Commands,ValueObjects,Errors}
│   │   └── Repositories/
│   ├── Application/{CommandServices,Internal/CommandServices}
│   ├── Infrastructure/Persistence/EntityFrameworkCore/
│   │   ├── Configuration/Extensions/
│   │   └── Repositories/
│   └── Interfaces/Rest/{Resources,Transform}
├── Shared/{Application,Domain,Infrastructure,Interfaces,Resources}
├── Program.cs
└── appsettings.json
```

En C#, `namespace` no obliga a usar exactamente esa carpeta física, pero **mantenerlos alineados** hace fácil localizar responsabilidades.

## 4. Preparar Shared SIN reinventarlo

Conserva del profesor las firmas de `Result<T>`, `Error`, `IBaseRepository`, `IUnitOfWork`, `AggregateRoot`, `IAggregateRoot`, `IEvent`, `IAuditableEntity` y `IEntityId<T>`.

**Dependencias indispensables a revisar:**
- `IEvent : INotification` necesita `using Cortex.Mediator.Notifications` y `Cortex.Mediator`. No es MediatR.
- `AggregateRoot` utiliza `IEvent` y `IAggregateRoot`; resuelve los `using` y el namespace al copiar.
- `IAuditableEntity` usa `DateTimeOffset? CreatedAt` y `DateTimeOffset? UpdatedAt`; el interceptor los actualiza en EF.
- `Result<T>.Failure(Error)` es una firma real; `Error.Conflict(code, message)` y `Error.Validation(code, message)` construyen errores tipados.
- `BaseRepository<T>` requiere `AppDbContext`, Entity Framework Core y los métodos de `IBaseRepository`.
- `AppDbContext` **no** puede copiarse sin revisar `ApplyProfilesConfiguration()`; registra solo los modelos del examen.

**Cambio masivo de namespaces:** en Rider, `Ctrl + Shift + R`, selecciona el alcance correcto, busca `Acme.Center.Platform` y reemplaza por el namespace raíz de la PC. Revisa los alias y recursos (por ejemplo `Acme.Center.Platform.Resources.Errors`) por separado. Preserva comportamiento y documentación de clases comunes.

## 5. Construir Value Objects, Enum y Aggregate

Orden:
1. Value Objects de ID: por ejemplo `ShipmentIdentifier(Guid Value)`; valida `Guid.Empty` si corresponde.
2. Intervalo: `StartDate` y `ExpectedDeliveryDate` de tipo `DateOnly`, con constructor que rechaza fechas inválidas y método `IsOverdue`.
3. Dinero: `MonetaryAmount(decimal Value, string Currency)`, rechaza negativos y moneda nula/vacía; `Multiply(decimal factor)` retorna otra instancia, conservando la moneda.
4. Enum: `Pending, InTransit, Delivered, Cancelled`, con `Pending` inicial según el simulacro.
5. Comando inmutable `CreateXCommand` (normalmente `record`), con datos primitivos del request.
6. `X : AggregateRoot, IAuditableEntity`, con constructor `X(CreateXCommand command)` que crea los Value Objects y protege reglas. En el ejemplo del profesor, las clases `Profile` y `Tutorial` implementan construcción desde Command.
7. Si el profesor exige `int Id` autogenerado, agrégalo explícitamente al aggregate: **el `AggregateRoot` de Shared no contiene un Id genérico**.

## 6. Repository y consulta de unicidad

```csharp
public interface IShipmentRepository : IBaseRepository<Shipment>
{
    Task<bool> ExistsByIdentifierAsync(
        Guid identifier, CancellationToken cancellationToken);
}
```

El contrato va en `Shipping/Domain/Repositories`. La implementación concreta va en `Shipping/Infrastructure/Persistence/EntityFrameworkCore/Repositories` e inyecta el `AppDbContext` como `BaseRepository<Shipment>`.

La comprobación en el service evita errores previsibles; **el índice único en MySQL** protege contra solicitudes concurrentes. Si se produce una carrera de concurrencia, captura la excepción de persistencia y transforma el caso de unicidad a HTTP 409 según el proveedor.

## 7. CommandService con el patrón REAL del profesor

En `ProfileCommandService` se hace:

```csharp
var profile = new Profile(command);
await profileRepository.AddAsync(profile, cancellationToken);
await unitOfWork.CompleteAsync(cancellationToken);
return Result<Profile>.Success(profile);
```

En una PC con unicidad, añade **antes** del `new X(command)` la consulta al repositorio. Para error:

```csharp
return Result<Shipment>.Failure(
    Error.Conflict("Shipping.DuplicateShipmentIdentifier",
                   "A shipment with this identifier already exists."));
```

El Controller no consulta EF directamente y el Service no construye respuestas HTTP. `Result<T>` transporta éxito/error hacia Interfaces.

## 8. EF Core: Fluent API, MySQL, auditoría

En el proyecto nuevo:
- `DbContext` recibe `DbContextOptions` y `OnModelCreating` invoca `builder.ApplyShippingConfiguration()`; debes crear esa extensión, no existe de antemano.
- `HasKey(s => s.Id)`; `ValueGeneratedOnAdd()`; `ToTable("shipments")`; columnas snake_case.
- Mapea los Value Objects como owned types (`OwnsOne`) o converters (`HasConversion`), según tipo y requisitos; **no confíes en mapeo automático**.
- Define índice único sobre el GUID externo en la tabla física.
- Registra `IAuditableEntity` mediante el interceptor del Shared, evitando duplicar fecha actual en cada CommandService.
- Crea una **nueva migración** para el nuevo agregado; no copies las migraciones de Profiles.

Desde la carpeta del `.csproj`, tras instalar `dotnet-ef` y paquete Design compatibles:

```powershell
dotnet ef migrations add InitialShipping
dotnet ef database update
```

Si faltan tool/paquete `dotnet ef`, instala la herramienta de EF compatible con .NET 10; si la conexión falla, revisa schema, usuario, contraseña, servidor y permisos antes de cambiar C#.

## 9. Resource, Assembler, Controller

Los archivos van en `Shipping/Interfaces/Rest`:
- `Resources/CreateShipmentResource.cs`: datos JSON que el usuario manda, **sin Id generado**.
- `Transform/CreateShipmentCommandFromResourceAssembler.cs`: traducción de resource a command.
- `Resources/ShipmentResource.cs`: respuesta exacta del PDF.
- `Transform/ShipmentResourceFromEntityAssembler.cs`: aplana Value Objects y convierte fechas a `yyyy-MM-dd` cuando el contrato pide strings.
- `ShipmentsController.cs`: `[ApiController]`, `[Route("api/v1/shipments")]`, `[HttpPost]`, `[ProducesResponseType]` y llamadas a assemblers/service.

**Solo POST:** `CreatedAtAction(nameof(GetById), …)` falla si no existe GET. Si el enunciado no exige GET, usa `StatusCode(StatusCodes.Status201Created, resource)` o `Created(uri, resource)` con una URI válida, sin añadir GET extra.

## 10. Errores HTTP y localización

Distingue tres niveles:

| Escenario | Dónde se maneja | HTTP deseado |
| --- | --- | --- |
| JSON inválido, campos obligatorios | ASP.NET model binding / validación | 400 |
| Fechas, monto o cantidad violan invariantes | Constructor/Value Object; ExceptionHandler | 400 |
| Identificador ya existe | CommandService + Repository + `Error.Conflict` | 409 |
| Error de conexión o fallo imprevisto | Global exception handler | 500 |

`Shared/Infrastructure/Pipeline/Middleware/Components/GlobalExceptionHandler.cs` del profesor **no transforma automáticamente `ArgumentException` en 400**: adapta el mapeo si la práctica lo exige.

Para en/es, añade recursos `.resx` con claves estables por idioma, `AddLocalization`, `UseRequestLocalization` y culturas compatibles con `Accept-Language` (`en`, `en-US`, `es`, `es-PE`). No uses mensajes hardcodeados como única fuente si el PDF requiere traducción.

Regla de seguridad: nunca devuelvas datos de conexión, stack traces ni contraseñas como detalle del 500.

## 11. Registrar dependencias en Program.cs

```csharp
builder.Services.AddControllers();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentCommandService, ShipmentCommandService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// AddDbContext<AppDbContext> + MySQL provider + localización + errors + OpenAPI.
// Para una PC sin IAM/CORS, NO agregues UseCors ni JWT.
```

Valida además que `app.MapControllers()` aparezca antes de `app.Run()` y que la documentación OpenAPI se exponga como exige el profesor.

## 12. Probar de verdad (no solo Build Success)

Para un POST de Shipment:

```powershell
$body = @{
  shipmentIdentifier = "550e8400-e29b-41d4-a716-446655440000"
  carrierId = "c5a2e7d1-1947-4b82-9cc7-839ab72fb2d4"
  periodStartDate = "2026-11-20"
  periodExpectedDeliveryDate = "2026-11-25"
  shippingCostValue = 150.00
  shippingCostCurrency = "USD"
  packageCount = 2
  notes = "Simulation"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5191/api/v1/shipments" -Method Post -ContentType "application/json" -Body $body
```

Cambia puerto y JSON a los de tu aplicación. **Esta llamada es un ejemplo, no un endpoint verificado en este repositorio**.

Matriz manual:
1. Válido → **201**, Id generado, JSON correcto.
2. Mismo identificador otra vez → **409**.
3. Fecha final igual/menor → **400**.
4. Monto negativo → **400**.
5. Moneda en blanco → **400**.
6. Cantidad `0` → **400** si la regla del caso lo exige.
7. `Accept-Language: es-PE` y `en-US` → mensajes correctos si hay i18n.
8. `SELECT * FROM shipments;` en MySQL → fila persistida, PK y marcas de auditoría.

La comprobación del POST y la verificación de persistencia deben hacerse incluso si la rúbrica excluye crear una suite automatizada de tests.

## 13. Diagnóstico rápido de los fallos frecuentes

| Síntoma | Causa probable | Comprobación |
| --- | --- | --- |
| `IEvent` / `INotification` rojo | Falta Cortex.Mediator/using | `.csproj`, import, restore |
| `dotnet add package` no encuentra proyecto | Terminal en solución, no proyecto | `cd` a carpeta `.csproj` |
| Error por `IAggregateRoot` | Namespace o archivo faltante | `using`, ruta, inclusión en proyecto |
| `Result<T>.Failure` no acepta argumentos | Se copió otra variante de Result | Firma real en Shared |
| `DateTime` no implementa auditoría | Interfaz usa `DateTimeOffset?` | Igualar tipos y propiedades |
| 500 cuando se espera 400 | Handler genérico sin `ArgumentException` | Error mapping |
| 409 no ocurre | No se consulta el identificador ni hay índice único | Repo / migration |
| No aparece tabla | Mapeo omitido / migración no aplicada | `ApplyXConfiguration`, `dotnet ef` |
| 404 tras POST | Ruta/controller no registrado | `MapControllers`, kebab-case |
| Fallo 201 con `CreatedAtAction` | No hay GET de destino | `StatusCode(201, resource)` |
| `dotnet build` correcto, `dotnet run` falla | Migración automática / credenciales DB | `Program.cs`, MySQL |

## 14. Rider para ganar velocidad

- `Alt + Enter`: quick fixes e imports.
- `Alt + Insert`: generación estructural en una clase (según función y configuración).
- `Ctrl + Space`: autocompletado.
- `Shift + F6`: refactor Rename.
- `Ctrl + Alt + L`: reformat.
- `Ctrl + Shift + R`: replace, **verificando alcance**.

El material de las PC proporcionadas admite sugerencias de **Code Completion**, pero prohíbe **AI Agents for Code Generation** durante la evaluación; no asumas que todas las funciones de generación automática o Live Templates están permitidas. Practícalas fuera del examen y confirma restricciones con el docente.

## 15. Finalizar y entregar

- `dotnet restore` y `dotnet build` sin errores.
- Verificar que `dotnet run` inicia con MySQL disponible.
- Crear los casos válidos/invalidos manualmente y revisar MySQL.
- README propio (en inglés si lo pide el enunciado), XML documentation comments y `<remarks>Leonardo Lino</remarks> en el **código nuevo del simulacro** donde corresponda.
- Evitar `bin/`, `obj/`, `.idea/`, secretos, `.env` en el ZIP.
- Renombrar ZIP y solución conforme a NRC y código. **Este repositorio es referencia de estudio**, no la solución que debes subir como PC.

## 16. Orden sugerido de 10 commits

```text
1. chore: initialize solution and shared
2. feat: implement domain value objects
3. feat: implement aggregate and create command
4. feat: define repository contracts
5. feat: map EF Core and migrations
6. feat: implement persistence and command service
7. feat: expose POST controller and resources
8. feat: handle validation conflicts and localization
9. docs: document OpenAPI and README
10. fix: verify edge cases and delivery
```

No hagas un commit si el proyecto queda deliberadamente roto. Cambia el orden si hace falta para mantener compilación incremental.

---

**Documentación de estudio:** Leonardo Lino. **Implementación de referencia:** Learning Center Platform.
