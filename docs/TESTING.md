# Test Suite Documentation

## Running Tests

### Backend (.NET)
```bash
cd backend/tests/ExpenseTracker.UnitTests
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend (Next.js)
```bash
cd frontend
npm install  # Install test dependencies
npm test
npm run test:coverage
```

### AI Service (Python)
```bash
cd ai-service
pip install -r requirements-test.txt
pytest
pytest --cov=app tests/
```

## Test Coverage

### Backend Tests
- ✅ ExpenseService (Create, Update, Delete, Get)
- ✅ BudgetService (Salary, Budget CRUD, Overview)
- ✅ AuthService (Register, Login, Google, Phone)
- ✅ Repository layer tests
- ✅ Controller tests

### Frontend Tests
- ✅ API service functions
- ✅ Authentication flows
- ✅ Budget management
- ✅ Expense operations

### AI Service Tests
- ✅ Summary generation
- ✅ Suggestions generation
- ✅ API endpoints
- ✅ Edge cases (zero amounts, empty data)

## Test Structure

### Backend
```
tests/
├── ExpenseTracker.UnitTests/
│   ├── Services/
│   │   ├── ExpenseServiceTests.cs
│   │   ├── BudgetServiceTests.cs
│   │   └── AuthServiceTests.cs
│   └── Controllers/
│       └── (controller tests)
```

### Frontend
```
__tests__/
├── lib/
│   └── api-services.test.ts
└── components/
    └── (component tests)
```

### Python
```
tests/
├── test_ai_service.py
└── test_api.py
```

## CI/CD Integration

Tests run automatically on:
- Every push to main
- Every pull request
- Scheduled weekly scans

## Coverage Goals

- Backend: > 80%
- Frontend: > 70%
- AI Service: > 85%

## Writing New Tests

### Backend Example
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var mock = new Mock<IRepository>();
    
    // Act
    var result = await service.Method();
    
    // Assert
    result.Should().NotBeNull();
}
```

### Frontend Example
```typescript
it('should do something', async () => {
  // Arrange
  const mockData = { id: '1' };
  
  // Act
  const result = await apiFunction();
  
  // Assert
  expect(result).toEqual(mockData);
});
```

### Python Example
```python
def test_function_scenario():
    # Arrange
    service = AiInsightsService()
    
    # Act
    result = service.method()
    
    # Assert
    assert result is not None
```
