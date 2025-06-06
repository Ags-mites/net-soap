using Microsoft.EntityFrameworkCore;
using System.ServiceModel;
using EnviosExpressAPI.Data;
using EnviosExpressAPI.DTOs;

namespace EnviosExpressAPI.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TrackingService> _logger;

        public TrackingService(ApplicationDbContext context, ILogger<TrackingService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Consultando estado del paquete: {TrackingNumber}", request?.TrackingNumber);

                // Validar entrada
                if (request == null)
                {
                    _logger.LogWarning("Request es nulo");
                    throw CreateTrackingFault(400, "La solicitud es requerida", null);
                }

                if (string.IsNullOrWhiteSpace(request.TrackingNumber))
                {
                    _logger.LogWarning("Número de tracking vacío o nulo");
                    throw CreateTrackingFault(400, "El número de tracking es requerido", nameof(request.TrackingNumber));
                }

                // Normalizar el número de tracking
                var trackingNumber = request.TrackingNumber.Trim().ToUpperInvariant();

                // Validar formato básico del tracking number
                if (trackingNumber.Length < 5 || trackingNumber.Length > 50)
                {
                    _logger.LogWarning("Formato de tracking number inválido: {TrackingNumber}", trackingNumber);
                    throw CreateTrackingFault(400, "El formato del número de tracking es inválido", nameof(request.TrackingNumber));
                }

                // Buscar paquete
                var package = _context.Packages
                    .Include(p => p.History)
                    .FirstOrDefault(p => p.TrackingNumber == trackingNumber);

                if (package == null)
                {
                    _logger.LogWarning("Paquete no encontrado: {TrackingNumber}", trackingNumber);
                    throw CreateTrackingFault(404, "Paquete no encontrado", nameof(request.TrackingNumber));
                }

                // Mapear respuesta
                var response = new GetTrackingStatusResponse
                {
                    Status = package.Status,
                    CurrentLocation = package.CurrentLocation,
                    EstimatedDeliveryDate = package.EstimatedDeliveryDate,
                    History = package.History
                        .OrderBy(h => h.Date)
                        .Select(h => new TrackingEventDto
                        {
                            Date = h.Date,
                            Description = h.Description,
                            Location = h.Location
                        })
                        .ToList()
                };

                _logger.LogInformation("Consulta exitosa para el paquete: {TrackingNumber}, Estado: {Status}",
                    trackingNumber, package.Status);

                return response;
            }
            catch (FaultException)
            {
                throw; // Re-lanzar las excepciones SOAP
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al consultar paquete: {TrackingNumber}", request?.TrackingNumber);
                throw CreateTrackingFault(500, "Error interno del servidor", null);
            }
        }

        private static FaultException<TrackingFault> CreateTrackingFault(int errorCode, string errorMessage, string? invalidField)
        {
            return new FaultException<TrackingFault>(
                new TrackingFault
                {
                    Error = new TrackingError
                    {
                        ErrorCode = errorCode,
                        ErrorMessage = errorMessage,
                        InvalidField = invalidField
                    }
                },
                new FaultReason(errorMessage)
            );
        }
    }
}