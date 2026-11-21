# GUÍA DEFINITIVA PC2 — Aplicaciones Web

**Leonardo Lino** · C# 14 · .NET 10 · ASP.NET Core · Rider · DDD + CQRS · EF Core + MySQL

> **Propósito:** una única guía para estudiar y construir una PC2 de **un endpoint POST**, conservando el estilo real de Learning Center. **No es código listo para todos los enunciados**: usa los nombres, tipos y validaciones literales de tu PDF. Las reglas de la evaluación sobre generación automática de código prevalecen.

## 1. Leer el enunciado (ANTES de Rider)

Completa esta ficha en 2–5 minutos:

| Identifica | Qué buscar | Ejemplos |
| --- | --- | --- |
| **Bounded Context** | “belongs to ... bounded context” | Paperwork / Order / Shipping |
| **Aggregate Root** | “X is an aggregate root” | Covenant / PurchaseOrder / Shipment |
| **Propiedades y tipos** | “NombrePropiedad (Tipo)” | ClientId (PartyId); DocumentIdentifier (CapbaseIdentifier) |
| **Value Objects** | GUID, periodo, dinero; con invariantes | PartyId, LegalityPeriod, MonetaryAmount |
| **Enum/default** | “Possible values…”, “default…” | Draft / Pending |
| **Unicidad** | “Must not be duplicated” | DocumentIdentifier / OrderIdentifier |
| **API** | verbo + URL + campos exactos del body | POST /api/v1/purchase-orders |
| **Persistencia** | schema, tablas, PK, migraciones | clerky_wa / cuts_wa |
| **Errores** | HTTP codes y mensajes localizados | 201 / 400 / 409 / 500; en / es |

**Regla de sintaxis:** \`ClientId (PartyId)\` → \`public PartyId ClientId { get; private set; }\`. **Tipo** entre paréntesis; **nombre de propiedad** antes. El \`Id (int)\` interno es distinto del GUID externo. El \`Id\` no se envía en el POST cuando es autogenerado.

| Caso | Contexto | Aggregate | Identifier | Relación | Periodo | POST |
| --- | --- | --- | --- | --- | --- | --- |
| Capbase | Paperwork | Covenant | CapbaseIdentifier | PartyId | LegalityPeriod | /api/v1/covenants |
| CUTS | Order | PurchaseOrder | OrderIdentifier | SupplierId | DeliveryPeriod | /api/v1/purchase-orders |
| Simulacro | Shipping | Shipment | ShipmentIdentifier | CarrierId | TransitPeriod | /api/v1/shipments |

En los PDF pueden existir contradicciones (por ejemplo, \`purchaseorder\` frente a \`purchase-orders\` o la referencia a una fecha no definida). Identifícalas; no inventes propiedades sin justificarlas.

## 2. Crear solución y compilar en Rider

**New Solution → Web → Web API**; framework \`net10.0\`, C#, **UseControllers ✓**, **UseMinimalAPIs ✗**, sin Docker ni seguridad si no los pide la PC. Nombre de solución/proyecto **exactamente** como el PDF. Elimina los archivos de ejemplo WeatherForecast.

Desde la carpeta que contiene el \`.csproj\`:

~~~powershell
dotnet --version
dotnet restore
dotnet build
~~~

Si \`dotnet add package\` dice “No se encuentra ningún proyecto”, entra en la carpeta del proyecto o pasa la ruta explícita del \`.csproj\`. Instala solo los NuGet necesarios; el \`Acme.Center.Platform.csproj\` muestra las versiones de referencia.

## 3. Estructura DDD que debes repetir

~~~text
Proyecto/
├─ Shared/
│  ├─ Application/Model/Result.cs
│  ├─ Domain/Model/{Aggregates,Events,Entities,Errors,ValueObjects}/
│  ├─ Domain/Repositories/
│  └─ Infrastructure/Persistence/EntityFrameworkCore/
├─ TU_CONTEXT/
│  ├─ Domain/
│  │  ├─ Model/{Aggregates,Commands,ValueObjects,Errors}/
│  │  └─ Repositories/
│  ├─ Application/{CommandServices,Internal/CommandServices}/
│  ├─ Infrastructure/Persistence/EntityFrameworkCore/
│  │  ├─ Configuration/Extensions/
│  │  └─ Repositories/
│  └─ Interfaces/Rest/
│     ├─ Resources/
│     ├─ Transform/
│     └─ XsController.cs
├─ Program.cs
└─ appsettings.json
~~~

**No son cuatro proyectos separados:** el ejemplo del profesor tiene los bounded contexts y sus cuatro capas dentro de un mismo proyecto ASP.NET Core.

## 4. Shared: qué copiar y qué adaptar

**Conservar firmas/lógica** de estos archivos reales del profesor (dentro de \`Acme.Center.Platform/Shared\`):

- \`Domain/Model/Aggregates/{IAggregateRoot.cs,Internal/AggregateRoot.cs}\`
- \`Domain/Model/Events/IEvent.cs\` (**Cortex.Mediator**, no MediatR)
- \`Domain/Model/Entities/IAuditableEntity.cs\` (\`CreatedAt\` y \`UpdatedAt\` son **DateTimeOffset?**)
- \`Domain/Model/Errors/Error.cs\` y \`Application/Model/Result.cs\`
- \`Domain/Repositories/{IBaseRepository.cs,IUnitOfWork.cs}\`
- \`Infrastructure/Persistence/EntityFrameworkCore/Repositories/{BaseRepository.cs,UnitOfWork.cs}\`
- \`Infrastructure/Persistence/EntityFrameworkCore/Interceptors/AuditableEntityInterceptor.cs\`

**Adaptar:** namespaces y \`using\` (\`Acme.Center.Platform\` → raíz de tu PC), versiones de paquetes, \`AppDbContext\`, conexión MySQL, mapeos EF Core, localización y el exception handler. **No copies** sus migraciones de Profiles, ni sus vínculos de IAM, JWT, CORS o Publishing si no están en el examen.

Las abstracciones \`AggregateRoot\` y \`IAuditableEntity\` **no crean automáticamente el \`int Id\` exigido por la PC**: agrégalo en tu aggregate. \`Result<T>\` soporta \`Success(value)\` y \`Failure(Error)\`; usa sus firmas reales, no inventes otro Result.

## 5. Construir las capas, en este orden

1. **Domain / ValueObjects:** los IDs externos (\`Guid Value\`), periodos (\`DateOnly StartDate\` / \`EndDate\`), monto (\`decimal Value\`, \`string Currency\`). Constructor con validaciones; métodos \`IsExpired\`, \`IsDelayed\`, \`Multiply\` si los exige el PDF. Coloca periodos y dinero en Shared **solo cuando el enunciado lo indique**.
2. **Domain / Enum y Command:** \`XStatus\` con valores/default literales y \`CreateXCommand\` como \`record\` con los datos de entrada.
3. **Domain / Aggregate:** \`X : AggregateRoot, IAuditableEntity\`; \`int Id\`, propiedades, constructor \`X(CreateXCommand command)\`, Value Objects y reglas. Auditoría exactamente \`DateTimeOffset?\`.
4. **Domain / Repository interface:** \`IXRepository : IBaseRepository<X>\`; si debe ser único, define \`ExistsByIdentifierAsync(...)\`.
5. **Application:** \`IXCommandService\` y \`XCommandService.Handle(command, cancellationToken)\`. Patrón real: consultar duplicado → \`new X(command)\` → \`repository.AddAsync(...)\` → \`unitOfWork.CompleteAsync(...)\` → \`Result<X>.Success(x)\`. Para duplicado, \`Result<X>.Failure(Error.Conflict("...","..."))\`.
6. **Infrastructure:** \`XRepository : BaseRepository<X>, IXRepository\`; \`AppDbContext\` aplica \`ApplyXConfiguration()\`; Fluent API con \`HasKey\`, \`ValueGeneratedOnAdd\`, \`OwnsOne\` / \`HasConversion\`, columnas \`snake_case\`, tablas plurales e **índice único físico**. Crear nueva migración.
7. **Interfaces:** \`CreateXResource\`, \`XResource\`, \`CreateXCommandFromResourceAssembler\`, \`XResourceFromEntityAssembler\`, \`XsController\`. POST: Resource → Command → Service → Result → Response. No pongas EF directamente en el Controller.
8. **Program.cs:** registrar DbContext, repositories, command services, controladores, OpenAPI/Scalar y, si corresponde, i18n/ProblemDetails.

**Referencias concretas en el repositorio:** \`Profiles/Application/Internal/CommandServices/ProfileCommandService.cs\` (Handle); \`Publishing/Application/Internal/CommandServices/TutorialCommandService.cs\` (duplicados); \`Profiles/Interfaces/Rest/ProfilesController.cs\` (POST); \`Profiles/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs\` (Fluent API). Rutas relativas a \`Acme.Center.Platform/\`.

### Convención de nombres

Si el Aggregate es **Shipment**: \`CreateShipmentCommand\`, \`IShipmentRepository\`, \`ShipmentRepository\`, \`IShipmentCommandService\`, \`ShipmentCommandService\`, \`CreateShipmentResource\`, \`ShipmentResource\`, \`ShipmentsController\`.

## 6. MySQL, API y errores

Configura \`ConnectionStrings:DefaultConnection\` con credenciales **fuera de Git**. Genera las migraciones **para tu modelo**, no copies las de Learning Center:

~~~powershell
dotnet tool install --global dotnet-ef --version 10.0.14
dotnet ef migrations add InitialExam --context AppDbContext
dotnet ef database update --context AppDbContext
dotnet run
~~~

Si \`dotnet-ef\` ya existe, verifica \`dotnet ef --version\`; el provider y el paquete Design deben ser compatibles. Valida el puerto real en \`launchSettings.json\` / terminal de Rider. En este proyecto de referencia, \`Program.cs\` ejecuta migraciones al iniciar y necesita MySQL disponible.

| Caso probado | Resultado esperado |
| --- | --- |
| JSON válido | **201 Created**, Resource e ID generado |
| Identificador repetido | **409 Conflict** |
| Fecha, moneda o monto inválidos | **400 Bad Request** |
| GUID o JSON malformados | **400 Bad Request** |
| Error inesperado | **500** sin filtrar secretos |
| Si se exige i18n | \`Accept-Language: es-PE / en-US\` y recursos \`.resx\` |
| Comprobación MySQL | Fila persistida, índice único y fechas auditadas |

**Ojo:** el \`GlobalExceptionHandler\` original manda muchas excepciones a **500**; adapta \`ArgumentException\` / \`ArgumentOutOfRangeException\` a **400**. \`CreatedAtAction\` requiere una ruta GET válida: si la PC pide **solo POST**, devuelve \`StatusCode(201, resource)\` y no inventes GET.

## 7. Máximo 10 commits y entrega

| Commit | Contenido |
| --- | --- |
| 1 | Proyecto + Shared |
| 2 | Value Objects y Enum |
| 3 | Command + Aggregate |
| 4 | Repository contracts |
| 5 | EF mapping + DbContext + migración |
| 6 | Repository EF + CommandService |
| 7 | Resources + Assemblers + POST Controller |
| 8 | Errores 400/409 + i18n |
| 9 | OpenAPI + comentarios XML + README |
| 10 | Correcciones y pruebas finales |

Ejecuta \`dotnet build\` frecuentemente; al final ejecuta la API, prueba el POST y verifica datos en MySQL. Evita incluir \`bin/\`, \`obj/\`, \`.idea/\` o contraseñas en el ZIP de entrega.

**Rider para acelerar:** \`Alt+Enter\` importa/corrige, \`Ctrl+Space\` completa, \`Shift+F6\` renombra, \`Ctrl+Alt+L\` formatea; \`Alt+Insert\` genera miembros según el contexto. Revisa qué herramientas de generación permite el profesor durante la evaluación.

---

**Guía de estudio y adaptación:** Leonardo Lino. **Código base y patrones:** Learning Center Platform del profesor.  
**Límite importante:** este README es una guía de resolución, **no una afirmación de que cualquier enunciado compile solo por copiar Shared**.