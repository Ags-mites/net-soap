using EnviosExpressAPI.Data;
using EnviosExpressAPI.DTOs;
using EnviosExpressAPI.Services;
using Microsoft.EntityFrameworkCore;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro del servicio SOAP
builder.Services.AddScoped<ITrackingService, TrackingService>();


builder.Services.AddSoapCore();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();

// Configuración del Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Ejecución de migraciones y seed datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Validando conexion a la base de datos");
        context.Database.EnsureCreated();
        
        // Ejecución del seed
        DatabaseSeeder.SeedDatabase(context, logger);
        
        var connectionString = context.Database.GetConnectionString();
        logger.LogInformation("Conectado a: {ConnectionString}", connectionString?.Replace("Password=postgres123", "Password=***"));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al inicializar la base de datos");
        throw;
    }
}

// Configurar SOAP endpoint
app.UseSoapEndpoint<ITrackingService>("/TrackingService.asmx", new SoapEncoderOptions()
{
    MessageVersion = System.ServiceModel.Channels.MessageVersion.Soap11,
    WriteEncoding = System.Text.Encoding.UTF8,
    ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max
});

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

    if (context.Request.Path.StartsWithSegments("/TrackingService.asmx"))
    {
        if (context.Request.Headers.ContainsKey("SOAPAction"))
        {
            logger.LogInformation("SOAPAction: {SOAPAction}", context.Request.Headers["SOAPAction"]);
        }

        if (context.Request.Method == "POST" &&
            context.Request.ContentType?.Contains("xml") == true)
        {
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            logger.LogInformation("SOAP Body: {Body}", body);
        }

    }

    await next();
});

app.MapControllers();

// Información del servicio
app.MapGet("/", () =>
{
    return Results.Text(@"
    API SOAP - EnvíosExpress S.A.C.
    
     - Servicio de Seguimiento de Paquetes
    
    Endpoints:
    • WSDL: /TrackingService.asmx?wsdl
    • SOAP: /TrackingService.asmx
    • Swagger: /swagger
    
    Estado del servidor: Activo
    Base de datos: PostgreSQL
    ", "text/plain; charset=utf-8");
});

app.Run();