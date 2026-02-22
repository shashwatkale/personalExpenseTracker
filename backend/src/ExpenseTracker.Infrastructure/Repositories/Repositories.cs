using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using ExpenseTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(Guid id, Guid userId)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(e => e.Date >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(e => e.Date <= endDate.Value);

        return await query.OrderByDescending(e => e.Date).ToListAsync();
    }

    public async Task<Expense> AddAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var expense = await GetByIdAsync(id, userId);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalByPeriodAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
            .SumAsync(e => e.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetCategoryBreakdownAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
            .GroupBy(e => e.Category.Name)
            .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
            .ToDictionaryAsync(x => x.Category, x => x.Total);
    }

    public async Task<Dictionary<Guid, decimal>> GetCategorySpendingAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Expenses
            .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
            .GroupBy(e => e.CategoryId)
            .Select(g => new { CategoryId = g.Key, Total = g.Sum(e => e.Amount) })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Total);
    }
}

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }
}

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByPhoneAsync(string phoneNumber)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task SetMonthlySalaryAsync(Guid userId, decimal salary)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.MonthlySalary = salary;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    public async Task<decimal> GetMonthlySalaryAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        return user?.MonthlySalary ?? 0;
    }
}
