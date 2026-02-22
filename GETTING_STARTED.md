# 🚀 Getting Started - Visual Guide

## Step 1: Prerequisites ✅

Install these before starting:

```
┌─────────────────────────────────────────────────────────┐
│  Required Software                                      │
├─────────────────────────────────────────────────────────┤
│  ✓ .NET 8 SDK                                          │
│  ✓ Node.js 18+                                         │
│  ✓ Python 3.11+                                        │
│  ✓ Docker Desktop                                      │
│  ✓ SQL Server (or Docker)                              │
└─────────────────────────────────────────────────────────┘
```

## Step 2: Quick Start (Choose One) 🎯

### Option A: Docker (Recommended) 🐳

```bash
# One command to rule them all!
docker-compose up -d

# Wait 30 seconds, then visit:
# → http://localhost:3000 (Frontend)
# → http://localhost:5000/swagger (Backend API)
# → http://localhost:8000/docs (AI Service)
```

### Option B: Manual Setup 🔧

```bash
# Terminal 1: Database
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Terminal 2: Backend
cd backend/src/ExpenseTracker.API
dotnet restore
dotnet ef database update
dotnet run

# Terminal 3: AI Service
cd ai-service
python -m venv venv && source venv/bin/activate
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000

# Terminal 4: Frontend
cd frontend
npm install
cp .env.example .env.local
npm run dev
```

## Step 3: First Login 👤

```
┌─────────────────────────────────────────────────────────┐
│  Visit: http://localhost:3000                           │
├─────────────────────────────────────────────────────────┤
│  1. Click "Sign up"                                     │
│  2. Enter your details:                                 │
│     • Email: your@email.com                             │
│     • Password: YourPassword123!                        │
│     • First Name: John                                  │
│     • Last Name: Doe                                    │
│  3. Click "Register"                                    │
│  4. You'll be logged in automatically!                  │
└─────────────────────────────────────────────────────────┘
```

## Step 4: Add Your First Expense 💰

```
┌─────────────────────────────────────────────────────────┐
│  Dashboard → Click "Expenses" → Click "Add Expense"     │
├─────────────────────────────────────────────────────────┤
│  Fill in the form:                                      │
│  • Category: 🍔 Food & Dining                           │
│  • Amount: 45.50                                        │
│  • Description: Lunch at restaurant                     │
│  • Date: Today                                          │
│  • Notes: Team lunch (optional)                         │
│                                                         │
│  Click "Save Expense"                                   │
└─────────────────────────────────────────────────────────┘
```

## Step 5: View AI Insights 🤖

```
┌─────────────────────────────────────────────────────────┐
│  Go back to Dashboard                                   │
├─────────────────────────────────────────────────────────┤
│  You'll see:                                            │
│  • Total expenses for the month                         │
│  • Number of transactions                               │
│  • Category breakdown chart                             │
│  • 💡 AI Insights section with:                         │
│    - Spending summary                                   │
│    - Personalized suggestions                           │
└─────────────────────────────────────────────────────────┘
```

## Architecture Overview 🏗️

```
┌──────────────┐
│   Browser    │
└──────┬───────┘
       │
       ▼
┌──────────────────────────────────────────────────────────┐
│              Next.js Frontend (Port 3000)                 │
│  • Login/Register pages                                   │
│  • Dashboard with charts                                  │
│  • Expense management                                     │
└────────────┬─────────────────────────┬────────────────────┘
             │                         │
             ▼                         ▼
┌────────────────────────┐   ┌─────────────────────────────┐
│  .NET API (Port 5000)  │   │  Python AI (Port 8000)      │
│  • Authentication      │◄──┤  • Spending summaries       │
│  • Expense CRUD        │   │  • Saving suggestions       │
│  • Monthly reports     │   │  • LLM-ready                │
└──────────┬─────────────┘   └─────────────────────────────┘
           │
           ▼
┌────────────────────────┐
│  SQL Server (1433)     │
│  • Users               │
│  • Expenses            │
│  • Categories          │
└────────────────────────┘
```

## Project Structure 📁

```
Personal Expense Tracker/
│
├── 📂 backend/              ← .NET 8 Clean Architecture
│   ├── Domain/              ← Entities (User, Expense, Category)
│   ├── Application/         ← Business logic
│   ├── Infrastructure/      ← Database, External APIs
│   └── API/                 ← Controllers, Middleware
│
├── 📂 frontend/             ← Next.js 14 App Router
│   ├── app/                 ← Pages (login, dashboard, expenses)
│   ├── components/          ← Reusable UI
│   ├── hooks/               ← React Query hooks
│   └── lib/                 ← API client
│
├── 📂 ai-service/           ← Python FastAPI
│   └── app/                 ← AI endpoints & logic
│
├── 📂 docs/                 ← Documentation
│   ├── SETUP.md             ← Setup guide
│   ├── ARCHITECTURE.md      ← System design
│   ├── API.md               ← API docs
│   └── CHECKLIST.md         ← Implementation steps
│
└── 🐳 docker-compose.yml    ← One-command deployment
```

## Key Features ✨

```
┌─────────────────────────────────────────────────────────┐
│  ✅ User Authentication (JWT)                            │
│  ✅ Expense CRUD Operations                              │
│  ✅ 7 Pre-configured Categories                          │
│  ✅ Monthly Expense Summaries                            │
│  ✅ Interactive Dashboard with Charts                    │
│  ✅ AI-Powered Spending Insights                         │
│  ✅ Personalized Saving Suggestions                      │
│  ✅ Responsive UI Design                                 │
│  ✅ RESTful API with Swagger Docs                        │
│  ✅ Docker Containerization                              │
└─────────────────────────────────────────────────────────┘
```

## Default Categories 🏷️

```
🍔 Food & Dining       🚗 Transportation
🛍️ Shopping            🎬 Entertainment
🏥 Healthcare          💡 Bills & Utilities
📦 Other
```

## API Endpoints 🔌

```
Authentication:
  POST /api/auth/register    → Register new user
  POST /api/auth/login       → Login user

Expenses:
  GET    /api/expenses       → Get all expenses
  POST   /api/expenses       → Create expense
  PUT    /api/expenses/{id}  → Update expense
  DELETE /api/expenses/{id}  → Delete expense
  GET    /api/expenses/summary/{year}/{month} → Monthly summary

Categories:
  GET /api/categories        → Get all categories

AI Service:
  POST /ai/summary           → Generate spending summary
  POST /ai/suggestions       → Get saving suggestions
```

## Testing Your Setup ✅

### 1. Backend Health Check
```bash
curl http://localhost:5000/health
# Expected: 200 OK
```

### 2. AI Service Health Check
```bash
curl http://localhost:8000/health
# Expected: {"status": "healthy"}
```

### 3. Frontend Access
```bash
open http://localhost:3000
# Expected: Login page loads
```

### 4. Swagger UI
```bash
open http://localhost:5000/swagger
# Expected: API documentation loads
```

## Troubleshooting 🔧

### Problem: Port already in use
```bash
# Find and kill process
lsof -i :3000  # or :5000, :8000
kill -9 <PID>
```

### Problem: Database connection failed
```bash
# Check SQL Server is running
docker ps | grep mssql

# Restart SQL Server
docker restart expense-tracker-db
```

### Problem: Frontend can't connect to backend
```bash
# Check .env.local file
cat frontend/.env.local

# Should contain:
# NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

### Problem: Docker issues
```bash
# Clean and rebuild
docker-compose down -v
docker system prune -a
docker-compose up -d --build
```

## Next Steps 🎯

```
1. ✅ Complete setup
2. ✅ Test all features
3. ✅ Add more expenses
4. ✅ View AI insights
5. 📚 Read docs/ARCHITECTURE.md for production deployment
6. 🚀 Customize and extend!
```

## Useful URLs 🔗

```
┌─────────────────────────────────────────────────────────┐
│  Service          │  URL                                 │
├───────────────────┼──────────────────────────────────────┤
│  Frontend         │  http://localhost:3000               │
│  Backend API      │  http://localhost:5000               │
│  Swagger UI       │  http://localhost:5000/swagger       │
│  AI Service       │  http://localhost:8000               │
│  FastAPI Docs     │  http://localhost:8000/docs          │
└─────────────────────────────────────────────────────────┘
```

## Documentation 📚

```
┌─────────────────────────────────────────────────────────┐
│  File                    │  Purpose                      │
├──────────────────────────┼───────────────────────────────┤
│  README.md               │  Project overview             │
│  EXECUTIVE_SUMMARY.md    │  High-level summary           │
│  QUICK_REFERENCE.md      │  Common commands              │
│  docs/SETUP.md           │  Detailed setup guide         │
│  docs/ARCHITECTURE.md    │  System design                │
│  docs/API.md             │  API documentation            │
│  docs/CHECKLIST.md       │  Implementation steps         │
│  docs/PROJECT_STRUCTURE  │  File structure               │
└─────────────────────────────────────────────────────────┘
```

## Success Checklist ✅

```
□ All services start without errors
□ Can register a new user
□ Can login successfully
□ Can create an expense
□ Dashboard displays correctly
□ Charts render properly
□ AI insights appear
□ Can edit an expense
□ Can delete an expense
□ Monthly summary calculates correctly
```

## Support 💬

If you encounter issues:
1. Check terminal logs for errors
2. Review documentation in docs/ folder
3. Test APIs with Swagger/FastAPI docs
4. Check browser console (F12)
5. Verify environment variables

---

**🎉 Congratulations! You're ready to build amazing features! 🎉**

**Happy coding! 💻**
