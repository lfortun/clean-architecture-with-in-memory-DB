# VinManagement.CleanArch

Vehicle Identification Number (VIN) management Web API built on **Clean Architecture** with **ASP.NET Core 8** and **Entity Framework Core In-Memory** database.

## Solution Structure

```
├── Domain/                   Entities, interfaces, domain logic
├── Application/              Use cases, DTOs, service interfaces
├── Infrastructure/           Persistence, repositories, validators
├── VinManagement.CleanArch/  Web API (controllers, DI, startup)
└── VinManagement.CleanArch.Tests/  Unit tests (xUnit + Moq)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Postman](https://www.postman.com/downloads/) (optional, for API testing)

## Getting Started

### Run the API

```bash
dotnet run --project VinManagement.CleanArch/VinManagement.CleanArch
```

The API starts at:
- HTTPS: `https://localhost:7139`
- HTTP: `http://localhost:5072`

Swagger UI is available at `https://localhost:7139/swagger` in Development mode.

### Run Tests

```bash
dotnet test VinManagement.CleanArch/VinManagement.CleanArch.Tests/VinManagement.CleanArch.Tests.csproj
```

### Build

```bash
dotnet build VinManagement.CleanArch/VinManagement.CleanArch.sln
```

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login and receive a JWT token |

### VIN Management (requires authentication)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/vins` | Create a new VIN record |
| GET | `/api/vins` | Get all VIN records |
| GET | `/api/vins/{id}` | Get a VIN record by ID |
| PUT | `/api/vins/{id}` | Update a VIN record |
| DELETE | `/api/vins/{id}` | Delete a VIN record |

## Authentication

All VIN endpoints require a valid JWT token. Authenticate via:

```bash
curl -X POST https://localhost:7139/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'
```

Response:
```json
{
  "token": "eyJhbGci...",
  "tokenType": "Bearer",
  "expiresAt": "2026-05-07T12:00:00Z"
}
```

Then use the token in subsequent requests:
```bash
curl -H "Authorization: Bearer <token>" https://localhost:7139/api/vins
```

**Default credentials:**

| Username | Password | Role |
|----------|----------|------|
| admin | admin123 | Admin |
| user | user123 | User |

## Postman Collection

Import `VinManagement.CleanArch.postman_collection.json` into Postman. The collection auto-extracts the JWT token from the login response and applies it to all VIN endpoints. Run the **Login** request first, then run any VIN endpoint.

> **Note:** When using HTTPS, Postman may show an SSL certificate warning. You can disable SSL verification in Postman Settings > General > "SSL certificate verification" if needed for local testing.

## Configuration

### appsettings.json

```json
{
  "VinValidators": {
    "DataOne": {
      "ProviderName": "DataOne API",
      "Enabled": true
    }
  },
  "JwtSettings": {
    "Secret": "<your-secret-key-at-least-32-chars>",
    "Issuer": "VinManagement.CleanArch",
    "Audience": "VinManagement.CleanArch",
    "ExpiryMinutes": 60
  }
}
```

> **Important:** Replace the `Secret` with a strong, randomly generated key (min 32 characters) before deploying to production.

## VIN Validator Pipeline

VIN codes are validated on **Create** and **Update** operations through a pluggable validator pipeline. Implement `IVinValidator` in the Domain layer, add the implementation in Infrastructure, and register it in DI. All registered validators run sequentially.

## DTO Validation

Request DTOs are validated using **FluentValidation**. Validators live in the Web API project's `Validators/` folder. Invalid requests automatically return `400 Bad Request` with detailed error messages.

## Architecture Rules

- **Domain** depends on nothing
- **Application** depends on Domain only
- **Infrastructure** depends on Application + Domain
- **Web API** depends on Application + Infrastructure

No circular dependencies allowed.
