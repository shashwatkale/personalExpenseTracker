namespace ExpenseTracker.Application.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record AuthResponse(string Token, string Email, string FirstName, string LastName);

public record CreateExpenseRequest(Guid CategoryId, decimal Amount, string Description, DateTime Date, string? Notes);
public record UpdateExpenseRequest(Guid CategoryId, decimal Amount, string Description, DateTime Date, string? Notes);

public record ExpenseResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    decimal Amount,
    string Description,
    DateTime Date,
    string? Notes,
    DateTime CreatedAt
);

public record CategoryResponse(Guid Id, string Name, string? Description, string Icon, string Color);

public record MonthlySummaryResponse(
    decimal TotalExpenses,
    int ExpenseCount,
    Dictionary<string, decimal> CategoryBreakdown,
    string AiSummary,
    List<string> AiSuggestions
);
