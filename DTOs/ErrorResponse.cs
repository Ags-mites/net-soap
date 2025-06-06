using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "ErrorResponse", Namespace = "http://tempuri.org/")]
    public class ErrorResponse
    {
        [DataMember(Name = "Success", Order = 1)]
        [XmlElement("Success")]
        public bool Success { get; set; } = false;

        [DataMember(Name = "ErrorCode", Order = 2)]
        [XmlElement("ErrorCode")]
        public string ErrorCode { get; set; } = string.Empty;

        [DataMember(Name = "ErrorMessage", Order = 3)]
        [XmlElement("ErrorMessage")]
        public string ErrorMessage { get; set; } = string.Empty;

        [DataMember(Name = "Details", Order = 4)]
        [XmlElement("Details")]
        public string? Details { get; set; }

        [DataMember(Name = "Timestamp", Order = 5)]
        [XmlElement("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}