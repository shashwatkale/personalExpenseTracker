# Local Development Setup Guide

## Prerequisites

### Required Software
- **.NET 8 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+**: [Download](https://nodejs.org/)
- **Python 3.11+**: [Download](https://www.python.org/downloads/)
- **SQL Server**: 
  - Windows: SQL Server Express
  - Mac/Linux: Docker (`docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`)
- **Docker Desktop** (optional but recommended)

### Recommended Tools
- **Visual Studio 2022** or **VS Code** with C# extension
- **Postman** or **Insomnia** for API testing
- **Azure Data Studio** or **SQL Server Management Studio**

## Step-by-Step Setup

### 1. Clone Repository
```bash
git clone <repository-url>
cd Personal\ Expense\ Tracker
```

### 2. Setup SQL Server Database

#### Option A: Using Docker
```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 \
  --name expense-tracker-db \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

#### Option B: Local SQL Server
- Ensure SQL Server is running
- Update connection string in `backend/src/ExpenseTracker.API/appsettings.json`

### 3. Setup .NET Backend

```bash
cd backend

# Restore dependencies
dotnet restore

# Apply database migrations
cd src/ExpenseTracker.API
dotnet ef database update

# If migrations don't exist, create them:
dotnet ef migrations add InitialCreate --project ../ExpenseTracker.Infrastructure --startup-project .

# Run the API
dotnet run
```

API will be available at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### 4. Setup Python AI Service

```bash
cd ai-service

# Create virtual environment
python -m venv venv

# Activate virtual environment
# On Mac/Linux:
source venv/bin/activate
# On Windows:
venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Run the service
uvicorn app.main:app --reload --port 8000
```

AI Service will be available at: `http://localhost:8000`
API Docs: `http://localhost:8000/docs`

### 5. Setup Next.js Frontend

```bash
cd frontend

# Install dependencies
npm install

# Create environment file
cp .env.example .env.local

# Update .env.local with:
# NEXT_PUBLIC_API_URL=http://localhost:5000/api
# NEXTAUTH_URL=http://localhost:3000
# NEXTAUTH_SECRET=generate-with-openssl-rand-base64-32

# Run development server
npm run dev
```

Frontend will be available at: `http://localhost:3000`

## Environment Configuration

### Backend (.NET)
File: `backend/src/ExpenseTracker.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ExpenseTrackerDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ExpenseTrackerAPI",
    "Audience": "ExpenseTrackerClient"
  },
  "AiService": {
    "BaseUrl": "http://localhost:8000"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:3000"
  }
}
```

### Frontend (Next.js)
File: `frontend/.env.local`

```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
NEXTAUTH_URL=http://localhost:3000
NEXTAUTH_SECRET=your-generated-secret-here
```

Generate secret:
```bash
openssl rand -base64 32
```

### AI Service (Python)
File: `ai-service/.env` (optional)

```env
ENVIRONMENT=development
LOG_LEVEL=info
```

## Testing the Setup

### 1. Test Backend API
```bash
# Health check
curl http://localhost:5000/health

# Register a user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### 2. Test AI Service
```bash
curl http://localhost:8000/health

curl -X POST http://localhost:8000/ai/summary \
  -H "Content-Type: application/json" \
  -d '{
    "user_id": "test-user",
    "total_amount": 1500.00,
    "category_breakdown": {
      "Food": 500.00,
      "Transport": 300.00
    }
  }'
```

### 3. Test Frontend
- Navigate to `http://localhost:3000`
- Register a new account
- Login
- Add an expense
- View dashboard

## Database Migrations

### Create New Migration
```bash
cd backend/src/ExpenseTracker.API

dotnet ef migrations add MigrationName \
  --project ../ExpenseTracker.Infrastructure \
  --startup-project .
```

### Apply Migrations
```bash
dotnet ef database update
```

### Rollback Migration
```bash
dotnet ef database update PreviousMigrationName
```

### Remove Last Migration
```bash
dotnet ef migrations remove \
  --project ../ExpenseTracker.Infrastructure \
  --startup-project .
```

## Running with Docker Compose

### Start All Services
```bash
# From project root
docker-compose up -d
```

### View Logs
```bash
docker-compose logs -f
```

### Stop All Services
```bash
docker-compose down
```

### Rebuild Services
```bash
docker-compose up -d --build
```

## Troubleshooting

### Backend Issues

**Problem:** Cannot connect to SQL Server
```bash
# Check if SQL Server is running
docker ps | grep mssql

# Check connection string in appsettings.json
# Ensure password matches Docker container
```

**Problem:** Migration errors
```bash
# Drop database and recreate
dotnet ef database drop --force
dotnet ef database update
```

### Frontend Issues

**Problem:** API calls failing
- Check NEXT_PUBLIC_API_URL in .env.local
- Ensure backend is running on port 5000
- Check browser console for CORS errors

**Problem:** NextAuth errors
- Ensure NEXTAUTH_SECRET is set
- Check NEXTAUTH_URL matches your frontend URL

### AI Service Issues

**Problem:** Module not found
```bash
# Ensure virtual environment is activated
source venv/bin/activate  # Mac/Linux
venv\Scripts\activate     # Windows

# Reinstall dependencies
pip install -r requirements.txt
```

## Development Workflow

### 1. Start Services
```bash
# Terminal 1: Backend
cd backend/src/ExpenseTracker.API
dotnet watch run

# Terminal 2: AI Service
cd ai-service
source venv/bin/activate
uvicorn app.main:app --reload --port 8000

# Terminal 3: Frontend
cd frontend
npm run dev
```

### 2. Make Changes
- Backend changes auto-reload with `dotnet watch`
- Frontend changes auto-reload with Next.js
- AI service changes auto-reload with `--reload` flag

### 3. Test Changes
- Use Swagger UI for backend: `http://localhost:5000/swagger`
- Use FastAPI docs for AI: `http://localhost:8000/docs`
- Test frontend in browser: `http://localhost:3000`

## Default Test Data

After running migrations, the database is seeded with default categories:
- 🍔 Food & Dining
- 🚗 Transportation
- 🛍️ Shopping
- 🎬 Entertainment
- 🏥 Healthcare
- 💡 Bills & Utilities
- 📦 Other

## Next Steps

1. ✅ Complete local setup
2. ✅ Test all services
3. ✅ Create your first expense
4. ✅ View AI insights
5. 📚 Read [ARCHITECTURE.md](./ARCHITECTURE.md) for production deployment
6. 🚀 Start building features!

## Getting Help

- Check Swagger/FastAPI docs for API endpoints
- Review logs in terminal
- Check browser console for frontend errors
- Refer to [ARCHITECTURE.md](./ARCHITECTURE.md) for system design
