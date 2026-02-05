using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace UserService.Tests.Application;

/// <summary>
/// Negative test cases for <see cref="UserService"/>.
/// Verifies validation, conflict, and exception handling logic.
/// </summary>
public class UserServiceNegativeTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService.Application.Services.UserService _service;

    public UserServiceNegativeTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserService.Application.Services.UserService(_repositoryMock.Object, Mock.Of<Microsoft.Extensions.Logging.ILogger<UserService.Application.Services.UserService>>());
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenEmailIsMissing()
    {
        // Arrange
        var user = new User { Email = "", FullName = "John Doe" };

        // Act
        var result = await _service.CreateAsync(user);

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
        var result = await _service.CreateAsync(user);

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
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.EmailConflict);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenEmailIsTooLong()
    {
        // Arrange
        var user = new User
        {
            Email = new string('a', 201) + "@mail.com",
            FullName = "John Doe"
        };

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.EmailTooLong);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenFullNameIsTooLong()
    {
        // Arrange
        var user = new User
        {
            Email = "john@doe.com",
            FullName = new string('x', 201)
        };

        // Act
        var result = await _service.CreateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.FullNameTooLong);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "ghost@user.com", FullName = "Ghost" };
        _repositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.UpdateAsync(user);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Contain("NotFound");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenEmailConflictDetected()
    {
        // Arrange
        var existing = new User { Id = Guid.NewGuid(), Email = "old@doe.com" };
        var another = new User { Id = Guid.NewGuid(), Email = "new@doe.com" };
        var update = new User { Id = existing.Id, Email = "new@doe.com", FullName = "John Doe" };

        _repositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.GetByEmailAsync(update.Email)).ReturnsAsync(another);

        // Act
        var result = await _service.UpdateAsync(update);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Errors.User.EmailConflict);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenRepositoryThrowsException()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "fail@case.com", FullName = "Crash Test" };
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Database unavailable"));

        // Act
        Func<Task> act = async () => await _service.UpdateAsync(user);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("*Database unavailable*");
    }
}
