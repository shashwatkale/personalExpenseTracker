# Production Architecture Guide

## System Architecture

### Microservices Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Internet / Load Balancer                  │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
                    ┌────────────────┐
                    │   API Gateway  │ (Optional)
                    │   / Nginx      │
                    └────────┬───────┘
                             │
              ┌──────────────┼──────────────┐
              ▼              ▼              ▼
    ┌─────────────┐  ┌─────────────┐  ┌──────────┐
    │  Next.js    │  │  .NET API   │  │ Python   │
    │  Frontend   │  │  (Port 5000)│  │ AI       │
    │  (Port 3000)│  │             │  │ (8000)   │
    └─────────────┘  └──────┬──────┘  └────┬─────┘
                            │              │
                            ▼              │
                    ┌──────────────┐       │
                    │  SQL Server  │       │
                    │  (Port 1433) │       │
                    └──────────────┘       │
                            │              │
                            ▼              ▼
                    ┌──────────────────────┐
                    │   Redis Cache        │
                    │   (Port 6379)        │
                    └──────────────────────┘
```

## Authentication Flow

1. **User Login**
   - User submits credentials to Next.js
   - Next.js calls .NET API `/auth/login`
   - .NET validates credentials, generates JWT
   - JWT returned to NextAuth
   - NextAuth stores JWT in session

2. **Authenticated Requests**
   - Next.js includes JWT in Authorization header
   - .NET validates JWT on each request
   - User ID extracted from JWT claims
   - Request processed with user context

3. **Token Refresh** (Future Enhancement)
   - Implement refresh tokens
   - Store refresh token in HttpOnly cookie
   - Access token expires in 15 minutes
   - Refresh token expires in 7 days

## API Contract: .NET ↔ Python

### .NET calls Python AI Service

**Endpoint:** `POST http://ai-service:8000/ai/summary`

**Request:**
```json
{
  "user_id": "uuid-string",
  "total_amount": 1500.00,
  "category_breakdown": {
    "Food & Dining": 450.00,
    "Transportation": 300.00,
    "Shopping": 750.00
  }
}
```

**Response:**
```json
{
  "summary": "You spent $1500.00 across 3 categories this month..."
}
```

**Endpoint:** `POST http://ai-service:8000/ai/suggestions`

**Response:**
```json
{
  "suggestions": [
    "🍽️ Food spending is 30% of your budget...",
    "🛍️ Shopping represents 50% of expenses..."
  ]
}
```

## Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL
);
```

### Categories Table
```sql
CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Icon NVARCHAR(10) NOT NULL,
    Color NVARCHAR(20) NOT NULL,
    IsDefault BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
```

### Expenses Table
```sql
CREATE TABLE Expenses (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    CategoryId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Date DATETIME2 NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
```

## Environment Variables Strategy

### Development
- Use `.env` files (gitignored)
- Store in project root for each service
- Never commit secrets

### Production
- Use cloud secret managers:
  - **AWS**: AWS Secrets Manager / Parameter Store
  - **Azure**: Azure Key Vault
  - **GCP**: Secret Manager
- Inject at runtime via environment variables
- Rotate secrets regularly

### Example: Azure Key Vault Integration (.NET)
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential()
);
```

## Logging & Monitoring

### .NET API
```csharp
// Use Serilog
builder.Host.UseSerilog((context, config) => {
    config
        .WriteTo.Console()
        .WriteTo.File("logs/api-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
});
```

### Python AI Service
```python
import logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
```

### Next.js Frontend
- Use console for development
- Send errors to monitoring service (Sentry, DataDog)

### Recommended Tools
- **Application Performance**: Application Insights, New Relic, DataDog
- **Error Tracking**: Sentry
- **Log Aggregation**: ELK Stack, CloudWatch, Azure Monitor

## Security Best Practices

### 1. Authentication & Authorization
- ✅ Use strong JWT secrets (32+ characters)
- ✅ Implement token expiration
- ✅ Use HTTPS in production
- ✅ Implement refresh tokens
- ✅ Rate limiting on auth endpoints

### 2. API Security
- ✅ Validate all inputs (FluentValidation)
- ✅ Use parameterized queries (EF Core does this)
- ✅ Implement CORS properly
- ✅ Add API rate limiting
- ✅ Use API keys for service-to-service communication

### 3. Database Security
- ✅ Use connection string encryption
- ✅ Principle of least privilege for DB users
- ✅ Enable SQL Server encryption at rest
- ✅ Regular backups
- ✅ Audit logging

### 4. Frontend Security
- ✅ Never store sensitive data in localStorage
- ✅ Use HttpOnly cookies for tokens
- ✅ Implement CSP headers
- ✅ Sanitize user inputs
- ✅ Keep dependencies updated

## CI/CD Pipeline

### GitHub Actions Example

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  backend-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: 8.0.x
      - name: Restore dependencies
        run: dotnet restore ./backend/ExpenseTracker.sln
      - name: Build
        run: dotnet build ./backend/ExpenseTracker.sln --no-restore
      - name: Test
        run: dotnet test ./backend/ExpenseTracker.sln --no-build

  frontend-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node
        uses: actions/setup-node@v3
        with:
          node-version: 18
      - name: Install dependencies
        run: cd frontend && npm ci
      - name: Build
        run: cd frontend && npm run build

  deploy:
    needs: [backend-test, frontend-test]
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Deploy to production
        run: echo "Deploy to cloud provider"
```

## Deployment Options

### Option 1: Docker Compose (Simple)
```bash
docker-compose up -d
```

### Option 2: Kubernetes (Scalable)
- Create Kubernetes manifests
- Use Helm charts
- Deploy to AKS, EKS, or GKE

### Option 3: Cloud Native
- **Frontend**: Vercel, Netlify, Azure Static Web Apps
- **Backend**: Azure App Service, AWS Elastic Beanstalk
- **AI Service**: Azure Container Instances, AWS Fargate
- **Database**: Azure SQL, AWS RDS

## Performance Optimization

### Backend
- ✅ Use async/await consistently
- ✅ Implement caching (Redis)
- ✅ Database indexing on foreign keys
- ✅ Use pagination for large datasets
- ✅ Connection pooling

### Frontend
- ✅ Code splitting
- ✅ Image optimization
- ✅ React Query caching
- ✅ Lazy loading components
- ✅ CDN for static assets

### Database
- ✅ Index frequently queried columns
- ✅ Optimize queries (avoid N+1)
- ✅ Use stored procedures for complex operations
- ✅ Regular maintenance (rebuild indexes)

## Common Pitfalls to Avoid

### 1. Authentication
- ❌ Storing passwords in plain text
- ❌ Weak JWT secrets
- ❌ No token expiration
- ✅ Use BCrypt for password hashing
- ✅ Implement proper token lifecycle

### 2. API Design
- ❌ Exposing internal IDs without validation
- ❌ No input validation
- ❌ Returning stack traces to client
- ✅ Use DTOs for all API responses
- ✅ Global exception handling

### 3. Database
- ❌ No connection string encryption
- ❌ Missing indexes
- ❌ No backup strategy
- ✅ Use migrations for schema changes
- ✅ Regular backups

### 4. Frontend
- ❌ Storing JWT in localStorage (XSS vulnerable)
- ❌ No error boundaries
- ❌ Hardcoded API URLs
- ✅ Use environment variables
- ✅ Implement proper error handling

### 5. DevOps
- ❌ No health check endpoints
- ❌ Missing logging
- ❌ No monitoring
- ✅ Implement /health endpoints
- ✅ Structured logging

## Scaling Considerations

### Horizontal Scaling
- Load balancer in front of API
- Multiple API instances
- Stateless API design
- Shared Redis cache

### Database Scaling
- Read replicas for reporting
- Connection pooling
- Query optimization
- Consider sharding for massive scale

### Caching Strategy
- Cache frequently accessed data
- Use Redis for distributed cache
- Implement cache invalidation
- Cache AI responses

## Monitoring Checklist

- [ ] API response times
- [ ] Error rates
- [ ] Database query performance
- [ ] Memory usage
- [ ] CPU usage
- [ ] Disk space
- [ ] Active users
- [ ] Failed login attempts
- [ ] AI service availability
- [ ] Cache hit rates
