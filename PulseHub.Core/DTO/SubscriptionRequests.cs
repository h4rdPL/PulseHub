namespace PulseHub.Core.DTO
{
    public record SubscriptionRequest(string UserId, string DeviceToken, string Channel);
}
