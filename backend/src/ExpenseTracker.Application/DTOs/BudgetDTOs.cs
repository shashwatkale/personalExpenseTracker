namespace ExpenseTracker.Application.DTOs;

public record CreateBudgetRequest(Guid CategoryId, decimal AllocatedAmount, string Description);
public record UpdateBudgetRequest(Guid CategoryId, decimal AllocatedAmount, string Description);
public record SetSalaryRequest(decimal MonthlySalary);

public record BudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryIcon,
    decimal AllocatedAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    string Description
);

public record SalaryOverviewResponse(
    decimal MonthlySalary,
    decimal TotalAllocated,
    decimal TotalSpent,
    decimal Remaining,
    List<BudgetResponse> Budgets
);
