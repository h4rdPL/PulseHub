using PulseHub.Core.DTO;

namespace PulseHub.Core.Services
{
    public interface INotificationService
    {
        bool Subscribe(SubscriptionRequest request);
        bool Unsubscribe(string userId, string channel);
        Task SendNotificationAsync(string userId, string message, string channel);
    }
}
