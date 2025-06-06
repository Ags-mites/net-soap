using System.Runtime.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract]
    public class GetTrackingStatusRequest
    {
        [DataMember]
        public string TrackingNumber { get; set; } = string.Empty;
    }
}