using System.Runtime.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract]
    public class GetTrackingStatusResponse
    {
        [DataMember]
        public string Status { get; set; } = string.Empty;

        [DataMember]
        public string CurrentLocation { get; set; } = string.Empty;

        [DataMember]
        public DateTime? EstimatedDeliveryDate { get; set; }

        [DataMember]
        public List<TrackingEventDto> History { get; set; } = new List<TrackingEventDto>();
    }
}