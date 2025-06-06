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
            _context = context;
            _logger = logger;
        }

        public async Task<GetTrackingStatusResponse> GetTrackingStatus(GetTrackingStatusRequest request)
        {
            try
            {
                _logger.LogInformation("Consultando estado del paquete: {TrackingNumber}", request.TrackingNumber);

                // Validar entrada
                if (string.IsNullOrWhiteSpace(request.TrackingNumber))
                {
                    _logger.LogWarning("Número de tracking vacío o nulo");
                    throw new FaultException<TrackingFault>(
                        new TrackingFault
                        {
                            Error = new TrackingError
                            {
                                ErrorCode = 400,
                                ErrorMessage = "El número de tracking es requerido",
                                InvalidField = nameof(request.TrackingNumber)
                            }
                        },
                        "Número de tracking inválido"
                    );
                }

                // Buscar paquete
                var package = await _context.Packages
                    .Include(p => p.History)
                    .FirstOrDefaultAsync(p => p.TrackingNumber == request.TrackingNumber);

                if (package == null)
                {
                    _logger.LogWarning("Paquete no encontrado: {TrackingNumber}", request.TrackingNumber);
                    throw new FaultException<TrackingFault>(
                        new TrackingFault
                        {
                            Error = new TrackingError
                            {
                                ErrorCode = 404,
                                ErrorMessage = "Paquete no encontrado",
                                InvalidField = nameof(request.TrackingNumber)
                            }
                        },
                        "Paquete no encontrado"
                    );
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

                _logger.LogInformation("Consulta exitosa para el paquete: {TrackingNumber}", request.TrackingNumber);
                return response;
            }
            catch (FaultException)
            {
                throw; // Re-lanzar las excepciones SOAP
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al consultar paquete: {TrackingNumber}", request.TrackingNumber);
                throw new FaultException<TrackingFault>(
                    new TrackingFault
                    {
                        Error = new TrackingError
                        {
                            ErrorCode = 500,
                            ErrorMessage = "Error interno del servidor",
                            InvalidField = null
                        }
                    },
                    "Error interno del servidor"
                );
            }
        }
    }
}