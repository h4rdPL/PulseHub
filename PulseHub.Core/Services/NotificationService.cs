using System.Collections.Concurrent;
using PulseHub.Core.CustomError;
using PulseHub.Core.DTO;

namespace PulseHub.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ConcurrentDictionary<string, List<SubscriptionRequest>> _subscriptions = new();
        private readonly ConcurrentDictionary<string, string> _deviceTokens = new();

        public List<SubscriptionRequest> GetSubscriptions(string userId)
        {
            return _subscriptions.TryGetValue(userId, out var subscriptions)
                ? subscriptions
                : new List<SubscriptionRequest>();
        }

        public Result IsSubscribed(string userId, string channel)
        {
            if (_subscriptions.TryGetValue(userId, out var userSubscriptions) &&
                userSubscriptions.Any(sub => sub.Channel == channel))
            {
                return Result.Success();
            }

            var error = new Error($"User {userId} is not subscribed to channel {channel}",
                "The subscription could not be validated because the user is not subscribed to the specified channel.");
            return Result.Failure(error);
        }
        public async Task<Result> SendNotificationAsync(string userId, string message, string channel)
        {
            if (!_subscriptions.TryGetValue(userId, out var subscriptions) || !subscriptions.Any(s => s.Channel == channel))
            {
                var error = new Error("NoSubscriptions", "No subscriptions found for user.");
                return Result.Failure(error);
            }

            Console.WriteLine($"Sending '{message}' to {subscriptions.Count} devices for user {userId} on channel {channel}");
            await Task.CompletedTask;
            return Result.Success();
        }




        public Result Subscribe(SubscriptionRequest request)
        {
            if (!_deviceTokens.ContainsKey(request.UserId))
            {
                _deviceTokens[request.UserId] = request.DeviceToken;
            }

            var userSubscriptions = _subscriptions.GetOrAdd(request.UserId, _ => new List<SubscriptionRequest>());
            if (userSubscriptions.Any(s => s.Channel == request.Channel && s.DeviceToken == request.DeviceToken))
            {
                var error = new Error("Already subscribed to this channel with this device.",
                    "Subscription request failed because the device is already subscribed to the channel.");
                return Result.Failure(error);
            }

            userSubscriptions.Add(request);
            return Result.Success();
        }

        public Result Unsubscribe(string userId, string channel)
        {
            if (!_subscriptions.ContainsKey(userId))
            {
                var error = new Error($"No subscriptions found for user {userId}",
                    "Unsubscription failed because the user has no subscriptions.");
                return Result.Failure(error);
            }

            var userSubscriptions = _subscriptions[userId];
            var removed = userSubscriptions.RemoveAll(s => s.Channel == channel) > 0;

            if (!userSubscriptions.Any())
            {
                _subscriptions.TryRemove(userId, out _);
            }

            return removed
                ? Result.Success()
                : Result.Failure(new Error($"No subscriptions for channel {channel} found for user {userId}",
                    "Unsubscription failed because no subscriptions exist for the specified channel."));
        }

        public Result UpdateDeviceToken(string userId, string newToken)
        {
            if (!_deviceTokens.ContainsKey(userId))
            {
                var error = new Error($"No device token found for user {userId}",
                    "Update failed because no device token is associated with the user.");
                return Result.Failure(error);
            }

            _deviceTokens[userId] = newToken;

            if (_subscriptions.TryGetValue(userId, out var userSubscriptions))
            {
                for (int i = 0; i < userSubscriptions.Count; i++)
                {
                    userSubscriptions[i] = userSubscriptions[i].WithUpdatedDeviceToken(newToken);
                }
            }

            return Result.Success();
        }

        public Result ValidateDeviceToken(string deviceToken)
        {
            return _deviceTokens.Values.Contains(deviceToken)
                ? Result.Success()
                : Result.Failure(new Error("Invalid device token",
                    "The provided device token does not match any existing tokens."));
        }
    }
}
