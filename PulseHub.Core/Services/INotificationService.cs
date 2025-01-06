using PulseHub.Core.CustomError;
using PulseHub.Core.DTO;

namespace PulseHub.Core.Services
{
    public interface INotificationService
    {
        Result Subscribe(SubscriptionRequest request);
        Result Unsubscribe(string userId, string channel);
        Task<Result> SendNotificationAsync(string userId, string message, string channel);
        List<SubscriptionRequest> GetSubscriptions(string userId);
        Result UpdateDeviceToken(string userId, string newToken);
        Result ValidateDeviceToken(string deviceToken);
        Result IsSubscribed(string userId, string channel);
    }
}
