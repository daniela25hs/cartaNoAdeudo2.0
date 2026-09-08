# API — Esqueleto base (.NET 10)

Punto de partida limpio para APIs nuevas. La arquitectura y el flujo para agregar
entidades están en [`../docs/api/GUIA_DESARROLLO.md`](../docs/api/GUIA_DESARROLLO.md).

## Estructura

```
api/
├── carta-no-adeudo-api.slnx
├── API/                     # Controllers, Program.cs, appsettings
│   ├── API.http             # peticiones de ejemplo para probar los endpoints
│   ├── Controllers/         # HealthController (uno por agregado/entidad raíz)
│   ├── Extensions/          # ServiceExtensions (DI), StartupExtensions (migrate)
│   └── Middleware/          # GlobalExceptionHandler
├── Core/                    # sin dependencias de infraestructura
│   ├── Entities/            # BaseEntity → AuditableEntity → ActivableEntity
│   ├── Exceptions/          # BusinessException y derivadas
│   └── Interfaces/
│       ├── ICurrentUserService.cs
│       └── Repositories/    # IGenericRepository<T>, IUnitOfWork, ITransaction
└── Infrastructure/
    ├── Auth/                # CurrentUserService + integración SIGA (SigaAuthClient, SigaAppHandler)
    ├── Data/                # AppDbContext (snake_case automático) + interceptor de auditoría
    └── Repositories/        # GenericRepository<T>, UnitOfWork
```

Lo que **no** trae (a propósito): entidades de negocio, sus DTOs, servicios y
controllers. Eso lo agregas siguiendo los 9 pasos de la guía.

### Logging (Serilog)

Configurado y activo. Consola + archivo rotado diario en `logs/` con
`CompactJsonFormatter`. Ajusta niveles en la sección `Serilog` de
`appsettings.json` / `appsettings.Development.json`.

### Integración con SIGA (comentada)

El código está completo pero **desactivado**: las llamadas
`AddSigaAuth` / `AddSigaAuthProxy` y `app.UseAuthentication()` están comentadas
en `Program.cs`. La API arranca **sin autenticación** (todos los endpoints
anónimos), igual que el proyecto actual.

Para activarla:

1. Consigue un `AppKey` (GUID) real de un administrador de SIGA y las llaves
   públicas RS256 vigentes.
2. Llena la sección `Siga` de `appsettings` (de preferencia por variables de
   entorno / secret manager).
3. Descomenta en `Program.cs`: los dos `builder.Services.AddSiga*` y
   `app.UseAuthentication()`.
4. Agrega la definición de seguridad Bearer en `AddSwaggerGen`.

Detalle en [`../docs/INTEGRACION_SIGA.md`](../docs/INTEGRACION_SIGA.md). Los
controllers `AuthController` (proxy login/refresh) y `EjemploProtegidoController`
ya están listos y empiezan a funcionar en cuanto descomentas lo anterior.

## Requisitos

- .NET SDK 10
- `dotnet tool install --global dotnet-ef`
- PostgreSQL corriendo

## Primer arranque

```bash
# 1. Levanta un PostgreSQL local (ejemplo con Docker):
docker run -d --name carta-no-adeudo-postgres \
  -e POSTGRES_USER=carta_no_adeudo -e POSTGRES_PASSWORD=carta_no_adeudo -e POSTGRES_DB=carta_no_adeudo \
  -p 5432:5432 postgres:16

# 2. Ajusta la cadena de conexión en:
#    API/appsettings.Development.json  →  ConnectionStrings:Postgres
#    (para el ejemplo de arriba: Host=localhost;Port=5432;Database=carta_no_adeudo;Username=carta_no_adeudo;Password=carta_no_adeudo)

# 3. Desde api/
cd API
dotnet run
```

Al arrancar aplica migraciones automáticamente (todavía no hay ninguna) y queda
escuchando:

- Health:  <http://localhost:5080/health> → `{ "status": "ok", ... }`
- Swagger: <http://localhost:5080/swagger>

Para contenerizar la API (Dockerfile + compose), sigue el paso a paso en
[`../docs/api/DOCKERIZACION.md`](../docs/api/DOCKERIZACION.md).

### Probar los endpoints

Abre [`API/API.http`](API/API.http) en VS Code (extensión REST Client / "HTTP")
o Rider y ejecuta las peticiones. Ajusta `@host` según el puerto donde corras la
API. Los bloques de SIGA y de `categorias` están para cuando actives esas piezas.

> Al arrancar, la API aplica migraciones automáticamente. Todavía no hay ninguna:
> se crea la primera cuando agregues tu primera entidad.

## Primera entidad

```bash
cd API
dotnet ef migrations add AddCategoria --project ../Infrastructure --startup-project .
dotnet ef database update --project ../Infrastructure --startup-project .
```

Sigue [`../docs/api/GUIA_DESARROLLO.md`](../docs/api/GUIA_DESARROLLO.md) §0 para el resto
(DTOs → servicio → DI → controller).

## Convenciones (resumen — el detalle está en la guía)

- Reglas de negocio → lanzar una `BusinessException` (`ConflictException`,
  `NotFoundException`, `DependencyException`). Nunca `InvalidOperationException`
  suelta.
- CRUD genérico vía `uow.Repository<T>()`; repo específico solo si necesitas
  `Include` / proyección / SQL crudo o encapsular una regla.
- `[Authorize]` a nivel de clase por default; `[AllowAnonymous]` como excepción
  (surte efecto al activar SIGA).
- Soft-delete en `ActivableEntity` → siempre `PATCH .../{id}/toggle`.
- CORS y URLs siempre desde `appsettings` / variables de entorno.
- Agrega un proyecto de tests desde la primera entidad real, no "después".
