using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MonthlySalary).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Icon).HasMaxLength(10);
            entity.Property(e => e.Color).HasMaxLength(20);
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MonthlySalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AllocatedAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var categories = new[]
        {
            new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Food & Dining", Icon = "🍔", Color = "#EF4444", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Transportation", Icon = "🚗", Color = "#3B82F6", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Shopping", Icon = "🛍️", Color = "#8B5CF6", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Entertainment", Icon = "🎬", Color = "#EC4899", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Healthcare", Icon = "🏥", Color = "#10B981", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Bills & Utilities", Icon = "💡", Color = "#F59E0B", IsDefault = true, CreatedAt = DateTime.UtcNow },
            new Category { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Other", Icon = "📦", Color = "#6B7280", IsDefault = true, CreatedAt = DateTime.UtcNow }
        };

        modelBuilder.Entity<Category>().HasData(categories);
    }
}
