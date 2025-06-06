using System.ComponentModel.DataAnnotations;

namespace EnviosExpressAPI.Models
{
    public class Package
    {
        [Key]
        public string TrackingNumber { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public string Dimensions { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CurrentLocation { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<TrackingEvent> History { get; set; } = new List<TrackingEvent>();
    }
}