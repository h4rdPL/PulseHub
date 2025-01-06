using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseHub.Core.DTO;
using PulseHub.Core.Services;

namespace PulseHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost("subscribe")]
        public IActionResult Subscribe(SubscriptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: SubscriptionRequest cannot be null");
            }

            var result = _notificationService.Subscribe(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error.Code); // Zwrot kodu błędu w przypadku niepowodzenia
            }

            return Ok("Subscription successful");
        }




        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe(string userId, string channel)
        {
            var result = _notificationService.Unsubscribe(userId, channel);
            return result.IsSuccess ? Ok("Unsubscription successful") : BadRequest(result.Error?.Description ?? "Unsubscription failed");
        }


        [HttpGet("subscriptions/{userId}")]
        public IActionResult GetSubscriptions(string userId)
        {
            var subscriptions = _notificationService.GetSubscriptions(userId);
            return Ok(subscriptions);
        }

        [HttpPost("update-device-token")]
        public IActionResult UpdateDeviceToken([FromBody] UpdateDeviceTokenRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.NewToken))
            {
                return BadRequest("Invalid request: UserId and NewToken cannot be null or empty");
            }

            var result = _notificationService.UpdateDeviceToken(request.UserId, request.NewToken);
            return result.IsSuccess ? Ok("Device token updated successfully") : BadRequest(result.Error?.Description ?? "Failed to update device token");
        }


        [HttpPost("is-subscribed")]
        public IActionResult IsSubscribed(string userId, string channel)
        {
            var result = _notificationService.IsSubscribed(userId, channel);
            return result.IsSuccess
                ? Ok("User is subscribed to the channel")
                : BadRequest(result.Error?.Description ?? "User is not subscribed to the channel");
        }


        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotificationAsync(string userId, string message, string channel)
        {
            var result = await _notificationService.SendNotificationAsync(userId, message, channel);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok("Notification sent successfully");
        }

    }
}
