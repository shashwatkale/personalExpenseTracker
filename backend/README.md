# .NET Backend - Clean Architecture

## Project Structure

```
backend/
├── ExpenseTracker.sln
├── src/
│   ├── ExpenseTracker.Domain/          # Entities, Enums, Interfaces
│   ├── ExpenseTracker.Application/     # Business Logic, DTOs, Services
│   ├── ExpenseTracker.Infrastructure/  # EF Core, Repositories, External Services
│   └── ExpenseTracker.API/             # Controllers, Middleware, Startup
└── tests/
    ├── ExpenseTracker.UnitTests/
    └── ExpenseTracker.IntegrationTests/
```

## Layers

### Domain Layer
- Pure business entities
- No dependencies on other layers
- Domain interfaces

### Application Layer
- Use cases / business logic
- DTOs and mapping
- Service interfaces
- Validation rules

### Infrastructure Layer
- EF Core DbContext
- Repository implementations
- External API clients (Python AI)
- Identity/Auth implementation

### API Layer
- Controllers
- Middleware (Exception, Auth)
- Dependency injection setup
- API configuration

## Dependencies Flow

API → Infrastructure → Application → Domain
