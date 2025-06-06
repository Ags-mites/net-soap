using Microsoft.EntityFrameworkCore;
using SoapCore;
using EnviosExpressAPI.Data;
using EnviosExpressAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio SOAP
builder.Services.AddScoped<ITrackingService, TrackingService>();

// Configurar SoapCore
builder.Services.AddSoapCore();

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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

// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.EnsureCreated();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Base de datos inicializada correctamente");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

// Configurar SOAP endpoint - Versión simplificada
app.UseSoapEndpoint<ITrackingService>("/TrackingService.asmx", new SoapEncoderOptions());

// Middleware para logging de requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Solicitud recibida: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

// Mapear controladores (opcional)
app.MapControllers();

// Página de información en la raíz
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