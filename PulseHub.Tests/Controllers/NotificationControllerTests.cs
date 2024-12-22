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
    }
}
