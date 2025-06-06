using System.ComponentModel.DataAnnotations;

namespace EnviosExpressAPI.Models
{
    public class TrackingEvent
    {
        [Key]
        public int Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        public virtual Package Package { get; set; } = null!;
    }
}