using PulseHub.Core.DTO;

namespace PulseHub.Core.Services
{
    public class NotificationService : INotificationService
    {
        public Task SendNotificationAsync(string userId, string message, string channel)
        {
            throw new NotImplementedException();
        }

        public bool Subscribe(SubscriptionRequest request)
        {
            throw new NotImplementedException();
        }

        public bool Unsubscribe(string userId, string channel)
        {
            throw new NotImplementedException();
        }
    }
}
