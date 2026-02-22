using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAiService _aiService;

    public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IAiService aiService)
    {
        _expenseRepository = expenseRepository;
        _categoryRepository = categoryRepository;
        _aiService = aiService;
    }

    public async Task<ExpenseResponse> CreateExpenseAsync(Guid userId, CreateExpenseRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new ArgumentException("Category not found");

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Description = request.Description,
            Date = request.Date,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _expenseRepository.AddAsync(expense);
        
        return MapToResponse(expense, category);
    }

    public async Task<ExpenseResponse> UpdateExpenseAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
        if (expense == null) throw new ArgumentException("Expense not found");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new ArgumentException("Category not found");

        expense.CategoryId = request.CategoryId;
        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.Date = request.Date;
        expense.Notes = request.Notes;
        expense.UpdatedAt = DateTime.UtcNow;

        await _expenseRepository.UpdateAsync(expense);
        
        return MapToResponse(expense, category);
    }

    public async Task DeleteExpenseAsync(Guid userId, Guid expenseId)
    {
        await _expenseRepository.DeleteAsync(expenseId, userId);
    }

    public async Task<ExpenseResponse?> GetExpenseByIdAsync(Guid userId, Guid expenseId)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId, userId);
        if (expense == null) return null;
        
        return MapToResponse(expense, expense.Category);
    }

    public async Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var expenses = await _expenseRepository.GetByUserIdAsync(userId, startDate, endDate);
        return expenses.Select(e => MapToResponse(e, e.Category));
    }

    public async Task<MonthlySummaryResponse> GetMonthlySummaryAsync(Guid userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var total = await _expenseRepository.GetTotalByPeriodAsync(userId, startDate, endDate);
        var breakdown = await _expenseRepository.GetCategoryBreakdownAsync(userId, startDate, endDate);
        var expenses = await _expenseRepository.GetByUserIdAsync(userId, startDate, endDate);

        var aiSummary = await _aiService.GetSpendingSummaryAsync(userId, breakdown, total);
        var aiSuggestions = await _aiService.GetSavingSuggestionsAsync(userId, breakdown, total);

        return new MonthlySummaryResponse(
            total,
            expenses.Count(),
            breakdown,
            aiSummary,
            aiSuggestions
        );
    }

    private static ExpenseResponse MapToResponse(Expense expense, Category category)
    {
        return new ExpenseResponse(
            expense.Id,
            expense.CategoryId,
            category.Name,
            category.Icon,
            category.Color,
            expense.Amount,
            expense.Description,
            expense.Date,
            expense.Notes,
            expense.CreatedAt
        );
    }
}
