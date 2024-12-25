using Microsoft.AspNetCore.Mvc;
using PulseHub.Core.DTO;
using PulseHub.Core.Services;

namespace PulseHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notification)
        {
            _notificationService = notification;
        }



        [HttpPost("send")]
        public async Task<IActionResult> SendNotificationAsync(string userId, string message, string channel)
        {
            try
            {
                await _notificationService.SendNotificationAsync(userId, message, channel);
                return Ok("Notification sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("subscribe")]
        public IActionResult Subscribe(SubscriptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: SubscriptionRequest cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.UserId) ||
                string.IsNullOrWhiteSpace(request.DeviceToken) ||
                string.IsNullOrWhiteSpace(request.Channel))
            {
                return BadRequest("Invalid request: UserId, DeviceToken, and Channel cannot be null or empty");
            }

            try
            {
                if (_notificationService.Subscribe(request))
                {
                    return Ok("Subscription successful");
                }

                return BadRequest("Subscription failed");
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                return BadRequest("Subscription failed");
            }
        }


        [HttpGet("subscriptions/{userId}")]
        public IActionResult GetSubscriptions(string userId)
        {
            var subscriptions = _notificationService.GetSubscriptions(userId);
            if (subscriptions == null || subscriptions.Count == 0)
            {
                return NotFound("No subscriptions found for the user");
            }
            return Ok(subscriptions);
        }


        [HttpPost("validate-token")]
        public IActionResult ValidateDeviceToken([FromBody] string deviceToken)
        {
            if (_notificationService.ValidateDeviceToken(deviceToken))
            {
                return Ok("Token is valid");
            }
            return BadRequest("Invalid device token");
        }

        [HttpPut("update-token")]
        public IActionResult UpdateDeviceToken(string userId, string newToken)
        {
            if (_notificationService.UpdateDeviceToken(userId, newToken))
            {
                return Ok("Device token updated successfully");
            }
            return BadRequest("Failed to update device token");
        }


        [HttpGet("is-subscribed/{userId}/{channel}")]
        public IActionResult IsSubscribed(string userId, string channel)
        {
            if (_notificationService.IsSubscribed(userId, channel))
            {
                return Ok("User is subscribed to the channel");
            }
            return BadRequest("User is not subscribed to the channel");
        }

        [HttpPost("unsubscribe")]
        public IActionResult Unsubscribe(string userId, string channel)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(channel))
            {
                return BadRequest("Invalid input: UserId or channel cannot be null or empty");
            }

            try
            {
                if (_notificationService.Unsubscribe(userId, channel))
                {
                    return Ok("Unsubscription successful");
                }
                return BadRequest("Unsubscription failed");
            }
            catch (Exception ex)
            {
                return BadRequest($"Unsubscription failed due to an error: {ex.Message}");
            }
        }
    }

}
