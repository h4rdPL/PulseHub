using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseHub.API.Controllers;
using PulseHub.Core.CustomError;
using PulseHub.Core.Models;
using PulseHub.Infrastructure.Repositories;

namespace PulseHub.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new UserController(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            var user = new User { Id = userId, UserName = "testuser", Email = "test@example.com" };
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
            Assert.Equal("testuser", returnedUser.UserName);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent-user";
            _mockUserRepository.Setup(repo => repo.GetUserById(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task AddUser_ShouldReturnCreated_WhenUserIsValid()
        {
            // Arrange
            var user = new User { Id = "user-123", UserName = "newuser", Email = "new@example.com" };

            // Act
            var result = await _controller.AddUser(user);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(user.Id, createdUser.Id);
            Assert.Equal("newuser", createdUser.UserName);
        }

        [Fact]
        public async Task AddUser_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Act
            var result = await _controller.AddUser(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User cannot be null", badRequestResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldSetIsActiveToTrue_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            _mockUserRepository.Setup(repo => repo.ConfirmEmail(userId)).ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.ConfirmEmail(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent-user";
            _mockUserRepository.Setup(repo => repo.ConfirmEmail(userId)).ReturnsAsync(Result.Failure(new Error("NotFound", "User not found")));

            // Act
            var result = await _controller.ConfirmEmail(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", ((Error)((NotFoundObjectResult)result).Value).Description);
        }


        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            _mockUserRepository.Setup(repo => repo.Delete(userId));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent-user";
            _mockUserRepository.Setup(repo => repo.Delete(userId)).Throws(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdated()
        {
            // Arrange
            var user = new User { Id = "user-123", UserName = "updateduser", Email = "updated@example.com" };

            _mockUserRepository.Setup(repo => repo.Update(user));

            // Act
            var result = await _controller.UpdateUser(user);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new User { Id = "nonexistent-user", UserName = "updateduser", Email = "updated@example.com" };
            _mockUserRepository.Setup(repo => repo.Update(user)).Throws(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.UpdateUser(user);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserIsNull()
        {
            // Act
            var result = await _controller.UpdateUser(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User cannot be null", badRequestResult.Value);
        }
        [Fact]
        public async Task DeactivateUser_ShouldReturnSuccessResult_WhenUserDeactivatedSuccessfully()
        {
            // Arrange
            var userId = "user-123";
            _mockUserRepository.Setup(repo => repo.DeactivateUser(userId))
                .ReturnsAsync(Result.Success());

            // Act
            var result = await _controller.DeactivateUser(userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent-user";
            _mockUserRepository.Setup(repo => repo.DeactivateUser(userId)).Throws(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.DeactivateUser(userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("NotFound", result.Error.Code);
            Assert.Equal("User not found", result.Error.Description);
        }
    }
}
