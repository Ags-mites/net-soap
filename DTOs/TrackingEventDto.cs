using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract(Namespace = "http://tempuri.org/")]
    [XmlRoot(ElementName = "TrackingEventDto", Namespace = "http://tempuri.org/")]
    public class TrackingEventDto
    {
        [DataMember(Name = "Date", Order = 1)]
        [XmlElement("Date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "Description", Order = 2)]
        [XmlElement("Description")]
        public string Description { get; set; } = string.Empty;

        [DataMember(Name = "Location", Order = 3)]
        [XmlElement("Location")]
        public string Location { get; set; } = string.Empty;
    }
}