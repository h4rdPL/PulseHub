using Microsoft.AspNetCore.SignalR;
using Moq;
using PulseHub.API.Hubs;
using System.Security.Claims;

namespace PulseHub.Tests.Hubs
{
    public class NotificationHubTests
    {
        private readonly Mock<IHubCallerClients> _mockClients;
        private readonly Mock<IGroupManager> _mockGroups;
        private readonly Mock<IClientProxy> _mockClientProxy;
        private readonly Mock<HubCallerContext> _mockContext;
        private readonly NotificationHub _hub;

        public NotificationHubTests()
        {
            _mockClients = new Mock<IHubCallerClients>();
            _mockClientProxy = new Mock<IClientProxy>();
            _mockGroups = new Mock<IGroupManager>();
            _mockContext = new Mock<HubCallerContext>();

            _mockClients
                .Setup(clients => clients.User(It.IsAny<string>()))
                .Returns(_mockClientProxy.Object);

            _mockContext.Setup(context => context.ConnectionId).Returns("TestConnectionId");

            _mockContext.Setup(context => context.User)
                        .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "user-123") })));

            _hub = new NotificationHub
            {
                Clients = _mockClients.Object,
                Context = _mockContext.Object,
                Groups = _mockGroups.Object
            };
        }


        [Fact]
        public async Task SendNotification_ShouldSendMessageToCorrectUser()
        {
            // Arrange
            string userId = "user-123";
            string message = "Test notification";

            var mockClientProxy = new Mock<IClientProxy>();

            _mockClients.Setup(clients => clients.User(userId))
                        .Returns(mockClientProxy.Object);

            mockClientProxy.Setup(client => client.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask)
                           .Callback<string, object[], CancellationToken>((method, args, cancellationToken) =>
                           {
                               Assert.Equal("ReceiveNotification", method);
                               Assert.Equal(message, args[0]);
                           });

            // Act
            await _hub.SendNotification(userId, message);

            // Assert
            _mockClients.Verify(clients => clients.User(userId), Times.Once);

            mockClientProxy.Verify(client => client.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }







        [Fact]
        public async Task OnConnectedAsync_ShouldAddUserToGroup_WhenUserIdIsValid()
        {
            // Arrange
            string userId = "user-123";
            string connectionId = "connection-abc";

            _mockContext.Setup(context => context.ConnectionId).Returns(connectionId);

            // Act
            await _hub.OnConnectedAsync();

            // Assert: 
            _mockGroups.Verify(groups => groups.AddToGroupAsync(connectionId, userId, default), Times.Once);
        }

        [Fact]
        public async Task OnDisconnectedAsync_ShouldRemoveUserFromGroup_WhenUserIdIsValid()
        {
            // Arrange
            string userId = "user-123";
            string connectionId = "connection-abc";

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userId) }));
            _mockContext.Setup(context => context.User).Returns(claimsPrincipal);
            _mockContext.Setup(context => context.ConnectionId).Returns(connectionId);

            _mockGroups.Setup(groups => groups.RemoveFromGroupAsync(connectionId, userId, default))
                       .Returns(Task.CompletedTask);

            // Act
            await _hub.OnDisconnectedAsync(null);

            // Assert
            _mockGroups.Verify(groups => groups.RemoveFromGroupAsync(connectionId, userId, default), Times.Once);
        }


        [Fact]
        public async Task OnConnectedAsync_ShouldNotAddUserToGroup_WhenUserIdIsNull()
        {
            // Arrange
            _mockContext.Setup(context => context.User).Returns((ClaimsPrincipal)null);

            // Act
            await _hub.OnConnectedAsync();

            // Assert
            _mockGroups.Verify(groups => groups.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
        }

        [Fact]
        public async Task OnDisconnectedAsync_ShouldNotRemoveUserFromGroup_WhenUserIdIsNull()
        {
            // Arrange
            _mockContext.Setup(context => context.User).Returns((ClaimsPrincipal)null);

            // Act
            await _hub.OnDisconnectedAsync(null);

            // Assert
            _mockGroups.Verify(groups => groups.RemoveFromGroupAsync(It.IsAny<string>(), It.IsAny<string>(), default), Times.Never);
        }
    }
}
