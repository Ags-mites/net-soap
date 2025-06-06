using System.ServiceModel;
using EnviosExpressAPI.DTOs;

namespace EnviosExpressAPI.Services
{
    [ServiceContract(Namespace = "http://tempuri.org/")]
    public interface ITrackingService
    {
        // Método original con objeto
        [OperationContract(Action = "http://tempuri.org/ITrackingService/GetTrackingStatus")]
        [FaultContract(typeof(TrackingFault))]
        GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request);

        // Método alternativo con parámetro directo
        [OperationContract(Action = "http://tempuri.org/ITrackingService/GetTrackingStatusDirect")]
        [FaultContract(typeof(TrackingFault))]
        GetTrackingStatusResponse GetTrackingStatusDirect(string trackingNumber);

        // Método de test simple
        [OperationContract(Action = "http://tempuri.org/ITrackingService/TestConnection")]
        string TestConnection();
    }
}