namespace PulseHub.Core.Models
{
    public class Subscriptions
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DeviceToken { get; set; }
        public string Channel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
