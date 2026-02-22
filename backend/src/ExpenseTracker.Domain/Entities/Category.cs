namespace ExpenseTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Icon { get; set; } = "📁";
    public string Color { get; set; } = "#6B7280";
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
