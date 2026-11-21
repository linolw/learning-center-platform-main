# Learning Center Platform — guía práctica PC2

**Autor de esta guía y adaptación académica:** Leonardo Lino  
**Repositorio base:** proyecto Learning Center utilizado como referencia de arquitectura en el curso Aplicaciones Web.  
**Stack de la referencia:** .NET 10, C# 14, ASP.NET Core, Entity Framework Core, MySQL, Cortex.Mediator y OpenAPI/Scalar.

> **Objetivo:** utilizar el código real del profesor como referencia para aprender a resolver enunciados nuevos de PC2. No es una solución universal que se copie sin adaptar: el enunciado siempre determina nombres, reglas, contrato HTTP, almacenamiento y alcance.
>
> **Integridad académica:** los enunciados de referencia permiten sugerencias de *Code Completion* de JetBrains Rider, pero prohíben usar agentes de IA para generar código durante la evaluación. Este material está preparado para estudio y simulacros fuera de la evaluación.

## Empieza por aquí

| Recurso | Para qué sirve |
| --- | --- |
| **[Guía PC2 paso a paso](docs/pc2-guia-paso-a-paso.md)** | Interpretación del enunciado, Rider, Shared, DDD, persistencia, REST, errores, MySQL, pruebas y commits |
| **[Arquitectura completa de referencia](docs/pc2-arquitectura-referencia.md)** | Árbol de archivos y fragmentos adaptables por capas para un POST de Shipment |
| **[Guía original extensa](docs/study-guide.md)** | Estudiar los patrones y conceptos presentes en la referencia |
| **[ADR de referencia](docs/adrs.md)** | Decisiones arquitectónicas del proyecto original |

**Importante sobre la guía original:** algunos apartados de `docs/study-guide.md` describen más funciones de las que están implementadas en esta copia. Comprueba los archivos reales antes de asumir que existen controladores, Docker Compose, servicios de IAM o endpoints concretos.

## ¿Qué hay realmente en este repositorio?

- `Acme.Center.Platform/Shared/`: núcleo reutilizable (`AggregateRoot`, `IEvent`, `IAuditableEntity`, `Result<T>`, `Error`, `IBaseRepository`, `IUnitOfWork`), persistencia EF Core, interceptores, rutas, localización y manejo de excepciones.
- `Acme.Center.Platform/Profiles/`: un ejemplo rastreable de **Command → CommandService → Repository → Resource/Assembler → Controller**.
- `Acme.Center.Platform/Publishing/`: ejemplos adicionales de entidades, Value Objects, consultas, comandos, eventos y validación de duplicados; su configuración EF está deshabilitada en `AppDbContext` de esta copia.
- `Acme.Center.Platform/Iam/`: únicamente los archivos que existen realmente en esta versión; no presupongas un IAM funcional por aparecer en la guía teórica.
- `Acme.Center.Platform/Migrations/`: migraciones del proyecto de referencia, **no** de tu futura práctica.

No mezcles automáticamente bounded contexts ni elimines identificadores con el mismo nombre que pertenecen a contextos distintos (por ejemplo, `Profiles.ProfileId` frente a `Publishing.ProfileId`).

## 1. Preparar Rider desde cero (para una PC nueva)

1. **Rider → New Solution → Web → Web API.** Elige `.NET 10`, `C#`, `UseControllers` activado, `UseMinimalAPIs` desactivado. Sin Docker, CORS ni IAM salvo que el enunciado los pida.
2. Nombra **solución y proyecto exactamente como indique el PDF**. Ejemplo de simulacro: solución `pc212999u202422298`, proyecto `PC212999.U202422298.CargoPulse.Platform`.
3. Abre la terminal en la carpeta donde está el `.csproj`:

```powershell
dotnet --info
dotnet restore
dotnet build
```

4. Borra los ejemplos `WeatherForecast.cs` y `WeatherForecastController.cs` si no pertenecen al caso.
5. Reproduce **el patrón de carpetas** de la guía de arquitectura. Usa solo los bounded contexts que pida la práctica.

## 2. Interpretar el enunciado antes de programar

Completa esta ficha en papel o en un borrador:

| Pregunta | Ejemplo ficticio |
| --- | --- |
| ¿Qué se registra? | Shipment |
| ¿Qué dice el profesor que es aggregate root? | Shipment |
| ¿A qué bounded context pertenece? | Shipping |
| ¿Qué método y ruta pide? | POST `api/v1/shipments` |
| ¿Qué atributos, tipos, obligatoriedad y defaults contiene? | GUID, periodo, dinero, estado, cantidad |
| ¿Qué Value Objects y enums exige? | ShipmentIdentifier, CarrierId, TransitPeriod, MonetaryAmount, ShipmentStatus |
| ¿Qué debe ser único? | ShipmentIdentifier |
| ¿Cuál es el nombre de MySQL/schema? | cargopulse_wa |
| ¿Qué devuelve el POST? | HTTP 201 y resource con Id generado |
| ¿Qué errores y traducciones exige? | 400, 409, 500; inglés/español |

La secuencia usual de creación es:

```text
HTTP JSON
 → CreateXResource → CreateXCommandFromResourceAssembler
 → CreateXCommand → XCommandService.Handle(...)
 → X (Aggregate Root + Value Objects) → IXRepository / XRepository
 → IUnitOfWork → AppDbContext / MySQL
 → XResourceFromEntityAssembler → HTTP 201
```

## 3. Lo que puedes conservar de Shared

**Reutiliza la lógica original**, revisando `using`, namespaces, dependencias y compatibilidad:

- `Shared/Domain/Model/Aggregates/Internal/AggregateRoot.cs`
- `Shared/Domain/Model/Aggregates/IAggregateRoot.cs`
- `Shared/Domain/Model/Events/IEvent.cs` (requiere **Cortex.Mediator**, no MediatR)
- `Shared/Domain/Model/Entities/IAuditableEntity.cs` (**DateTimeOffset?**, no DateTime)
- `Shared/Domain/Model/Errors/Error.cs`
- `Shared/Application/Model/Result.cs` (firmas reales `Success(T)`, `Failure(Error)`)
- `Shared/Domain/Repositories/IBaseRepository.cs` y `IUnitOfWork.cs`

Después adapta conscientemente `AppDbContext`, interceptores, `BaseRepository`, `UnitOfWork`, localización y ProblemDetails. **No copies toda la configuración original del proyecto como si fuera independiente del dominio.** Consulta [la guía detallada](docs/pc2-guia-paso-a-paso.md).

## 4. Iniciar este repositorio de referencia en local

```powershell
git clone https://github.com/linolw/learning-center-platform-main.git
cd learning-center-platform-main
dotnet restore learning-center-platform.sln
dotnet build learning-center-platform.sln
```

La aplicación de referencia requiere MySQL. Hay una conexión configurada en `appsettings*.json` y el programa ejecuta migraciones al iniciar, por lo que **un build exitoso no implica que `dotnet run` pueda conectarse a la base de datos**.

**Nunca subas contraseñas reales.** Esta rama reemplaza las contraseñas literales por el marcador `%DATABASE_PASSWORD%`. Para ejecución local en PowerShell:

```powershell
$env:DATABASE_PASSWORD = "TU_CLAVE_LOCAL"
dotnet run --project .\Acme.Center.Platform\Acme.Center.Platform.csproj
```

Configura además host, usuario y nombre de schema válidos en tu entorno. En una PC nueva utiliza **su propia base y sus propias migraciones**. Si una contraseña estuvo publicada en Git, cámbiala en la base de datos: modificar el archivo no limpia el historial remoto.

## 5. Qué debes programar en una PC de solo POST

1. **Domain:** Value Objects validados, enum, `CreateXCommand`, `X : AggregateRoot, IAuditableEntity`, errores e `IXRepository`.
2. **Application:** `IXCommandService` y `XCommandService` (consultar duplicados, crear el aggregate, `AddAsync`, `CompleteAsync`, devolver `Result<X>`).
3. **Infrastructure:** `AppDbContext`, mapeo Fluent API, índice único y repositorio EF Core.
4. **Interfaces:** `CreateXResource`, `XResource`, assemblers, `XsController`, documentación OpenAPI.
5. **Program:** registrar `DbContext`, repositorios, servicios, localización, ProblemDetails, controladores y documentación sin funcionalidades no exigidas.
6. **Verificar:** POST válido 201; duplicado 409; fechas/montos/cantidad inválidos 400; recurso correcto; persistencia en MySQL; mensaje en/es si se solicita.

**No uses `CreatedAtAction` apuntando a un GET inexistente:** para una práctica que exige exclusivamente POST, puedes devolver `StatusCode(201, resource)`.

## 6. Distribución recomendada en un máximo de 10 commits

| # | Alcance |
| --- | --- |
| 1 | Solution, `.csproj`, paquetes mínimos, estructura y Shared base |
| 2 | Value Objects + enums del caso |
| 3 | Command + Aggregate Root + reglas |
| 4 | Repository interface + EF Core mappings |
| 5 | `AppDbContext`, Repository EF + migración |
| 6 | CommandService + unidad de trabajo |
| 7 | Resources + Assemblers + Controller |
| 8 | Errores, localización y ProblemDetails |
| 9 | OpenAPI, XML comments y README |
| 10 | Ajustes finales tras compilar y probar |

Realiza `dotnet build` entre etapas. Los commits representan avances verificables; no hagas diez commits vacíos para llenar la lista.

## 7. Referencias principales de código real

- Servicio de comando: `Acme.Center.Platform/Profiles/Application/Internal/CommandServices/ProfileCommandService.cs`
- POST Controller: `Acme.Center.Platform/Profiles/Interfaces/Rest/ProfilesController.cs`
- Assembler: `Acme.Center.Platform/Profiles/Interfaces/Rest/Transform/CreateProfileCommandFromResourceAssembler.cs`
- Mapeo de Value Objects: `Acme.Center.Platform/Profiles/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs`
- Validación de duplicados: `Acme.Center.Platform/Publishing/Application/Internal/CommandServices/TutorialCommandService.cs`
- Arranque y DI: `Acme.Center.Platform/Program.cs`

## 8. Estado de la adaptación y limitaciones

Esta rama corrige archivos importados que no son código fuente, quita una implementación obsoleta duplicada de excepción, protege las configuraciones de contraseña y agrega documentación práctica. **La aplicación de ejemplo sigue siendo Learning Center**; los fragmentos de PC2 en `docs/` son plantillas didácticas, **no** una API nueva compilada y desplegada. Es obligatorio adaptar y verificar su código en cada enunciado.

---

**Documentación y adaptación de estudio:** Leonardo Lino.  
**Base técnica:** Learning Center Platform (material académico de referencia). Se preserva la atribución de la implementación original.
