using System.ServiceModel;
using EnviosExpressAPI.DTOs;

namespace EnviosExpressAPI.Services
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface ITrackingService
    {
        [OperationContract(Action = "http://tempuri.org/ITrackingService/GetTrackingStatus")]
        [FaultContract(typeof(TrackingFault))]
        GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request);

        [OperationContract(Action = "http://tempuri.org/ITrackingService/GetTrackingStatusDirect")]
        [FaultContract(typeof(TrackingFault))]
        GetTrackingStatusResponse GetTrackingStatusDirect(string trackingNumber);
    }
}