using EnviosExpressAPI.Data;
using EnviosExpressAPI.DTOs;
using EnviosExpressAPI.Services;
using Microsoft.EntityFrameworkCore;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio SOAP
builder.Services.AddScoped<ITrackingService, TrackingService>();

// Configurar SoapCore
builder.Services.AddSoapCore();

// Configurar logging mejorado
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Agregar servicios para controladores (opcional)
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicar migraciones y seed data automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Crear la base de datos si no existe
        logger.LogInformation("Creando base de datos...");
        context.Database.EnsureCreated();
        logger.LogInformation("Base de datos creada exitosamente");

        // Verificar si ya hay datos
        if (!context.Packages.Any())
        {
            logger.LogInformation("Insertando datos de prueba...");

            // Agregar paquetes de prueba
            var testPackages = new[]
            {
                new EnviosExpressAPI.Models.Package
                {
                    TrackingNumber = "PE1234567890",
                    SenderName = "Juan Pérez",
                    ReceiverName = "María García",
                    Origin = "Lima",
                    Destination = "Quito",
                    Weight = 2.5m,
                    Dimensions = "30x20x15",
                    Status = "En tránsito",
                    CurrentLocation = "Lima - Perú",
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3),
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new EnviosExpressAPI.Models.Package
                {
                    TrackingNumber = "PE0987654321",
                    SenderName = "Ana López",
                    ReceiverName = "Carlos Mendoza",
                    Origin = "Arequipa",
                    Destination = "Guayaquil",
                    Weight = 1.8m,
                    Dimensions = "25x15x10",
                    Status = "Entregado",
                    CurrentLocation = "Guayaquil - Ecuador",
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };

            context.Packages.AddRange(testPackages);
            context.SaveChanges();

            // Agregar eventos de seguimiento
            var events = new[]
            {
                new EnviosExpressAPI.Models.TrackingEvent
                {
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.UtcNow.AddDays(-2),
                    Description = "Paquete recibido en bodega central",
                    Location = "Lima"
                },
                new EnviosExpressAPI.Models.TrackingEvent
                {
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Description = "Salida hacia destino",
                    Location = "Lima"
                },
                new EnviosExpressAPI.Models.TrackingEvent
                {
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.UtcNow.AddDays(-5),
                    Description = "Paquete recibido en bodega central",
                    Location = "Arequipa"
                },
                new EnviosExpressAPI.Models.TrackingEvent
                {
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Description = "Paquete entregado exitosamente",
                    Location = "Guayaquil"
                }
            };

            context.TrackingEvents.AddRange(events);
            context.SaveChanges();

            logger.LogInformation("Datos de prueba insertados exitosamente");
        }

        // Mostrar los paquetes disponibles
        var existingPackages = context.Packages.ToList();
        logger.LogInformation("=== PAQUETES DISPONIBLES PARA PRUEBA ===");
        foreach (var pkg in existingPackages)
        {
            logger.LogInformation("📦 {TrackingNumber}: {Status} en {Location}",
                pkg.TrackingNumber, pkg.Status, pkg.CurrentLocation);
        }
        logger.LogInformation("=== FIN LISTA DE PAQUETES ===");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al inicializar la base de datos");
        throw; // Re-lanzar para que Docker sepa que falló
    }
}

// Configurar SOAP endpoint con configuraciones mejoradas
app.UseSoapEndpoint<ITrackingService>("/TrackingService.asmx", new SoapEncoderOptions()
{
    MessageVersion = System.ServiceModel.Channels.MessageVersion.Soap11,
    WriteEncoding = System.Text.Encoding.UTF8,
    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
});

// Middleware para logging detallado de requests SOAP
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    if (context.Request.Path.StartsWithSegments("/TrackingService.asmx"))
    {
        logger.LogInformation("🔍 === SOAP REQUEST RECIBIDO ===");
        logger.LogInformation("Method: {Method}", context.Request.Method);
        logger.LogInformation("Path: {Path}", context.Request.Path);
        logger.LogInformation("Content-Type: {ContentType}", context.Request.ContentType);

        // Log headers importantes
        if (context.Request.Headers.ContainsKey("SOAPAction"))
        {
            logger.LogInformation("SOAPAction: {SOAPAction}", context.Request.Headers["SOAPAction"]);
        }

        // Log body para requests POST
        if (context.Request.Method == "POST" &&
            context.Request.ContentType?.Contains("xml") == true)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            logger.LogInformation("📄 SOAP Body: {Body}", body);
        }

        logger.LogInformation("🔍 === FIN SOAP REQUEST ===");
    }

    await next();
});

// Mapear controladores (opcional)
app.MapControllers();

// Endpoint de testing para verificar BD
app.MapGet("/test/packages", (ApplicationDbContext context) =>
{
    try
    {
        var packages = context.Packages.Include(p => p.History).ToList();
        return Results.Json(new
        {
            success = true,
            count = packages.Count,
            packages = packages.Select(p => new
            {
                trackingNumber = p.TrackingNumber,
                status = p.Status,
                currentLocation = p.CurrentLocation,
                weight = p.Weight,
                dimensions = p.Dimensions,
                estimatedDeliveryDate = p.EstimatedDeliveryDate,
                historyCount = p.History.Count,
                history = p.History.OrderBy(h => h.Date).Select(h => new
                {
                    date = h.Date,
                    description = h.Description,
                    location = h.Location
                })
            })
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
    }
});

// Endpoint de testing para el servicio SOAP
app.MapGet("/test/soap/{trackingNumber}", async (string trackingNumber, ITrackingService trackingService) =>
{
    try
    {
        var request = new GetTrackingStatusRequest { TrackingNumber = trackingNumber };
        var response = trackingService.GetTrackingStatus(request);
        return Results.Json(new { success = true, response });
    }
    catch (Exception ex)
    {
        return Results.Json(new { success = false, error = ex.Message, type = ex.GetType().Name, stackTrace = ex.StackTrace });
    }
});

// Página de información en la raíz
app.MapGet("/", () =>
{
    return Results.Text(@"
    🚚 API SOAP - EnvíosExpress S.A.C.
    
    ✅ Servicio de Seguimiento de Paquetes
    
    📋 Endpoints disponibles:
    • WSDL: /TrackingService.asmx?wsdl
    • SOAP: /TrackingService.asmx
    • Swagger: /swagger
    • Test BD: /test/packages
    • Test SOAP: /test/soap/{trackingNumber}
    
    📦 Números de tracking de prueba:
    • PE1234567890 (En tránsito)
    • PE0987654321 (Entregado)
    
    🌐 Estado del servidor: ✅ Activo
    🐘 Base de datos: PostgreSQL
    🐳 Ejecutándose en Docker
    ", "text/plain; charset=utf-8");
});

app.Run();