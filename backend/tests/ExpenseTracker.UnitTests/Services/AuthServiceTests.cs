using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ExpenseTracker.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _sut = new AuthService(_userRepositoryMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_NewUser_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "password123", "John", "Doe");
        _userRepositoryMock.Setup(x => x.ExistsAsync(request.Email)).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<Guid>(), request.Email)).Returns("test-token");

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        result.Token.Should().Be("test-token");
        result.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task RegisterAsync_ExistingUser_ThrowsArgumentException()
    {
        // Arrange
        var request = new RegisterRequest("test@example.com", "password123", "John", "Doe");
        _userRepositoryMock.Setup(x => x.ExistsAsync(request.Email)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "password123");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "John",
            LastName = "Doe"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync(user);
        _jwtServiceMock.Setup(x => x.GenerateToken(user.Id, user.Email)).Returns("test-token");

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        result.Token.Should().Be("test-token");
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "wrongpassword");
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.LoginAsync(request));
    }
}
