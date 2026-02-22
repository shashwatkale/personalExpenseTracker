# API Documentation

## Base URLs

- **Backend API**: `http://localhost:5000/api`
- **AI Service**: `http://localhost:8000`

## Authentication

All protected endpoints require JWT Bearer token in Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

---

## Backend API Endpoints (.NET)

### Authentication

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "user@example.com",
  "firstName": "John",
  "lastName": "Doe"
}
```

---

### Expenses

#### Get All Expenses
```http
GET /api/expenses?startDate=2024-01-01&endDate=2024-01-31
Authorization: Bearer <token>
```

**Query Parameters:**
- `startDate` (optional): Filter expenses from this date (ISO 8601)
- `endDate` (optional): Filter expenses until this date (ISO 8601)

**Response (200 OK):**
```json
[
  {
    "id": "uuid",
    "categoryId": "uuid",
    "categoryName": "Food & Dining",
    "categoryIcon": "🍔",
    "categoryColor": "#EF4444",
    "amount": 45.50,
    "description": "Lunch at restaurant",
    "date": "2024-01-15T12:00:00Z",
    "notes": "Team lunch",
    "createdAt": "2024-01-15T12:05:00Z"
  }
]
```

#### Get Single Expense
```http
GET /api/expenses/{id}
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "categoryId": "uuid",
  "categoryName": "Food & Dining",
  "categoryIcon": "🍔",
  "categoryColor": "#EF4444",
  "amount": 45.50,
  "description": "Lunch at restaurant",
  "date": "2024-01-15T12:00:00Z",
  "notes": "Team lunch",
  "createdAt": "2024-01-15T12:05:00Z"
}
```

#### Create Expense
```http
POST /api/expenses
Authorization: Bearer <token>
Content-Type: application/json

{
  "categoryId": "uuid",
  "amount": 45.50,
  "description": "Lunch at restaurant",
  "date": "2024-01-15T12:00:00Z",
  "notes": "Team lunch"
}
```

**Response (201 Created):**
```json
{
  "id": "uuid",
  "categoryId": "uuid",
  "categoryName": "Food & Dining",
  "categoryIcon": "🍔",
  "categoryColor": "#EF4444",
  "amount": 45.50,
  "description": "Lunch at restaurant",
  "date": "2024-01-15T12:00:00Z",
  "notes": "Team lunch",
  "createdAt": "2024-01-15T12:05:00Z"
}
```

#### Update Expense
```http
PUT /api/expenses/{id}
Authorization: Bearer <token>
Content-Type: application/json

{
  "categoryId": "uuid",
  "amount": 50.00,
  "description": "Updated description",
  "date": "2024-01-15T12:00:00Z",
  "notes": "Updated notes"
}
```

**Response (200 OK):**
```json
{
  "id": "uuid",
  "categoryId": "uuid",
  "categoryName": "Food & Dining",
  "categoryIcon": "🍔",
  "categoryColor": "#EF4444",
  "amount": 50.00,
  "description": "Updated description",
  "date": "2024-01-15T12:00:00Z",
  "notes": "Updated notes",
  "createdAt": "2024-01-15T12:05:00Z"
}
```

#### Delete Expense
```http
DELETE /api/expenses/{id}
Authorization: Bearer <token>
```

**Response (204 No Content)**

#### Get Monthly Summary
```http
GET /api/expenses/summary/{year}/{month}
Authorization: Bearer <token>

Example: GET /api/expenses/summary/2024/1
```

**Response (200 OK):**
```json
{
  "totalExpenses": 1500.00,
  "expenseCount": 25,
  "categoryBreakdown": {
    "Food & Dining": 450.00,
    "Transportation": 300.00,
    "Shopping": 750.00
  },
  "aiSummary": "You spent $1500.00 across 3 categories this month. Your highest spending was in Shopping ($750.00, 50.0% of total). Consider diversifying your budget as Shopping dominates your expenses.",
  "aiSuggestions": [
    "🛍️ Shopping represents 50.0% of expenses. Consider a 30-day rule: wait before non-essential purchases.",
    "🍽️ Food spending is 30.0% of your budget. Try meal prepping to save 20-30% on dining costs."
  ]
}
```

---

### Categories

#### Get All Categories
```http
GET /api/categories
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
[
  {
    "id": "uuid",
    "name": "Food & Dining",
    "description": "Restaurants, groceries, etc.",
    "icon": "🍔",
    "color": "#EF4444"
  },
  {
    "id": "uuid",
    "name": "Transportation",
    "description": "Gas, public transit, etc.",
    "icon": "🚗",
    "color": "#3B82F6"
  }
]
```

---

## AI Service Endpoints (Python)

### Health Check
```http
GET /health
```

**Response (200 OK):**
```json
{
  "status": "healthy"
}
```

### Generate Spending Summary
```http
POST /ai/summary
Content-Type: application/json

{
  "user_id": "uuid",
  "total_amount": 1500.00,
  "category_breakdown": {
    "Food & Dining": 450.00,
    "Transportation": 300.00,
    "Shopping": 750.00
  }
}
```

**Response (200 OK):**
```json
{
  "summary": "You spent $1500.00 across 3 categories this month. Your highest spending was in Shopping ($750.00, 50.0% of total). Consider diversifying your budget as Shopping dominates your expenses."
}
```

### Generate Saving Suggestions
```http
POST /ai/suggestions
Content-Type: application/json

{
  "user_id": "uuid",
  "total_amount": 1500.00,
  "category_breakdown": {
    "Food & Dining": 450.00,
    "Transportation": 300.00,
    "Shopping": 750.00
  }
}
```

**Response (200 OK):**
```json
{
  "suggestions": [
    "🛍️ Shopping represents 50.0% of expenses. Consider a 30-day rule: wait before non-essential purchases.",
    "🍽️ Food spending is 30.0% of your budget. Try meal prepping to save 20-30% on dining costs.",
    "🚗 Transportation costs are 20.0%. Consider carpooling, public transit, or biking for short trips."
  ]
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "Validation error message",
  "details": "Additional error details"
}
```

### 401 Unauthorized
```json
{
  "message": "Invalid credentials"
}
```

### 404 Not Found
```json
{
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "statusCode": 500,
  "message": "An error occurred",
  "details": "Error details"
}
```

---

## Rate Limiting

Currently not implemented. Recommended for production:
- Auth endpoints: 5 requests per minute
- Other endpoints: 100 requests per minute

---

## Pagination

Not currently implemented. For future enhancement:

```http
GET /api/expenses?page=1&pageSize=20
```

**Response:**
```json
{
  "data": [...],
  "page": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8
}
```

---

## Testing with cURL

### Register and Login
```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }'

# Login (save the token)
TOKEN=$(curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!"
  }' | jq -r '.token')
```

### Create Expense
```bash
curl -X POST http://localhost:5000/api/expenses \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "categoryId": "11111111-1111-1111-1111-111111111111",
    "amount": 45.50,
    "description": "Lunch",
    "date": "2024-01-15T12:00:00Z"
  }'
```

### Get Expenses
```bash
curl -X GET http://localhost:5000/api/expenses \
  -H "Authorization: Bearer $TOKEN"
```

---

## Interactive API Documentation

- **Backend Swagger UI**: http://localhost:5000/swagger
- **AI Service FastAPI Docs**: http://localhost:8000/docs
