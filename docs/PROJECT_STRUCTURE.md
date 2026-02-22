# Complete Project Structure

```
Personal Expense Tracker/
│
├── README.md                          # Project overview
├── .gitignore                         # Git ignore rules
├── docker-compose.yml                 # Multi-service orchestration
│
├── docs/                              # Documentation
│   ├── SETUP.md                       # Local development guide
│   ├── ARCHITECTURE.md                # System architecture & best practices
│   ├── API.md                         # API documentation
│   └── CHECKLIST.md                   # Implementation checklist
│
├── backend/                           # .NET 8 Web API
│   ├── ExpenseTracker.sln            # Solution file
│   ├── Dockerfile                     # Backend container
│   ├── README.md                      # Backend documentation
│   │
│   └── src/
│       │
│       ├── ExpenseTracker.Domain/    # Domain Layer (Entities)
│       │   ├── Entities/
│       │   │   ├── User.cs
│       │   │   ├── Expense.cs
│       │   │   └── Category.cs
│       │   ├── Interfaces/
│       │   │   └── IRepositories.cs
│       │   └── ExpenseTracker.Domain.csproj
│       │
│       ├── ExpenseTracker.Application/  # Application Layer (Business Logic)
│       │   ├── DTOs/
│       │   │   └── CommonDTOs.cs
│       │   ├── Interfaces/
│       │   │   └── IServices.cs
│       │   ├── Services/
│       │   │   ├── AuthService.cs
│       │   │   ├── ExpenseService.cs
│       │   │   └── CategoryService.cs
│       │   └── ExpenseTracker.Application.csproj
│       │
│       ├── ExpenseTracker.Infrastructure/  # Infrastructure Layer
│       │   ├── Data/
│       │   │   └── ApplicationDbContext.cs
│       │   ├── Repositories/
│       │   │   └── Repositories.cs
│       │   ├── Services/
│       │   │   ├── JwtService.cs
│       │   │   └── AiService.cs
│       │   └── ExpenseTracker.Infrastructure.csproj
│       │
│       └── ExpenseTracker.API/        # API Layer (Controllers)
│           ├── Controllers/
│           │   ├── AuthController.cs
│           │   ├── ExpensesController.cs
│           │   └── CategoriesController.cs
│           ├── Middleware/
│           │   └── ExceptionMiddleware.cs
│           ├── Program.cs
│           ├── appsettings.json
│           └── ExpenseTracker.API.csproj
│
├── frontend/                          # Next.js 14 Frontend
│   ├── app/
│   │   ├── (auth)/                   # Auth route group
│   │   │   ├── login/
│   │   │   │   └── page.tsx
│   │   │   └── register/
│   │   │       └── page.tsx
│   │   │
│   │   ├── (dashboard)/              # Dashboard route group
│   │   │   ├── layout.tsx            # Dashboard layout with nav
│   │   │   ├── dashboard/
│   │   │   │   └── page.tsx          # Main dashboard
│   │   │   └── expenses/
│   │   │       └── page.tsx          # Expense management
│   │   │
│   │   ├── api/
│   │   │   └── auth/
│   │   │       └── [...nextauth]/
│   │   │           └── route.ts      # NextAuth config
│   │   │
│   │   ├── layout.tsx                # Root layout
│   │   ├── page.tsx                  # Home page (redirects)
│   │   ├── providers.tsx             # React Query & NextAuth providers
│   │   └── globals.css               # Global styles
│   │
│   ├── components/
│   │   ├── ui/                       # Reusable UI components
│   │   └── dashboard/                # Dashboard components
│   │
│   ├── hooks/
│   │   └── useExpenses.ts            # React Query hooks
│   │
│   ├── lib/
│   │   └── api/
│   │       ├── client.ts             # Axios client
│   │       └── services.ts           # API functions
│   │
│   ├── package.json
│   ├── tsconfig.json
│   ├── tailwind.config.js
│   ├── next.config.js
│   ├── Dockerfile
│   ├── .env.example
│   └── README.md
│
└── ai-service/                        # Python FastAPI Microservice
    ├── app/
    │   ├── __init__.py
    │   ├── main.py                    # FastAPI app
    │   │
    │   ├── models/
    │   │   └── schemas.py             # Pydantic models
    │   │
    │   ├── services/
    │   │   └── ai_service.py          # AI business logic
    │   │
    │   └── routers/
    │       └── ai_router.py           # API endpoints
    │
    ├── requirements.txt
    ├── Dockerfile
    └── README.md
```

## Layer Dependencies

```
┌─────────────────────────────────────────────────────────┐
│                      API Layer                          │
│  - Controllers                                          │
│  - Middleware                                           │
│  - Dependency Injection                                 │
└────────────────────┬────────────────────────────────────┘
                     │ depends on
                     ▼
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                      │
│  - DbContext                                            │
│  - Repositories                                         │
│  - External Services (AI, JWT)                          │
└────────────────────┬────────────────────────────────────┘
                     │ depends on
                     ▼
┌─────────────────────────────────────────────────────────┐
│                Application Layer                         │
│  - Business Logic                                       │
│  - DTOs                                                 │
│  - Service Interfaces                                   │
└────────────────────┬────────────────────────────────────┘
                     │ depends on
                     ▼
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                          │
│  - Entities (User, Expense, Category)                   │
│  - Domain Interfaces                                    │
│  - No external dependencies                             │
└─────────────────────────────────────────────────────────┘
```

## Data Flow

### Creating an Expense

```
User (Browser)
    │
    │ 1. Submit form
    ▼
Next.js Frontend
    │
    │ 2. POST /api/expenses (with JWT)
    ▼
.NET API Controller
    │
    │ 3. Validate JWT
    │ 4. Call ExpenseService
    ▼
Application Service
    │
    │ 5. Business logic
    │ 6. Call Repository
    ▼
Infrastructure Repository
    │
    │ 7. EF Core query
    ▼
SQL Server Database
    │
    │ 8. Return saved entity
    ▼
Response flows back up
    │
    ▼
User sees new expense
```

### Getting AI Insights

```
User requests dashboard
    │
    ▼
Next.js calls .NET API
    │
    ▼
.NET ExpenseService
    │
    │ 1. Get expenses from DB
    │ 2. Calculate totals
    │ 3. Call Python AI Service
    ▼
Python AI Service
    │
    │ 4. Generate summary
    │ 5. Generate suggestions
    ▼
Return to .NET
    │
    ▼
Return to Next.js
    │
    ▼
Display on dashboard
```

## Technology Stack Summary

### Backend (.NET 8)
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server 2022
- **Authentication**: JWT Bearer
- **Password Hashing**: BCrypt.Net
- **API Documentation**: Swagger/OpenAPI
- **Architecture**: Clean Architecture

### Frontend (Next.js 14)
- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **Authentication**: NextAuth.js
- **State Management**: React Query (TanStack Query)
- **HTTP Client**: Axios
- **Charts**: Recharts
- **Forms**: React Hook Form + Zod

### AI Service (Python)
- **Framework**: FastAPI
- **Validation**: Pydantic
- **Server**: Uvicorn
- **Future**: OpenAI/Anthropic/AWS Bedrock integration ready

### DevOps
- **Containerization**: Docker
- **Orchestration**: Docker Compose
- **Database**: SQL Server (Docker)
- **Cache**: Redis (Optional)

## Port Mapping

| Service          | Port | URL                          |
|------------------|------|------------------------------|
| Frontend         | 3000 | http://localhost:3000        |
| Backend API      | 5000 | http://localhost:5000        |
| Swagger UI       | 5000 | http://localhost:5000/swagger|
| AI Service       | 8000 | http://localhost:8000        |
| FastAPI Docs     | 8000 | http://localhost:8000/docs   |
| SQL Server       | 1433 | localhost:1433               |
| Redis (Optional) | 6379 | localhost:6379               |

## File Count Summary

- **Backend**: ~20 C# files
- **Frontend**: ~15 TypeScript/TSX files
- **AI Service**: ~5 Python files
- **Docker**: 4 Dockerfiles + 1 docker-compose.yml
- **Documentation**: 5 markdown files
- **Configuration**: ~10 config files

## Total Lines of Code (Approximate)

- **Backend**: ~1,500 lines
- **Frontend**: ~1,000 lines
- **AI Service**: ~200 lines
- **Documentation**: ~2,000 lines
- **Total**: ~4,700 lines

## Key Design Patterns Used

1. **Repository Pattern**: Data access abstraction
2. **Dependency Injection**: Loose coupling
3. **Service Layer Pattern**: Business logic separation
4. **DTO Pattern**: Data transfer objects
5. **Middleware Pattern**: Cross-cutting concerns
6. **Factory Pattern**: Object creation (React Query)
7. **Observer Pattern**: React state management
8. **Strategy Pattern**: AI service (ready for multiple providers)

## Security Features

- ✅ JWT authentication
- ✅ Password hashing (BCrypt)
- ✅ CORS configuration
- ✅ SQL injection prevention (EF Core)
- ✅ XSS prevention (React escaping)
- ✅ Environment-based secrets
- ✅ HTTPS ready
- ✅ Input validation

## Scalability Features

- ✅ Stateless API design
- ✅ Microservices architecture
- ✅ Docker containerization
- ✅ Database connection pooling
- ✅ Redis cache ready
- ✅ Horizontal scaling ready
- ✅ Load balancer ready

## Production Readiness

- ✅ Global exception handling
- ✅ Structured logging ready
- ✅ Health check endpoints
- ✅ Environment configuration
- ✅ Docker deployment
- ✅ API documentation
- ✅ Clean architecture
- ✅ Separation of concerns
