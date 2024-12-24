using Xunit;
using Microsoft.AspNetCore.Mvc;
using PulseHub.Core.Services;
using Moq;
using PulseHub.API.Controllers;
using PulseHub.Core.DTO;

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
                .Returns(true);

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
                .Returns(false);

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
                .Returns(false);

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

            mockService
                .Setup(s => s.SendNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new System.Exception("Notification failed"));

            // Act
            var result = await controller.SendNotificationAsync(userId, message, channel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Notification failed", badRequestResult.Value);
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

            string userId = "user-123";
            string newToken = "new-device-token";

            mockService
                .Setup(s => s.UpdateDeviceToken(userId, newToken))
                .Returns(true);

            // Act
            var result = controller.UpdateDeviceToken(userId, newToken);

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

            string userId = "user-123";
            string newToken = "new-device-token";

            mockService
                .Setup(s => s.UpdateDeviceToken(userId, newToken))
                .Returns(false);

            // Act
            var result = controller.UpdateDeviceToken(userId, newToken);

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
                .Returns(true);

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
                .Returns(false);

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
            Assert.Equal("Subscription failed", badRequestResult.Value);
        }

        [Fact]
        public void Subscribe_ShouldReturnBadRequest_WhenRequestHasEmptyFields()
        {
            // Arrange
            var mockService = new Mock<INotificationService>();
            var controller = new NotificationController(mockService.Object);

            var invalidSubscriptionRequest = new SubscriptionRequest("", "", "");

            mockService
                .Setup(s => s.Subscribe(It.IsAny<SubscriptionRequest>()))
                .Returns(false);

            // Act
            var result = controller.Subscribe(invalidSubscriptionRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Subscription failed", badRequestResult.Value);
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
            Assert.Equal("Subscription failed", badRequestResult.Value);
        }





    }
}
