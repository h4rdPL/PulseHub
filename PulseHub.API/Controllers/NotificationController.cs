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

        [HttpPost("subscribe")]
        public IActionResult Subscribe(SubscriptionRequest request)
        {
            if (_notificationService.Subscribe(request))
            {
                return Ok("Subscription successful");
            }

            return BadRequest("Subscription failed");
        }
    }
}
