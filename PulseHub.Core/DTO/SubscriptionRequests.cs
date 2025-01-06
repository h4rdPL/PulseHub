namespace PulseHub.Core.DTO
{
    public record SubscriptionRequest(string UserId, string DeviceToken, string Channel)
    {
        public SubscriptionRequest WithUpdatedDeviceToken(string newToken)
        {
            return this with { DeviceToken = newToken };
        }
    }
}
