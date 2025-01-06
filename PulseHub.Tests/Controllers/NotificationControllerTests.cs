using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseHub.API.Controllers;
using PulseHub.Core.CustomError;
using PulseHub.Core.DTO;
using PulseHub.Core.Services;

namespace PulseHub.Tests
{
    public class NotificationControllerTests
    {
        [Fact]
        public void Subscribe_ShouldReturnOk_WhenSubscriptionIsSuccessful()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var subscriptionRequest = new SubscriptionRequest("user-123", "device-token-abc", "Alerts");

            mockService
                .Setup(s => s.Subscribe(It.IsAny<SubscriptionRequest>()))
                .Returns(Result.Success());

            // Act
            var result = controller.Subscribe(subscriptionRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Subscription successful", okResult.Value);
        }


        [Fact]
        public void Subscribe_ShouldReturnBadRequest_WhenSubscriptionFails()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var subscriptionRequest = new SubscriptionRequest("user-123", "device-token-abc", "Alerts");

            mockService
                .Setup(s => s.Subscribe(It.IsAny<SubscriptionRequest>()))
                .Returns(Result.Failure(new Error("Subscription failed", "The subscription could not be processed due to a service error.")));

            // Act
            var result = controller.Subscribe(subscriptionRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Subscription failed", badRequestResult.Value);

        }

        [Fact]
        public void Unsubscribe_ShouldReturnBadRequest_WhenUnsubscribeFails()
        {
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            string userId = "user-123";
            string channel = "Alerts";

            mockService
                .Setup(s => s.Unsubscribe(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Result.Failure(new Error("400", "Unsubscription failed")));

            var result = controller.Unsubscribe(userId, channel);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unsubscription failed", badRequestResult.Value);
        }

        [Fact]
        public async Task SendNotificationAsync_ShouldReturnBadRequest_WhenNotificationFails()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            string userId = "user-123";
            string message = "Test notification";
            string channel = "Alerts";

            var expectedError = new Error("NoSubscriptions", "No subscriptions found for user.");

            mockService
                .Setup(s => s.SendNotificationAsync(userId, message, channel))
                .ReturnsAsync(Result.Failure(expectedError));

            // Act
            var result = await controller.SendNotificationAsync(userId, message, channel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            var actualError = Assert.IsType<Error>(badRequestResult.Value);
            Assert.Equal(expectedError.Code, actualError.Code);
            Assert.Equal(expectedError.Description, actualError.Description);
        }





        [Fact]
        public void GetSubscriptions_ShouldReturnListOfSubscriptions_WhenUserExists()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            string userId = "user-123";
            var subscriptions = new List<SubscriptionRequest>
            {
                new SubscriptionRequest("user-123", "device-token-abc", "Alerts"),
                new SubscriptionRequest("user-123", "device-token-def", "News")
            };

            mockService
                .Setup(s => s.GetSubscriptions(userId))
                .Returns(subscriptions);

            // Act
            var result = controller.GetSubscriptions(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSubscriptions = Assert.IsType<List<SubscriptionRequest>>(okResult.Value);
            Assert.Equal(2, returnedSubscriptions.Count);
        }

        [Fact]
        public void UpdateDeviceToken_ShouldReturnOk_WhenTokenIsUpdatedSuccessfully()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var request = new UpdateDeviceTokenRequest("user-123", "new-device-token");

            mockService
                .Setup(s => s.UpdateDeviceToken(request.UserId, request.NewToken))
                .Returns(Result.Success());

            // Act
            var result = controller.UpdateDeviceToken(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Device token updated successfully", okResult.Value);
        }

        [Fact]
        public void UpdateDeviceToken_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var request = new UpdateDeviceTokenRequest("user-123", "Device-token-updated");
            mockService
                .Setup(s => s.UpdateDeviceToken(request.UserId, request.NewToken))
                .Returns(Result.Failure(new Error("400", "Failed to update device token")));

            // Act
            var result = controller.UpdateDeviceToken(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update device token", badRequestResult.Value);
        }


        [Fact]
        public void IsSubscribed_ShouldReturnTrue_WhenUserIsSubscribedToChannel()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            string userId = "user-123";
            string channel = "Alerts";

            mockService
                .Setup(s => s.IsSubscribed(userId, channel))
                .Returns(Result.Success());

            // Act
            var result = controller.IsSubscribed(userId, channel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User is subscribed to the channel", okResult.Value);
        }


        [Fact]
        public void IsSubscribed_ShouldReturnFalse_WhenUserIsNotSubscribedToChannel()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            string userId = "user-123";
            string channel = "Alerts";

            mockService
                .Setup(s => s.IsSubscribed(userId, channel))
                .Returns(Result.Failure(new Error("404", "User is not subscribed to the channel")));

            // Act
            var result = controller.IsSubscribed(userId, channel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User is not subscribed to the channel", badRequestResult.Value);
        }


        [Fact]
        public void Subscribe_ShouldReturnBadRequest_WhenSubscriptionRequestIsNull()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            SubscriptionRequest subscriptionRequest = null;

            // Act
            var result = controller.Subscribe(subscriptionRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request: SubscriptionRequest cannot be null", badRequestResult.Value);
        }


        [Fact]
        public void Subscribe_ShouldReturnBadRequest_WhenRequestHasEmptyFields()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var invalidSubscriptionRequest = new SubscriptionRequest("", "", "");

            // Act
            var result = controller.Subscribe(invalidSubscriptionRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid request: UserId, DeviceToken, and Channel cannot be null or empty", badRequestResult.Value);
        }

        [Fact]
        public void Subscribe_ShouldHandleServiceException_Gracefully()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var subscriptionRequest = new SubscriptionRequest("user-123", "device-token-abc", "Alerts");

            mockService
                .Setup(s => s.Subscribe(It.IsAny<SubscriptionRequest>()))
                .Throws(new Exception("Unexpected error"));

            // Act
            var result = controller.Subscribe(subscriptionRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestValue = badRequestResult.Value as Dictionary<string, string>;
            Assert.NotNull(badRequestValue);
            Assert.Equal("Unexpected error", badRequestValue["error"]);
        }



    }
}
