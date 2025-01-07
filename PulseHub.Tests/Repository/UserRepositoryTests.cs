using Microsoft.EntityFrameworkCore;
using PulseHub.Core.Models;
using PulseHub.Infrastructure.Data;
using PulseHub.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void AddUser_ShouldAddUserToDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "TestUser",
            Email = "testuser@example.com",
            Password = "hashed-password"
        };

        // Act
        repository.Add(user);
        context.SaveChanges();

        // Assert
        var savedUser = context.Users.FirstOrDefault(u => u.Email == "testuser@example.com");
        Assert.NotNull(savedUser);
        Assert.Equal("TestUser", savedUser.Username);
    }

    [Fact]
    public void GetUserById_ShouldReturnCorrectUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "TestUser",
            Email = "testuser@example.com",
            Password = "hashed-password"
        };

        context.Users.Add(user);
        context.SaveChanges();

        // Act
        var retrievedUser = repository.GetUserById(user.Id);

        // Assert
        Assert.NotNull(retrievedUser);
        Assert.Equal("TestUser", retrievedUser.Result.Username);
    }

    [Fact]
    public void UpdateUser_ShouldModifyUserDetails()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "TestUser",
            Email = "testuser@example.com",
            Password = "hashed-password"
        };

        context.Users.Add(user);
        context.SaveChanges();

        // Act
        user.Username = "UpdatedUser";
        repository.Update(user);
        context.SaveChanges();

        // Assert
        var updatedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
        Assert.NotNull(updatedUser);
        Assert.Equal("UpdatedUser", updatedUser.Username);
    }

    [Fact]
    public void DeleteUser_ShouldRemoveUserFromDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "TestUser",
            Email = "testuser@example.com",
            Password = "hashed-password"
        };

        context.Users.Add(user);
        context.SaveChanges();

        // Act
        repository.Delete(user.Id);
        context.SaveChanges();

        // Assert
        var deletedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
        Assert.Null(deletedUser);
    }
}
