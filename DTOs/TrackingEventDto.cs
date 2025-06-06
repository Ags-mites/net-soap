using System.Runtime.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract]
    public class TrackingEventDto
    {
        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Description { get; set; } = string.Empty;

        [DataMember]
        public string Location { get; set; } = string.Empty;
    }
}