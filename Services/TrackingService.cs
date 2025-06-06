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

        public string TestConnection()
        {
            _logger.LogInformation("🧪 TestConnection llamado");
            return "✅ Conexión exitosa - Servicio SOAP funcionando perfectamente";
        }

        // Método alternativo que funciona mejor con SoapCore
        public GetTrackingStatusResponse GetTrackingStatusDirect(string trackingNumber)
        {
            _logger.LogInformation("🚀 === GetTrackingStatusDirect llamado ===");
            _logger.LogInformation("📦 TrackingNumber recibido directamente: '{TrackingNumber}'", trackingNumber ?? "NULL");

            return GetTrackingStatusInternal(trackingNumber);
        }

        public GetTrackingStatusResponse GetTrackingStatus(GetTrackingStatusRequest request)
        {
            _logger.LogInformation("🚀 === GetTrackingStatus (con objeto) llamado ===");

            try
            {
                // Debug: Verificar si el request llega
                _logger.LogInformation("Request recibido: {Request}", request != null ? "NO NULO" : "NULO");

                if (request != null)
                {
                    _logger.LogInformation("TrackingNumber: '{TrackingNumber}' (Tipo: {Type})",
                        request.TrackingNumber ?? "NULL",
                        request.TrackingNumber?.GetType().Name ?? "NULL");
                    _logger.LogInformation("TrackingNumber Length: {Length}",
                        request.TrackingNumber?.Length ?? -1);

                    // Llamar al método interno
                    return GetTrackingStatusInternal(request.TrackingNumber);
                }
                else
                {
                    _logger.LogWarning("Request es nulo - lanzando excepción");
                    throw CreateTrackingFault(400, "La solicitud es requerida", null);
                }
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al consultar paquete: {TrackingNumber}", request?.TrackingNumber);
                throw CreateTrackingFault(500, "Error interno del servidor", null);
            }
        }

        private GetTrackingStatusResponse GetTrackingStatusInternal(string trackingNumber)
        {
            _logger.LogInformation("🔍 === GetTrackingStatusInternal ===");

            try
            {
                // Debug: Verificar conexión a BD
                var connectionString = _context.Database.GetConnectionString();
                _logger.LogInformation("Connection String: {ConnectionString}",
                    connectionString ?? "NULL");

                // Debug: Verificar que hay datos en la BD
                var totalPackages = _context.Packages.Count();
                _logger.LogInformation("Total paquetes en BD: {Count}", totalPackages);

                if (totalPackages > 0)
                {
                    var allTrackingNumbers = _context.Packages.Select(p => p.TrackingNumber).ToList();
                    _logger.LogInformation("Tracking numbers en BD: [{Numbers}]",
                        string.Join(", ", allTrackingNumbers));
                }

                // Validar entrada
                if (string.IsNullOrWhiteSpace(trackingNumber))
                {
                    _logger.LogWarning("Número de tracking vacío o nulo. Valor: '{Value}'",
                        trackingNumber ?? "NULL");
                    throw CreateTrackingFault(400, "El número de tracking es requerido", "TrackingNumber");
                }

                // Normalizar el número de tracking
                var normalizedTrackingNumber = trackingNumber.Trim().ToUpperInvariant();
                _logger.LogInformation("Tracking number normalizado: '{TrackingNumber}'", normalizedTrackingNumber);

                // Validar formato básico del tracking number
                if (normalizedTrackingNumber.Length < 5 || normalizedTrackingNumber.Length > 50)
                {
                    _logger.LogWarning("Formato de tracking number inválido: {TrackingNumber} (Length: {Length})",
                        normalizedTrackingNumber, normalizedTrackingNumber.Length);
                    throw CreateTrackingFault(400, "El formato del número de tracking es inválido", "TrackingNumber");
                }

                // Debug: Buscar paquete con logging detallado
                _logger.LogInformation("Buscando paquete en BD con tracking: '{TrackingNumber}'", normalizedTrackingNumber);

                var package = _context.Packages
                    .Include(p => p.History)
                    .FirstOrDefault(p => p.TrackingNumber == normalizedTrackingNumber);

                _logger.LogInformation("Resultado de búsqueda: {Result}",
                    package != null ? "ENCONTRADO" : "NO ENCONTRADO");

                if (package == null)
                {
                    _logger.LogWarning("Paquete no encontrado: {TrackingNumber}", normalizedTrackingNumber);

                    // Debug adicional: mostrar comparación exacta
                    var exactMatches = _context.Packages
                        .Select(p => new { p.TrackingNumber, Match = p.TrackingNumber == normalizedTrackingNumber })
                        .ToList();

                    _logger.LogInformation("Comparación exacta de tracking numbers:");
                    foreach (var match in exactMatches)
                    {
                        _logger.LogInformation("BD: '{DbNumber}' vs Buscado: '{SearchNumber}' = {Match}",
                            match.TrackingNumber, normalizedTrackingNumber, match.Match);
                    }

                    throw CreateTrackingFault(404, "Paquete no encontrado", "TrackingNumber");
                }

                _logger.LogInformation("Paquete encontrado: {TrackingNumber}, Estado: {Status}, Historia: {HistoryCount} eventos",
                    package.TrackingNumber, package.Status, package.History.Count);

                // Mapear respuesta con logging
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

                _logger.LogInformation("Response creado: Status='{Status}', Location='{Location}', History={HistoryCount}",
                    response.Status, response.CurrentLocation, response.History.Count);

                _logger.LogInformation("✅ === FINALIZANDO GetTrackingStatusInternal EXITOSAMENTE ===");
                return response;
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno en GetTrackingStatusInternal: {TrackingNumber}", trackingNumber);
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