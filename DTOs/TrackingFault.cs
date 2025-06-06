using System.Runtime.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract]
    public class TrackingFault
    {
        [DataMember]
        public TrackingError Error { get; set; } = new TrackingError();
    }
}