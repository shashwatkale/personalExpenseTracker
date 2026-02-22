namespace ExpenseTracker.Domain.Entities;

public class Budget
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal MonthlySalary { get; set; }
    public Guid CategoryId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
