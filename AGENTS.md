# AGENTS.md — Guidelines for AI Agents Working on VinManagement.CleanArch

## Project Overview
Vehicle Identification Number (VIN) management Web API built on **Clean Architecture** with **ASP.NET Core 8** and **Entity Framework Core In-Memory** database.

## Solution Structure
```
clean-architecture-with-in-memory-DB/
├── AGENTS.md                     ← This file
├── README.md
└── VinManagement.CleanArch/
    ├── VinManagement.CleanArch.sln
    ├── Domain/                   ← Entities, interfaces, domain logic
    ├── Application/              ← Use cases, DTOs, service interfaces
    ├── Infrastructure/           ← Persistence, repos, external services
    ├── VinManagement.CleanArch/  ← Web API (controllers, DI, startup)
    └── VinManagement.CleanArch.Tests/ ← Unit tests (xUnit + Moq)
        ├── Application/Services/   ← Service layer tests
        └── Controllers/            ← Controller tests
```

## Layer Rules (Dependency Direction)
- **Domain** ← depends on NOTHING
- **Application** ← depends on Domain only
- **Infrastructure** ← depends on Application + Domain
- **Web API** ← depends on Application + Infrastructure

NEVER break this direction. No circular dependencies.

## Coding Conventions

### General
- C# 12, .NET 8, nullable reference types enabled, implicit usings enabled
- **NO** emoji in code or comments
- Follow existing naming conventions and file structure
- Use `record` for DTOs where appropriate
- Prefer immutable types; use `init` properties

### Naming
- Classes/Interfaces: PascalCase (`VinService`, `IVinRepository`)
- Methods: PascalCase (`CreateAsync`, `GetByIdAsync`)
- Local variables: camelCase (`vinRequest`, `entity`)
- Interface prefix: `I` (`IVinService`, `IAppDbContext`)
- Private fields: `_camelCase` (`_context`, `_repository`)

### Error Handling
- Domain-specific exceptions in `Domain/Common/` (e.g., `NotFoundException`, `BadRequestException`)
- Use `try/catch` with specific exception types, not bare `Exception`
- Controllers return appropriate HTTP status codes (200, 201, 400, 404, 500)

### Async Pattern
- All I/O methods MUST be async with `*Async` suffix
- Use `async/await`, not `.Result` or `.Wait()`

## Authentication

JWT Bearer authentication is configured:
- `IJwtTokenGenerator` interface in Domain, `JwtTokenGenerator` in Infrastructure
- `ICredentialValidator` interface in Domain, `DefaultCredentialValidator` in Infrastructure
- `IAuthService` in Application, `AuthService` in Application (uses both interfaces)
- `AuthController` in Web API exposes `POST /api/auth/login`
- Protected controllers use `[Authorize]` attribute

## CRUD Feature Pattern

When adding a new feature (e.g., "Vehicle"), follow this structure:

### Domain Layer
1. Entity in `Domain/Entities/Vehicle.cs` (id, properties, timestamps)
2. Repository interface in `Domain/Repositories/IVehicleRepository.cs`

### Application Layer
1. DTOs in `Application/DTOs/` (e.g., `CreateVehicleDto.cs`, `VehicleResponseDto.cs`)
2. Service interface in `Application/Services/IVehicleService.cs`
3. Service implementation in `Application/Services/VehicleService.cs`

### Infrastructure Layer
1. Repository implementation in `Infrastructure/Repositories/VehicleRepository.cs`
2. Register service + repository in `Infrastructure/DependencyInjection/`

### Web API Layer
1. Controller in `VinManagement.CleanArch/Controllers/VehiclesController.cs`
2. Register services in `Program.cs`

## Adding New Projects
```bash
cd VinManagement.CleanArch
dotnet new classlib -n Domain -o Domain
dotnet new classlib -n Application -o Application
dotnet new classlib -n Infrastructure -o Infrastructure
dotnet sln add Domain
dotnet sln add Application
dotnet sln add Infrastructure
```

## Common Packages
- `Microsoft.EntityFrameworkCore.InMemory` (Infrastructure)
- `Swashbuckle.AspNetCore` (Web API)
- `FluentValidation.AspNetCore` (Web API)

## DTO Validation

DTO validation uses **FluentValidation** in the Web API layer:
- Validators live in `VinManagement.CleanArch/Validators/`
- Use `AbstractValidator<T>` for each DTO
- Register with `AddFluentValidationAutoValidation()` and `AddValidatorsFromAssemblyContaining<Program>()` in `Program.cs`
- Do NOT use `System.ComponentModel.DataAnnotations` on DTOs

## Testing

### Framework
- xUnit for unit testing
- Moq for mocking dependencies

### Test Structure
- Service tests in `Tests/Application/Services/` — mock repositories, test business logic
- Controller tests in `Tests/Controllers/` — mock services, test HTTP responses

### Test Naming
`MethodName_Scenario_ExpectedBehavior` (e.g., `CreateAsync_ValidVin_ReturnsResponseDto`)

### Adding Tests for New Features
1. Create `{EntityName}ServiceTests.cs` in `Tests/Application/Services/`
2. Create `{EntityName}ControllerTests.cs` in `Tests/Controllers/`
3. Mock the dependencies (repositories for services, services for controllers)
4. Test happy path + error scenarios (not found, bad request, etc.)

## Build / Test Commands
```bash
# Build
dotnet build VinManagement.CleanArch/VinManagement.CleanArch.sln

# Run tests
dotnet test VinManagement.CleanArch/VinManagement.CleanArch.Tests/VinManagement.CleanArch.Tests.csproj

# Run
dotnet run --project VinManagement.CleanArch/VinManagement.CleanArch

# Clean + rebuild
dotnet clean VinManagement.CleanArch/VinManagement.CleanArch.sln
dotnet build VinManagement.CleanArch/VinManagement.CleanArch.sln
```

## Key Principles
1. Entities live in Domain, NOT in Infrastructure or Web API
2. Controllers should be thin — delegate to services
3. In-memory DB is configured via EF Core options in Infrastructure
4. Swagger is enabled for Development environment only
5. Keep methods focused; avoid god classes
