using Microsoft.EntityFrameworkCore;
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

        public GetTrackingStatusResponse GetTrackingStatusDirect(string trackingNumber)
        {
            return ProcessTrackingRequest(trackingNumber);
        }

        public GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request)
        {
            if (request == null)
            {
                return CreateErrorResponse("INVALID_REQUEST", "La solicitud es requerida");
            }

            return ProcessTrackingRequest(request.TrackingNumber);
        }

        private GetTrackingStatusResponse ProcessTrackingRequest(string trackingNumber)
        {
            try
            {
                if (!IsValidTrackingNumber(trackingNumber, out string errorMessage))
                {
                    return CreateErrorResponse("VALIDATION_ERROR", errorMessage);
                }

                var normalizedTrackingNumber = trackingNumber.Trim().ToUpperInvariant();
                var package = FindPackage(normalizedTrackingNumber);

                if (package == null)
                {
                    return CreateErrorResponse("PACKAGE_NOT_FOUND",
                        "El paquete con el número de tracking especificado no fue encontrado");
                }

                return CreateSuccessResponse(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando tracking: {TrackingNumber}", trackingNumber);
                return CreateErrorResponse("INTERNAL_ERROR", "Error interno del servidor");
            }
        }

        private bool IsValidTrackingNumber(string trackingNumber, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                errorMessage = "El número de tracking es requerido";
                return false;
            }

            var trimmed = trackingNumber.Trim();
            if (trimmed.Length < 5)
            {
                errorMessage = "El número de tracking debe tener al menos 5 caracteres";
                return false;
            }

            if (trimmed.Length > 50)
            {
                errorMessage = "El número de tracking no puede exceder 50 caracteres";
                return false;
            }

            return true;
        }

        private Models.Package? FindPackage(string normalizedTrackingNumber)
        {
            return _context.Packages
                .Include(p => p.History)
                .FirstOrDefault(p => p.TrackingNumber == normalizedTrackingNumber);
        }

        private GetTrackingStatusResponse CreateSuccessResponse(Models.Package package)
        {
            return new GetTrackingStatusResponse
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
        }

        private GetTrackingStatusResponse CreateErrorResponse(string errorCode, string errorMessage)
        {
            return new GetTrackingStatusResponse
            {
                Status = "ERROR",
                CurrentLocation = $"Error: {errorCode}",
                EstimatedDeliveryDate = null,
                History = new List<TrackingEventDto>
                {
                    new TrackingEventDto
                    {
                        Date = DateTime.UtcNow,
                        Description = $"ERROR [{errorCode}]: {errorMessage}",
                        Location = "Sistema"
                    }
                }
            };
        }
    }
}