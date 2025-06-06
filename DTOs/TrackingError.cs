using System.Runtime.Serialization;

namespace EnviosExpressAPI.DTOs
{
    [DataContract]
    public class TrackingError
    {
        [DataMember]
        public int ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; } = string.Empty;

        [DataMember]
        public string? InvalidField { get; set; }
    }
}