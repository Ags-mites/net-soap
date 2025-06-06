using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "GetTrackingStatusResponse", Namespace = "http://tempuri.org/")]
    public class GetTrackingStatusResponse
    {
        [DataMember(Name = "Status", Order = 1)]
        [XmlElement("Status")]
        public string Status { get; set; } = string.Empty;

        [DataMember(Name = "CurrentLocation", Order = 2)]
        [XmlElement("CurrentLocation")]
        public string CurrentLocation { get; set; } = string.Empty;

        [DataMember(Name = "EstimatedDeliveryDate", Order = 3)]
        [XmlElement("EstimatedDeliveryDate")]
        public DateTime? EstimatedDeliveryDate { get; set; }

        [DataMember(Name = "History", Order = 4)]
        [XmlArray("History")]
        [XmlArrayItem("TrackingEventDto")]
        public List<TrackingEventDto> History { get; set; } = new List<TrackingEventDto>();
    }
}