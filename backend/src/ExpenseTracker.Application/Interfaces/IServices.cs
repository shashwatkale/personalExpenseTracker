using ExpenseTracker.Application.DTOs;

namespace ExpenseTracker.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
    Task<AuthResponse> LoginWithPhoneAsync(PhoneLoginRequest request);
    Task<PhoneCodeResponse> SendPhoneVerificationAsync(SendPhoneCodeRequest request);
}

public interface IExpenseService
{
    Task<ExpenseResponse> CreateExpenseAsync(Guid userId, CreateExpenseRequest request);
    Task<ExpenseResponse> UpdateExpenseAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request);
    Task DeleteExpenseAsync(Guid userId, Guid expenseId);
    Task<ExpenseResponse?> GetExpenseByIdAsync(Guid userId, Guid expenseId);
    Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<MonthlySummaryResponse> GetMonthlySummaryAsync(Guid userId, int year, int month);
}

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
}

public interface IAiService
{
    Task<string> GetSpendingSummaryAsync(Guid userId, Dictionary<string, decimal> categoryBreakdown, decimal total);
    Task<List<string>> GetSavingSuggestionsAsync(Guid userId, Dictionary<string, decimal> categoryBreakdown, decimal total);
}

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
    Guid? ValidateToken(string token);
}

public interface IBudgetService
{
    Task<BudgetResponse> CreateBudgetAsync(Guid userId, CreateBudgetRequest request);
    Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request);
    Task DeleteBudgetAsync(Guid userId, Guid budgetId);
    Task<IEnumerable<BudgetResponse>> GetBudgetsAsync(Guid userId);
    Task SetMonthlySalaryAsync(Guid userId, SetSalaryRequest request);
    Task<SalaryOverviewResponse> GetSalaryOverviewAsync(Guid userId);
}
