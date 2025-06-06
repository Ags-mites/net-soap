using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "GetTrackingStatusRequest", Namespace = "http://tempuri.org/")]
    public class GetTrackingStatusRequest
    {
        [DataMember(Name = "TrackingNumber", Order = 1)]
        [XmlElement("TrackingNumber")]
        public string TrackingNumber { get; set; } = string.Empty;
    }
}