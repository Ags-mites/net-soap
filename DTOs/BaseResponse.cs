using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "BaseResponse", Namespace = "http://tempuri.org/")]
    public class BaseResponse
    {
        [DataMember(Name = "Success", Order = 1)]
        [XmlElement("Success")]
        public bool Success { get; set; } = true;

        [DataMember(Name = "Message", Order = 2)]
        [XmlElement("Message")]
        public string Message { get; set; } = "Operation completed successfully";

        [DataMember(Name = "Timestamp", Order = 3)]
        [XmlElement("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "TrackingResponse", Namespace = "http://tempuri.org/")]
    public class TrackingResponse : BaseResponse
    {
        [DataMember(Name = "Data", Order = 4)]
        [XmlElement("Data")]
        public GetTrackingStatusResponse? Data { get; set; }

        [DataMember(Name = "Error", Order = 5)]
        [XmlElement("Error")]
        public ErrorResponse? Error { get; set; }
    }
}