using Microsoft.EntityFrameworkCore;
using SoapCore;
using EnviosExpressAPI.Data;
using EnviosExpressAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITrackingService, TrackingService>();
builder.Services.AddSoapCore();

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configurar SOAP endpoint
app.UseSoapEndpoint<ITrackingService>("/TrackingService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

// Middleware para información del servidor
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Solicitud recibida: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

// Página de información
app.MapGet("/", () =>
{
    return Results.Text(@"
    🚚 API SOAP - EnvíosExpress S.A.C.
    
    Servicio de Seguimiento de Paquetes
    
    WSDL disponible en: /TrackingService.asmx?wsdl
    Endpoint SOAP: /TrackingService.asmx
    
    Números de tracking de prueba:
    - PE1234567890 (En tránsito)
    - PE0987654321 (Entregado)
    
    Estado del servidor: ✅ Activo
    ", "text/plain; charset=utf-8");
});

app.Run();