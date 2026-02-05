using UserService.Domain.Entities;
using UserService.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UserService.Tests.Infrastructure;

/// <summary>
/// Tests for <see cref="UserDbContext"/> verifying auditing and soft delete logic.
/// Uses EF Core InMemory provider.
/// </summary>
public class UserDbContextTests
{
    private static UserDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // isolate each test
            .Options;

        return new UserDbContext(options);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSetAuditFields_OnInsert()
    {
        // Arrange
        var db = CreateContext();
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };

        // Act
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Assert
        user.CreatedAt.Should().NotBe(default);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldUpdateTimestamp_OnModify()
    {
        // Arrange
        var db = CreateContext();
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var oldUpdatedAt = user.UpdatedAt;

        // Act
        user.FullName = "John Updated";
        await db.SaveChangesAsync();

        // Assert
        user.UpdatedAt.Should().BeAfter(oldUpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPerformSoftDelete()
    {
        // Arrange
        var db = CreateContext();
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        // Act
        db.Users.Remove(user);
        await db.SaveChangesAsync();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

        db.Entry(user).State.Should().Be(EntityState.Unchanged);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldNotAffectUnchangedEntities()
    {
        // Arrange
        var db = CreateContext();
        var user = new User { Email = "john@doe.com", FullName = "John Doe" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var originalCreated = user.CreatedAt;
        var originalUpdated = user.UpdatedAt;

        // Act
        await db.SaveChangesAsync(); // no changes

        // Assert
        user.CreatedAt.Should().Be(originalCreated);
        user.UpdatedAt.Should().Be(originalUpdated);
    }
}
