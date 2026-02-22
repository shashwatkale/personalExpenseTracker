using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.UnitTests.Services;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IAiService> _aiServiceMock;
    private readonly ExpenseService _sut;

    public ExpenseServiceTests()
    {
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _aiServiceMock = new Mock<IAiService>();
        _sut = new ExpenseService(_expenseRepositoryMock.Object, _categoryRepositoryMock.Object, _aiServiceMock.Object);
    }

    [Fact]
    public async Task CreateExpenseAsync_ValidRequest_ReturnsExpenseResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Food", Icon = "🍔", Color = "#FF0000" };
        var request = new CreateExpenseRequest(categoryId, 100m, "Lunch", DateTime.UtcNow, "Test");

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _expenseRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Expense>())).ReturnsAsync((Expense e) => e);

        // Act
        var result = await _sut.CreateExpenseAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(100m);
        result.Description.Should().Be("Lunch");
        result.CategoryName.Should().Be("Food");
    }

    [Fact]
    public async Task CreateExpenseAsync_InvalidCategory_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var request = new CreateExpenseRequest(categoryId, 100m, "Lunch", DateTime.UtcNow, null);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateExpenseAsync(userId, request));
    }

    [Fact]
    public async Task GetExpensesAsync_ReturnsExpenseList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = new Category { Id = Guid.NewGuid(), Name = "Food", Icon = "🍔", Color = "#FF0000" };
        var expenses = new List<Expense>
        {
            new Expense { Id = Guid.NewGuid(), UserId = userId, Category = category, Amount = 100m, Description = "Test", Date = DateTime.UtcNow }
        };

        _expenseRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, null, null)).ReturnsAsync(expenses);

        // Act
        var result = await _sut.GetExpensesAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().Amount.Should().Be(100m);
    }

    [Fact]
    public async Task DeleteExpenseAsync_CallsRepository()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();

        // Act
        await _sut.DeleteExpenseAsync(userId, expenseId);

        // Assert
        _expenseRepositoryMock.Verify(x => x.DeleteAsync(expenseId, userId), Times.Once);
    }
}
