namespace PulseHub.Core.DTO
{
    public record UpdateDeviceTokenRequest(
            string UserId,
            string NewToken
        );
}
