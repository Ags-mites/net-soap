using System.ServiceModel;
using EnviosExpressAPI.DTOs;

namespace EnviosExpressAPI.Services
{
    [ServiceContract]
    public interface ITrackingService
    {
        [OperationContract]
        [FaultContract(typeof(TrackingFault))]
        GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request);
    }
}