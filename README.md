# Personal Expense Tracker - Production Architecture

## Architecture Overview

Microservices-based expense tracking system with AI-powered insights.

### Services

- **Frontend**: Next.js 14 (Port 3000)
- **Backend API**: .NET 8 Web API (Port 5000)
- **AI Service**: Python FastAPI (Port 8000)
- **Database**: SQL Server (Port 1433)
- **Cache**: Redis (Port 6379) - Optional

## Quick Start

### Prerequisites
- Node.js 18+
- .NET 8 SDK
- Python 3.11+
- Docker & Docker Compose
- SQL Server

### Local Development

```bash
# Start all services with Docker
docker-compose up -d

# Or run individually:

# 1. Backend API
cd backend/ExpenseTracker.API
dotnet restore
dotnet run

# 2. AI Service
cd ai-service
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000

# 3. Frontend
cd frontend
npm install
npm run dev
```

### Environment Setup

Copy `.env.example` to `.env` in each service directory and configure.

## Project Structure

```
├── backend/              # .NET 8 Clean Architecture
├── frontend/             # Next.js 14 App Router
├── ai-service/           # Python FastAPI
├── docker-compose.yml    # Orchestration
└── docs/                 # Architecture docs
```

## API Endpoints

### .NET API (Port 5000)
- POST `/api/auth/register`
- POST `/api/auth/login`
- GET/POST/PUT/DELETE `/api/expenses`
- GET `/api/expenses/summary`
- GET `/api/categories`

### Python AI (Port 8000)
- POST `/ai/summary`
- POST `/ai/suggestions`

## Authentication Flow

1. User authenticates via NextAuth
2. JWT token issued by .NET API
3. Token stored in NextAuth session
4. All requests include Bearer token
5. .NET validates JWT on each request

## Development Guidelines

- Follow Clean Architecture principles
- Use TypeScript strict mode
- Implement proper error handling
- Write unit tests for business logic
- Use dependency injection
- Follow REST conventions

## Production Considerations

- Enable HTTPS
- Configure CORS properly
- Use connection pooling
- Implement rate limiting
- Setup logging (Serilog/.NET, Winston/Node)
- Monitor with Application Insights or similar
- Use secrets management (Azure Key Vault, AWS Secrets Manager)
