# Shipping PC2 integrado en Acme.Center.Platform

**Documentación:** Leonardo Lino.  
**Estado:** código fuente integrado en la rama `feature/pc2-shipping-reference`. Todavía requiere restaurar/compilar con .NET 10, ejecutar una migración nueva y comprobar MySQL. No afirmamos que un POST funcione hasta hacer esas comprobaciones.

## Por qué existe Shipping dentro del proyecto de referencia

La estructura `Shared` de Learning Center es reutilizable, pero no constituye una aplicación PC2 por sí sola. Para mostrar cómo se usa **realmente** añadimos un bounded context `Shipping` en `Acme.Center.Platform`, que reutiliza:
- `Shared.Domain.Model.Aggregates.Internal.AggregateRoot` (eventos),
- `Shared.Domain.Model.Entities.IAuditableEntity` (fechas `DateTimeOffset?`),
- `Shared.Application.Model.Result<T>` y `Shared.Domain.Model.Errors.Error`,
- `Shared.Domain.Repositories.IBaseRepository` y `IUnitOfWork`,
- `Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories.BaseRepository` / `UnitOfWork`,
- `AppDbContext`, interceptores de auditoría y modelo Fluent API.

Este ejemplo **mantiene** el resto del proyecto (Profiles y Publishing). Para una PC nueva, copia **solo** los bounded contexts exigidos; no es recomendable llevar IAM, CORS ni los módulos del ejemplo si están fuera del alcance.

## Archivos añadidos / adaptados

| Capa | Ubicación y responsabilidad |
| --- | --- |
| Domain | `Shipping/Domain/Model/Aggregates/Shipment.cs` — aggregate root |
| Domain | `Shipping/Domain/Model/Commands/CreateShipmentCommand.cs` — comando `record` |
| Domain | `Shipping/Domain/Model/ValueObjects/` — identificadores, enum |
| Shared | `Shared/Domain/Model/ValueObjects/` — `TransitPeriod`, `MonetaryAmount` |
| Domain | `Shipping/Domain/Repositories/IShipmentRepository.cs` y `Model/Errors/ShipmentErrors.cs` |
| Application | `Shipping/Application/CommandServices/IShipmentCommandService.cs` e `Internal/CommandServices/ShipmentCommandService.cs` |
| Infrastructure | `Shipping/Infrastructure/Persistence/EntityFrameworkCore/` — Repository + Fluent API |
| Interfaces | `Shipping/Interfaces/Rest/` — Resources, assemblers y `ShipmentsController.cs` |
| i18n / errores | `Resources/Shipping/ShippingMessages*.resx` y `ShippingExceptionHandler.cs` |
| Composición | `Program.cs` y `Shared/.../AppDbContext.cs` actualizados |
| Design time | `Shared/.../AppDbContextFactory.cs` para `dotnet ef` |

## Pasos precisos para ejecutar en Rider (Windows, PowerShell)

1. Cambia a la rama de ejemplo en tu clon:

```powershell
git fetch origin
git switch feature/pc2-shipping-reference
cd .\Acme.Center.Platform
```

2. Confirma .NET 10, restaura dependencias y compila:

```powershell
dotnet --version
dotnet restore
dotnet build
```

3. **Prepara MySQL** y la base indicada en `ConnectionStrings:DefaultConnection`. Para un ejercicio independiente debería llamarse `cargopulse_wa`; para ejecutar el repositorio de referencia, puedes continuar usando la base que declara `appsettings.json` (el nuevo modelo se añadirá a esa base). Evita credenciales reales en Git.

```powershell
$env:DATABASE_PASSWORD = "TU_CLAVE_LOCAL"
```

4. Genera y aplica una **nueva migración** (el historial del profesor no incluye la tabla `shipments`):

```powershell
dotnet tool install --global dotnet-ef --version 10.0.14
dotnet ef migrations add AddShippingPc2 --context AppDbContext
dotnet ef database update --context AppDbContext
```

Si `dotnet-ef` ya está instalado, no repitas la instalación; consulta `dotnet ef --version`. Si tienes dudas sobre versiones, utiliza las compatibles con los paquetes de `Acme.Center.Platform.csproj`. El factory permite crear `AppDbContext` a tiempo de diseño sin depender del arranque web.

5. Inicia la aplicación (la configuración de referencia puede necesitar MySQL operativo, porque ejecuta `Database.Migrate()`):

```powershell
dotnet run
```

6. Abre la documentación Scalar en la URL que muestre Rider (habitualmente `/scalar/v1`), ubica el endpoint `POST /api/v1/shipments` y envía:

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
  "notes": "PC2 practice"
}
```

7. Comprueba:
   - HTTP `201`, con `id` numérico.
   - Repite el mismo identificador: HTTP `409`.
   - Cambia fecha final a fecha inicial: HTTP `400`.
   - Cambia monto a negativo, moneda vacía o cantidad 0: HTTP `400`.
   - Header `Accept-Language: es-PE`: textos traducidos en respuestas controladas por Shipping.
   - Verifica en MySQL la tabla `shipments`, el índice único y las columnas de auditoría.

**Importante:** cambiar un enum por un valor inválido o enviar JSON malformado activa validación automática de ASP.NET Core. Ese comportamiento no utiliza necesariamente los recursos personalizados de Shipping y puede requerir configuración adicional si el examen exige traducir *todas* las respuestas de validación.

## ¿Qué cambiar en otras PC?

- **Capbase:** `Shipping` → `Paperwork`, `Shipment` → `Covenant`, IDs `CapbaseIdentifier` / `PartyId`, periodo `LegalityPeriod`, estado `Draft`, ruta `api/v1/covenants`, schema `clerky_wa`.
- **CUTS:** `Shipping` → `Order`, `Shipment` → `PurchaseOrder`, IDs `OrderIdentifier` / `SupplierId`, periodo `DeliveryPeriod`, estado `Pending`, ruta `api/v1/purchase-orders`, schema `cuts_wa`.
- **Otros casos:** extrae nombres y reglas literales del enunciado; nunca renombres campos solo por similitud.

Para arquitectura general y resolución completa, consulta [README](../README.md), [guía detallada](pc2-guia-paso-a-paso.md) y [blueprint de archivos](pc2-arquitectura-referencia.md).

> Esta rama es material de **estudio**. Los enunciados proporcionados prohíben usar agentes de generación de código durante la evaluación; respeta las reglas del docente.
