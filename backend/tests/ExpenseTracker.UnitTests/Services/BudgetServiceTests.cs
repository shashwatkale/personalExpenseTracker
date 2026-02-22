using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.UnitTests.Services;

public class BudgetServiceTests
{
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly BudgetService _sut;

    public BudgetServiceTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _sut = new BudgetService(_budgetRepositoryMock.Object, _categoryRepositoryMock.Object, 
            _userRepositoryMock.Object, _expenseRepositoryMock.Object);
    }

    [Fact]
    public async Task SetMonthlySalaryAsync_UpdatesSalary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new SetSalaryRequest(50000m);

        // Act
        await _sut.SetMonthlySalaryAsync(userId, request);

        // Assert
        _userRepositoryMock.Verify(x => x.SetMonthlySalaryAsync(userId, 50000m), Times.Once);
    }

    [Fact]
    public async Task CreateBudgetAsync_ValidRequest_ReturnsBudgetResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Food", Icon = "🍔", Color = "#FF0000" };
        var request = new CreateBudgetRequest(categoryId, 5000m, "Monthly Food Budget");

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _userRepositoryMock.Setup(x => x.GetMonthlySalaryAsync(userId)).ReturnsAsync(50000m);
        _budgetRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Budget>())).ReturnsAsync((Budget b) => b);
        _expenseRepositoryMock.Setup(x => x.GetCategorySpendingAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Dictionary<Guid, decimal>());

        // Act
        var result = await _sut.CreateBudgetAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.AllocatedAmount.Should().Be(5000m);
        result.Description.Should().Be("Monthly Food Budget");
    }

    [Fact]
    public async Task GetSalaryOverviewAsync_ReturnsOverview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Food", Icon = "🍔", Color = "#FF0000" };
        var budgets = new List<Budget>
        {
            new Budget { Id = Guid.NewGuid(), UserId = userId, CategoryId = categoryId, Category = category, 
                AllocatedAmount = 5000m, Description = "Food" }
        };

        _userRepositoryMock.Setup(x => x.GetMonthlySalaryAsync(userId)).ReturnsAsync(50000m);
        _budgetRepositoryMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(budgets);
        _expenseRepositoryMock.Setup(x => x.GetCategorySpendingAsync(userId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new Dictionary<Guid, decimal> { { categoryId, 3000m } });

        // Act
        var result = await _sut.GetSalaryOverviewAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.MonthlySalary.Should().Be(50000m);
        result.TotalAllocated.Should().Be(5000m);
        result.TotalSpent.Should().Be(3000m);
        result.Remaining.Should().Be(47000m);
    }
}
