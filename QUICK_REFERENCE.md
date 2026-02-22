# Quick Reference Card

## 🚀 Essential Commands

### Start Everything (Docker)
```bash
docker-compose up -d
```

### Start Services Individually

#### Backend (.NET)
```bash
cd backend/src/ExpenseTracker.API
dotnet restore
dotnet ef database update
dotnet run
# Access: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

#### AI Service (Python)
```bash
cd ai-service
python -m venv venv
source venv/bin/activate  # Windows: venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --reload --port 8000
# Access: http://localhost:8000
# Docs: http://localhost:8000/docs
```

#### Frontend (Next.js)
```bash
cd frontend
npm install
cp .env.example .env.local
npm run dev
# Access: http://localhost:3000
```

## 🗄️ Database Commands

### Start SQL Server (Docker)
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name expense-tracker-db \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Create Migration
```bash
cd backend/src/ExpenseTracker.API
dotnet ef migrations add MigrationName \
  --project ../ExpenseTracker.Infrastructure \
  --startup-project .
```

### Apply Migration
```bash
dotnet ef database update
```

### Drop Database
```bash
dotnet ef database drop --force
```

## 🧪 Testing Commands

### Test Backend API
```bash
# Health check
curl http://localhost:5000/health

# Register user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!","firstName":"Test","lastName":"User"}'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

### Test AI Service
```bash
curl -X POST http://localhost:8000/ai/summary \
  -H "Content-Type: application/json" \
  -d '{"user_id":"test","total_amount":1500,"category_breakdown":{"Food":500}}'
```

## 🐳 Docker Commands

### View Logs
```bash
docker-compose logs -f
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f ai-service
```

### Stop Services
```bash
docker-compose down
```

### Rebuild Services
```bash
docker-compose up -d --build
```

### Remove Volumes
```bash
docker-compose down -v
```

## 📦 Package Management

### Backend
```bash
# Add package
dotnet add package PackageName

# Restore packages
dotnet restore

# Update packages
dotnet list package --outdated
```

### Frontend
```bash
# Install packages
npm install

# Add package
npm install package-name

# Update packages
npm update

# Check outdated
npm outdated
```

### AI Service
```bash
# Install packages
pip install -r requirements.txt

# Add package
pip install package-name
pip freeze > requirements.txt

# Update packages
pip list --outdated
```

## 🔍 Debugging

### Check Running Services
```bash
# Check ports
lsof -i :3000  # Frontend
lsof -i :5000  # Backend
lsof -i :8000  # AI Service
lsof -i :1433  # SQL Server

# Check Docker containers
docker ps
```

### View Application Logs
```bash
# Backend logs
cd backend/src/ExpenseTracker.API
dotnet run --verbosity detailed

# Frontend logs (browser console)
# Open DevTools (F12) → Console

# AI Service logs
cd ai-service
uvicorn app.main:app --reload --log-level debug
```

## 🔐 Security

### Generate JWT Secret
```bash
openssl rand -base64 32
```

### Generate NextAuth Secret
```bash
openssl rand -base64 32
```

## 📊 Database Queries

### Connect to SQL Server
```bash
# Using Azure Data Studio or SSMS
Server: localhost,1433
Username: sa
Password: YourStrong@Passw0rd
```

### Useful Queries
```sql
-- View all users
SELECT * FROM Users;

-- View all expenses
SELECT * FROM Expenses;

-- View all categories
SELECT * FROM Categories;

-- Monthly summary
SELECT 
    MONTH(Date) as Month,
    SUM(Amount) as Total,
    COUNT(*) as Count
FROM Expenses
WHERE UserId = 'your-user-id'
GROUP BY MONTH(Date);
```

## 🎨 Frontend Development

### Add New Page
```bash
# Create page file
mkdir -p frontend/app/new-page
touch frontend/app/new-page/page.tsx
```

### Add New Component
```bash
touch frontend/components/NewComponent.tsx
```

### Add New Hook
```bash
touch frontend/hooks/useNewHook.ts
```

## 🔧 Backend Development

### Add New Entity
1. Create entity in `Domain/Entities/`
2. Add DbSet to `ApplicationDbContext`
3. Create migration
4. Apply migration

### Add New Endpoint
1. Add method to service interface
2. Implement in service class
3. Add controller action
4. Test with Swagger

## 📝 Git Commands

### Initial Commit
```bash
git init
git add .
git commit -m "Initial commit: Production-ready expense tracker"
git branch -M main
git remote add origin <your-repo-url>
git push -u origin main
```

### Feature Branch
```bash
git checkout -b feature/new-feature
# Make changes
git add .
git commit -m "Add new feature"
git push origin feature/new-feature
```

## 🌐 URLs Reference

| Service | URL | Description |
|---------|-----|-------------|
| Frontend | http://localhost:3000 | Main application |
| Backend API | http://localhost:5000 | REST API |
| Swagger UI | http://localhost:5000/swagger | API documentation |
| AI Service | http://localhost:8000 | AI microservice |
| FastAPI Docs | http://localhost:8000/docs | AI API docs |
| SQL Server | localhost:1433 | Database |

## 📚 Documentation Files

| File | Purpose |
|------|---------|
| README.md | Project overview |
| EXECUTIVE_SUMMARY.md | High-level summary |
| docs/SETUP.md | Setup instructions |
| docs/ARCHITECTURE.md | System design |
| docs/API.md | API documentation |
| docs/CHECKLIST.md | Implementation guide |
| docs/PROJECT_STRUCTURE.md | File structure |
| QUICK_REFERENCE.md | This file |

## 🆘 Troubleshooting

### Port Already in Use
```bash
# Find process using port
lsof -i :3000
# Kill process
kill -9 <PID>
```

### Database Connection Failed
- Check SQL Server is running
- Verify connection string
- Check firewall settings

### Frontend Can't Connect to Backend
- Check NEXT_PUBLIC_API_URL in .env.local
- Verify backend is running on port 5000
- Check CORS settings

### Docker Issues
```bash
# Clean everything
docker-compose down -v
docker system prune -a
# Rebuild
docker-compose up -d --build
```

## 💡 Pro Tips

1. **Use Swagger**: Test APIs before frontend integration
2. **Check Logs**: Most issues visible in terminal
3. **Git Branches**: Create feature branches
4. **Environment Variables**: Never commit secrets
5. **Docker First**: Use Docker Compose for consistency
6. **Read Docs**: Check docs/ folder for details

## 🎯 Common Tasks

| Task | Command |
|------|---------|
| Start all services | `docker-compose up -d` |
| View logs | `docker-compose logs -f` |
| Stop services | `docker-compose down` |
| Create migration | `dotnet ef migrations add Name` |
| Apply migration | `dotnet ef database update` |
| Install frontend deps | `npm install` |
| Install backend deps | `dotnet restore` |
| Run tests | `dotnet test` |
| Build for production | `docker-compose build` |

---

**Keep this file handy for quick reference! 📌**
