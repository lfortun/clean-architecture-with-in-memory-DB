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

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/vins` | Create a new VIN record |
| GET | `/api/vins` | Get all VIN records |
| GET | `/api/vins/{id}` | Get a VIN record by ID |
| PUT | `/api/vins/{id}` | Update a VIN record |
| DELETE | `/api/vins/{id}` | Delete a VIN record |

## Postman Collection

Import `VinManagement.CleanArch.postman_collection.json` into Postman to quickly test all endpoints. The collection is pre-configured with `https://localhost:7139` as the base URL.

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
  }
}
```

## VIN Validator Pipeline

VIN codes are validated on **Create** and **Update** operations through a pluggable validator pipeline. Implement `IVinValidator` in the Domain layer, add the implementation in Infrastructure, and register it in DI. All registered validators run sequentially.

## Architecture Rules

- **Domain** depends on nothing
- **Application** depends on Domain only
- **Infrastructure** depends on Application + Domain
- **Web API** depends on Application + Infrastructure

No circular dependencies allowed.
