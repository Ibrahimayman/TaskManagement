# Task Management API

A production-ready ASP.NET Core Web API built on **.NET 9** following **Clean Architecture** principles.

## Tech Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 9 |
| Web | ASP.NET Core Web API (Controllers) |
| Architecture | Clean Architecture + Vertical Slices |
| ORM | Entity Framework Core 9 |
| Database | SQL Server / LocalDB |
| Authentication | JWT Bearer |
| Password Hashing | BCrypt.Net-Next |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| Documentation | Swagger / OpenAPI (Swashbuckle) |
| Error Handling | Global `IExceptionHandler` + ProblemDetails |

## Solution Layout

```text
TaskManagement/
├── src/
│   ├── TaskManagement.Domain/             ← Entities, base classes, enums (no dependencies)
│   ├── TaskManagement.Application/        ← CQRS handlers, DTOs, validators, behaviors, ApiResponse
│   ├── TaskManagement.Infrastructure/     ← JWT, password hasher, current user, time provider
│   ├── TaskManagement.Persistence/        ← EF Core DbContext, configurations, interceptors
│   └── TaskManagement.API/                ← Controllers, middleware, Program.cs, Swagger
├── tests/
├── TaskManagement.slnx
├── NuGet.config
└── README.md
```

### Dependency Flow

```text
API ──► Application ──► Domain
API ──► Infrastructure ──► Application ──► Domain
API ──► Persistence    ──► Application ──► Domain
```

Domain has zero project references. All other layers depend on abstractions defined in `Application/Common/Interfaces`.

## Getting Started

### Prerequisites

- .NET 9 SDK (or newer)
- SQL Server LocalDB (default), or any SQL Server instance
- (Optional) `dotnet-ef` CLI: `dotnet tool install --global dotnet-ef`

### 1. Clone and restore

```bash
cd TaskManagement
dotnet restore
```

### 2. Configure connection string and JWT secret

Edit `src/TaskManagement.API/appsettings.json` and replace the `Secret` value with a cryptographically strong key (>=32 chars). For production, store secrets in environment variables, Azure Key Vault, or User Secrets.

```bash
cd src/TaskManagement.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Secret" "<your-strong-secret>"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

### 3. Create the database (first migration)

```bash
dotnet ef migrations add InitialCreate \
  --project src/TaskManagement.Persistence \
  --startup-project src/TaskManagement.API \
  --output-dir Migrations

dotnet ef database update \
  --project src/TaskManagement.Persistence \
  --startup-project src/TaskManagement.API
```

### 4. Run the API

```bash
dotnet run --project src/TaskManagement.API
```

Open Swagger at the root URL (e.g. `https://localhost:7xxx/`).

## Endpoints

| Method | Route | Auth |
|---|---|---|
| `POST` | `/api/v1/auth/register` | Anonymous |
| `POST` | `/api/v1/auth/login` | Anonymous |
| `GET`  | `/api/v1/projects` | JWT |
| `GET`  | `/api/v1/projects/{id}` | JWT (owner) |
| `POST` | `/api/v1/projects` | JWT |
| `PUT`  | `/api/v1/projects/{id}` | JWT (owner) |
| `DELETE` | `/api/v1/projects/{id}` | JWT (owner) |
| `GET`  | `/api/v1/tasks/by-project/{projectId}` | JWT (owner) |
| `POST` | `/api/v1/tasks` | JWT (owner) |
| `PATCH` | `/api/v1/tasks/{id}/status` | JWT (owner) |
| `DELETE` | `/api/v1/tasks/{id}` | JWT (owner) |

All responses are wrapped in a generic `ApiResponse<T>`:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Request completed successfully.",
  "data": { /* payload */ },
  "errors": []
}
```

## Architecture Highlights

- **CQRS via MediatR** — every use case is a `Command` or `Query` with a single handler.
- **Pipeline behaviors** — `UnhandledExceptionBehavior` → `LoggingBehavior` → `ValidationBehavior` → `PerformanceBehavior` run for every request automatically.
- **FluentValidation** — validators are auto-discovered from the Application assembly and run inside the MediatR pipeline.
- **Global exception handling** — `IExceptionHandler` (.NET 8+) maps domain/application exceptions to the proper HTTP status code with a uniform `ApiResponse` payload.
- **Auditing & soft delete** — `AuditableEntity` + `AuditableEntityInterceptor` automatically populate `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`, and convert deletes into `IsDeleted = true`. Global query filters hide soft-deleted rows.
- **JWT authentication** — `ITokenService` abstraction in Application; HMAC-SHA256 implementation in Infrastructure with configurable issuer/audience/lifetime.
- **Ownership checks** — every project/task handler verifies `OwnerId == ICurrentUserService.UserId` before mutating data.
- **No repository pattern** — `IApplicationDbContext` is injected into handlers directly; EF Core's `DbContext` already provides Unit of Work semantics.

## Folder Structure (per project)

```text
TaskManagement.Domain/
  Common/        BaseEntity, AuditableEntity
  Entities/      User, Project, TaskItem
  Enums/         TaskItemStatus
  Exceptions/    DomainException

TaskManagement.Application/
  Common/
    Behaviors/   Validation, Logging, Performance, UnhandledException
    Exceptions/  ValidationException, NotFoundException, ForbiddenAccessException, ConflictException
    Interfaces/  IApplicationDbContext, ICurrentUserService, ITokenService, IPasswordHasher, IDateTimeProvider
    Models/      ApiResponse<T>, PaginatedList<T>
  Features/
    Authentication/Commands/{Register,Login}
    Projects/Commands/{CreateProject,UpdateProject,DeleteProject}
    Projects/Queries/{GetProjectById,GetProjectsList}
    Tasks/Commands/{CreateTask,UpdateTaskStatus,DeleteTask}
    Tasks/Queries/GetTasksByProject
  DependencyInjection.cs

TaskManagement.Infrastructure/
  Identity/      JwtSettings, TokenService
  Services/      PasswordHasher, CurrentUserService, DateTimeProvider
  DependencyInjection.cs

TaskManagement.Persistence/
  Configurations/   UserConfiguration, ProjectConfiguration, TaskItemConfiguration
  Contexts/         ApplicationDbContext
  Interceptors/     AuditableEntityInterceptor
  DependencyInjection.cs

TaskManagement.API/
  Controllers/      AuthController, ProjectsController, TasksController
  Middleware/       GlobalExceptionHandler
  Extensions/       SwaggerExtensions
  Program.cs
  appsettings.json
```

## Key NuGet Packages

| Layer | Package | Version |
|---|---|---|
| Application | `MediatR` | 12.4.1 |
| Application | `FluentValidation` | 11.10.0 |
| Application | `FluentValidation.DependencyInjectionExtensions` | 11.10.0 |
| Application | `Microsoft.EntityFrameworkCore` | 9.0.0 |
| Infrastructure | `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.0 |
| Infrastructure | `System.IdentityModel.Tokens.Jwt` | 8.2.0 |
| Infrastructure | `BCrypt.Net-Next` | 4.0.3 |
| Persistence | `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.0 |
| Persistence | `Microsoft.EntityFrameworkCore.Design` | 9.0.0 |
| Persistence | `Microsoft.EntityFrameworkCore.Tools` | 9.0.0 |
| API | `Swashbuckle.AspNetCore` | 7.0.0 |

## Best Practices Applied

1. **Clean Architecture** with strict dependency direction (Domain ← Application ← Infrastructure/Persistence ← API).
2. **Vertical slices** under `Features/` — each use case keeps its command/query, validator, and handler together.
3. **Generic `ApiResponse<T>` wrapper** — every endpoint returns a uniform shape (success, status, message, data, errors).
4. **Global exception handling** via `IExceptionHandler` — no try/catch in controllers.
5. **MediatR pipeline behaviors** for validation, logging, performance, and unhandled exception cross-cutting concerns.
6. **`async`/`await` end-to-end** with `CancellationToken` propagation.
7. **`AsNoTracking` projections** in queries — never expose entities, always project to DTOs.
8. **Soft delete + audit** via EF Core interceptor.
9. **Ownership-based authorization** in every handler that mutates data.
10. **Strongly-typed configuration** with the Options pattern (`JwtSettings`).

## Next Steps

- Add unit tests (`tests/TaskManagement.Application.UnitTests`) for command/query handlers.
- Add integration tests with `WebApplicationFactory` and `Testcontainers`.
- Add refresh tokens (extend `AuthResponse` and `ITokenService`).
- Add API versioning (`Asp.Versioning.Mvc`).
- Add Serilog and structured logging.
- Add Health Checks and Rate Limiting (built into .NET 9).
- Move JWT secret to Azure Key Vault / User Secrets in production.
