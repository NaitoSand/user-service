using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace UserService.Tests.Application;

/// <summary>
/// Positive test cases for <see cref="UserService"/>.
/// Verifies validation, conflict, and exception handling logic.
/// </summary>
public class UserServicePositiveTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService.Application.Services.UserService _servicePositive;

    public UserServicePositiveTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _servicePositive = new UserService.Application.Services.UserService(_repositoryMock.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<UserService.Application.Services.UserService>>());
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenUserIsValid()
    {
        // Arrange
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };
        _repositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync((User?)null);

        // Act
        var result = await _servicePositive.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("john@doe.com");
        result.Value.FullName.Should().Be("John Doe");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenEmailIsMissing()
    {
        // Arrange
        var user = new User { Email = "", FullName = "John Doe" };

        // Act
        var result = await _servicePositive.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.MissingEmail);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenFullNameIsMissing()
    {
        // Arrange
        var user = new User { Email = "john@doe.com", FullName = "" };

        // Act
        var result = await _servicePositive.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.MissingFullName);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var existing = new User { Id = Guid.NewGuid(), Email = "john@doe.com" };
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };

        _repositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(existing);

        // Act
        var result = await _servicePositive.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.EmailConflict);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdate_WhenUserExists()
    {
        // Arrange
        var existing = new User
        {
            Id = Guid.NewGuid(),
            Email = "old@doe.com",
            FullName = "Old Doe",
            IsActive = true
        };

        var updated = new User
        {
            Id = existing.Id,
            Email = "new@doe.com",
            FullName = "New Name",
            IsActive = false
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.GetByEmailAsync(updated.Email)).ReturnsAsync((User?)null);

        // Act
        var result = await _servicePositive.UpdateAsync(updated);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Email.Should().Be("new@doe.com");
        result.Value.FullName.Should().Be("New Name");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "no@one.com", FullName = "Nobody" };
        _repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync((User?)null);

        // Act
        var result = await _servicePositive.UpdateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("NotFound");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
