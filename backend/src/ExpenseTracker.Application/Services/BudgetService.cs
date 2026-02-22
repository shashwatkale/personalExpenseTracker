using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;

namespace ExpenseTracker.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IExpenseRepository _expenseRepository;

    public BudgetService(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository, 
        IUserRepository userRepository, IExpenseRepository expenseRepository)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<BudgetResponse> CreateBudgetAsync(Guid userId, CreateBudgetRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new ArgumentException("Category not found");

        var salary = await _userRepository.GetMonthlySalaryAsync(userId);

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            AllocatedAmount = request.AllocatedAmount,
            MonthlySalary = salary,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _budgetRepository.AddAsync(budget);
        
        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var spending = await _expenseRepository.GetCategorySpendingAsync(userId, startDate, endDate);
        var spentAmount = spending.ContainsKey(request.CategoryId) ? spending[request.CategoryId] : 0;

        return new BudgetResponse(
            budget.Id,
            category.Id,
            category.Name,
            category.Icon,
            budget.AllocatedAmount,
            spentAmount,
            budget.AllocatedAmount - spentAmount,
            budget.Description
        );
    }

    public async Task<BudgetResponse> UpdateBudgetAsync(Guid userId, Guid budgetId, UpdateBudgetRequest request)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId, userId);
        if (budget == null) throw new ArgumentException("Budget not found");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
        if (category == null) throw new ArgumentException("Category not found");

        budget.CategoryId = request.CategoryId;
        budget.AllocatedAmount = request.AllocatedAmount;
        budget.Description = request.Description;
        budget.UpdatedAt = DateTime.UtcNow;

        await _budgetRepository.UpdateAsync(budget);

        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var spending = await _expenseRepository.GetCategorySpendingAsync(userId, startDate, endDate);
        var spentAmount = spending.ContainsKey(request.CategoryId) ? spending[request.CategoryId] : 0;

        return new BudgetResponse(
            budget.Id,
            category.Id,
            category.Name,
            category.Icon,
            budget.AllocatedAmount,
            spentAmount,
            budget.AllocatedAmount - spentAmount,
            budget.Description
        );
    }

    public async Task DeleteBudgetAsync(Guid userId, Guid budgetId)
    {
        await _budgetRepository.DeleteAsync(budgetId, userId);
    }

    public async Task<IEnumerable<BudgetResponse>> GetBudgetsAsync(Guid userId)
    {
        var budgets = await _budgetRepository.GetByUserIdAsync(userId);
        var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var spending = await _expenseRepository.GetCategorySpendingAsync(userId, startDate, endDate);

        return budgets.Select(b =>
        {
            var spentAmount = spending.ContainsKey(b.CategoryId) ? spending[b.CategoryId] : 0;
            return new BudgetResponse(
                b.Id,
                b.Category.Id,
                b.Category.Name,
                b.Category.Icon,
                b.AllocatedAmount,
                spentAmount,
                b.AllocatedAmount - spentAmount,
                b.Description
            );
        });
    }

    public async Task SetMonthlySalaryAsync(Guid userId, SetSalaryRequest request)
    {
        await _userRepository.SetMonthlySalaryAsync(userId, request.MonthlySalary);
    }

    public async Task<SalaryOverviewResponse> GetSalaryOverviewAsync(Guid userId)
    {
        var salary = await _userRepository.GetMonthlySalaryAsync(userId);
        var budgets = await GetBudgetsAsync(userId);
        var budgetList = budgets.ToList();
        
        var totalAllocated = budgetList.Sum(b => b.AllocatedAmount);
        var totalSpent = budgetList.Sum(b => b.SpentAmount);
        var remaining = salary - totalSpent;

        return new SalaryOverviewResponse(
            salary,
            totalAllocated,
            totalSpent,
            remaining,
            budgetList
        );
    }
}
