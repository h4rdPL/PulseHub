using System.Collections.Concurrent;
using PulseHub.Core.DTO;

namespace PulseHub.Core.Services
{
    public class NotificationService : INotificationService
    {
        // In-memory storage for subscriptions
        private readonly ConcurrentDictionary<string, List<SubscriptionRequest>> _subscriptions = new();
        private readonly ConcurrentDictionary<string, string> _deviceTokens = new();

        public List<SubscriptionRequest> GetSubscriptions(string userId)
        {
            if (_subscriptions.TryGetValue(userId, out var subscriptions))
            {
                return subscriptions;
            }
            return new List<SubscriptionRequest>();
        }

        public bool IsSubscribed(string userId, string channel)
        {
            if (_subscriptions.TryGetValue(userId, out var userSubscriptions))
            {
                return userSubscriptions.Any(sub => sub.Channel == channel);
            }
            return false;
        }

        public Task SendNotificationAsync(string userId, string message, string channel)
        {
            if (!_subscriptions.ContainsKey(userId))
            {
                throw new InvalidOperationException($"No subscriptions found for user {userId}");
            }

            var subscriptions = _subscriptions[userId]
                .Where(s => s.Channel == channel)
                .ToList();

            if (!subscriptions.Any())
            {
                throw new InvalidOperationException($"No subscriptions for channel {channel} for user {userId}");
            }

            // Simulate sending notification
            Console.WriteLine($"Sending '{message}' to {subscriptions.Count} devices for user {userId} on channel {channel}");
            return Task.CompletedTask;
        }

        public bool Subscribe(SubscriptionRequest request)
        {
            if (!_deviceTokens.ContainsKey(request.UserId))
            {
                _deviceTokens[request.UserId] = request.DeviceToken;
            }

            var userSubscriptions = _subscriptions.GetOrAdd(request.UserId, _ => new List<SubscriptionRequest>());
            if (userSubscriptions.Any(s => s.Channel == request.Channel && s.DeviceToken == request.DeviceToken))
            {
                return false; 
            }

            userSubscriptions.Add(request);
            return true;
        }

        public bool Unsubscribe(string userId, string channel)
        {
            if (!_subscriptions.ContainsKey(userId))
            {
                return false;
            }

            var userSubscriptions = _subscriptions[userId];
            var removed = userSubscriptions.RemoveAll(s => s.Channel == channel) > 0;

            if (!userSubscriptions.Any())
            {
                _subscriptions.TryRemove(userId, out _);
            }

            return removed;
        }

        public bool UpdateDeviceToken(string userId, string newToken)
        {
            if (!_deviceTokens.ContainsKey(userId))
            {
                return false;
            }

            _deviceTokens[userId] = newToken;

            if (_subscriptions.TryGetValue(userId, out var userSubscriptions))
            {
                for (int i = 0; i < userSubscriptions.Count; i++)
                {
                    userSubscriptions[i] = userSubscriptions[i].WithUpdatedDeviceToken(newToken);
                }
            }

            return true;
        }


        public bool ValidateDeviceToken(string deviceToken)
        {
            return _deviceTokens.Values.Contains(deviceToken);
        }
    }
}
