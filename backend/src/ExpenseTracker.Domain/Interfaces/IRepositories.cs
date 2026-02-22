using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Expense>> GetByUserIdAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<Expense> AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Guid id, Guid userId);
    Task<decimal> GetTotalByPeriodAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetCategoryBreakdownAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<Dictionary<Guid, decimal>> GetCategorySpendingAsync(Guid userId, DateTime startDate, DateTime endDate);
}

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category> AddAsync(Category category);
}

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneAsync(string phoneNumber);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> ExistsAsync(string email);
    Task SetMonthlySalaryAsync(Guid userId, decimal salary);
    Task<decimal> GetMonthlySalaryAsync(Guid userId);
}

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<Budget>> GetByUserIdAsync(Guid userId);
    Task<Budget> AddAsync(Budget budget);
    Task UpdateAsync(Budget budget);
    Task DeleteAsync(Guid id, Guid userId);
}
