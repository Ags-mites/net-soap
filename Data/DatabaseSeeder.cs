using EnviosExpressAPI.Models;

namespace EnviosExpressAPI.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Iniciando seed de datos...");

                if (context.Packages.Any())
                {
                    logger.LogInformation("La base de datos ya contiene datos.");
                    return;
                }

                var packages = new List<Package>
                {
                    new Package
                    {
                        TrackingNumber = "PE1234567890",
                        SenderName = "Agustin Mites",
                        ReceiverName = "Tatiana Ligñá",
                        Origin = "Cuenca",
                        Destination = "Quito",
                        Weight = 2.5m,
                        Dimensions = "30x20x15",
                        Status = "En tránsito",
                        CurrentLocation = "Oficina EnvíosExpress S.A.C. Azuay",
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3),
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },

                    new Package
                    {
                        TrackingNumber = "PE0987654321",
                        SenderName = "Karen Almagro",
                        ReceiverName = "Agustin Mites",
                        Origin = "Puerto Libre",
                        Destination = "Quito",
                        Weight = 1.8m,
                        Dimensions = "25x15x10",
                        Status = "Entregado",
                        CurrentLocation = "Quito - Ecuador",
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(-1),
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },

                    new Package
                    {
                        TrackingNumber = "EC5555123456",
                        SenderName = "Milena Reyes",
                        ReceiverName = "Santiago Llumiquinga",
                        Origin = "Quito",
                        Destination = "Ambato",
                        Weight = 0.8m,
                        Dimensions = "20x10x5",
                        Status = "Listo para retiro",
                        CurrentLocation = "Oficina EnvíosExpress S.A.C Ambato",
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(1),
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },

                    new Package
                    {
                        TrackingNumber = "PE2024001122",
                        SenderName = "Banco Pichincha",
                        ReceiverName = "Banco Solidario",
                        Origin = "Ibarra",
                        Destination = "Quito",
                        Weight = 15.2m,
                        Dimensions = "80x60x40",
                        Status = "En proceso",
                        CurrentLocation = "Oficina EnvíosExpress S.A.C Ibarra",
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5),
                        CreatedAt = DateTime.UtcNow.AddHours(-6)
                    },

                    new Package
                    {
                        TrackingNumber = "EC9999ERROR1",
                        SenderName = "Agustin Mites",
                        ReceiverName = "Johanna Moncayo",
                        Origin = "Baños",
                        Destination = "Guayaquil",
                        Weight = 1.0m,
                        Dimensions = "10x10x10",
                        Status = "Incidencia - Dirección incorrecta",
                        CurrentLocation = "Centro de Atención al Cliente",
                        EstimatedDeliveryDate = null,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };

                context.Packages.AddRange(packages);
                context.SaveChanges();

                logger.LogInformation($"Se insertaron {packages.Count} paquetes");

                var events = new List<TrackingEvent>
                {
                    new TrackingEvent
                    {
                        TrackingNumber = "PE1234567890",
                        Date = DateTime.UtcNow.AddDays(-2),
                        Description = "Paquete recibido en oficina de origen",
                        Location = "Cuenca"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE1234567890",
                        Date = DateTime.UtcNow.AddDays(-1).AddHours(-8),
                        Description = "En proceso de clasificación y preparación",
                        Location = "Cuenca"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE1234567890",
                        Date = DateTime.UtcNow.AddDays(-1),
                        Description = "Salida hacia destino",
                        Location = "Cuenca"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE1234567890",
                        Date = DateTime.UtcNow.AddHours(-6),
                        Description = "En tránsito - Llegada a centro de distribución",
                        Location = "Oficina EnvíosExpress S.A.C. Azuay"
                    },

                    new TrackingEvent
                    {
                        TrackingNumber = "PE0987654321",
                        Date = DateTime.UtcNow.AddDays(-5),
                        Description = "Paquete recibido en oficina de origen",
                        Location = "Puerto Libre"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE0987654321",
                        Date = DateTime.UtcNow.AddDays(-4),
                        Description = "En proceso de clasificación",
                        Location = "Puerto Libre"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE0987654321",
                        Date = DateTime.UtcNow.AddDays(-3),
                        Description = "En tránsito hacia destino",
                        Location = "Puerto Libre"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE0987654321",
                        Date = DateTime.UtcNow.AddDays(-2),
                        Description = "Llegada a centro de distribución destino",
                        Location = "Quito"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE0987654321",
                        Date = DateTime.UtcNow.AddDays(-1),
                        Description = "Paquete entregado exitosamente",
                        Location = "Quito - Ecuador"
                    },

                    new TrackingEvent
                    {
                        TrackingNumber = "EC5555123456",
                        Date = DateTime.UtcNow.AddDays(-3),
                        Description = "Paquete recibido en oficina de origen",
                        Location = "Quito"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC5555123456",
                        Date = DateTime.UtcNow.AddDays(-2),
                        Description = "En proceso de clasificación",
                        Location = "Quito"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC5555123456",
                        Date = DateTime.UtcNow.AddDays(-1).AddHours(-6),
                        Description = "En tránsito hacia destino",
                        Location = "Quito"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC5555123456",
                        Date = DateTime.UtcNow.AddHours(-12),
                        Description = "Llegada a oficina de destino - Listo para retiro",
                        Location = "Oficina EnvíosExpress S.A.C Ambato"
                    },

                    new TrackingEvent
                    {
                        TrackingNumber = "PE2024001122",
                        Date = DateTime.UtcNow.AddHours(-6),
                        Description = "Paquete recibido - En proceso de documentación",
                        Location = "Oficina EnvíosExpress S.A.C Ibarra"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "PE2024001122",
                        Date = DateTime.UtcNow.AddHours(-3),
                        Description = "Verificación de documentos bancarios en proceso",
                        Location = "Oficina EnvíosExpress S.A.C Ibarra"
                    },

                    new TrackingEvent
                    {
                        TrackingNumber = "EC9999ERROR1",
                        Date = DateTime.UtcNow.AddDays(-1),
                        Description = "Paquete recibido en oficina de origen",
                        Location = "Baños"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC9999ERROR1",
                        Date = DateTime.UtcNow.AddHours(-18),
                        Description = "En proceso de clasificación",
                        Location = "Baños"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC9999ERROR1",
                        Date = DateTime.UtcNow.AddHours(-12),
                        Description = "Incidencia detectada: Dirección de destino incorrecta",
                        Location = "Centro de Atención al Cliente"
                    },
                    new TrackingEvent
                    {
                        TrackingNumber = "EC9999ERROR1",
                        Date = DateTime.UtcNow.AddHours(-6),
                        Description = "Contactando al destinatario para confirmación de dirección",
                        Location = "Centro de Atención al Cliente"
                    }
                };

                context.TrackingEvents.AddRange(events);
                context.SaveChanges();

                logger.LogInformation($"Se insertaron {events.Count} eventos de seguimiento");

                var packageCount = context.Packages.Count();
                var eventCount = context.TrackingEvents.Count();

                logger.LogInformation("=== RESUMEN DE DATOS INSERTADOS ===");
                logger.LogInformation($"Total paquetes: {packageCount}");
                logger.LogInformation($"Total eventos: {eventCount}");
                logger.LogInformation("Números de tracking disponibles:");

                var trackingNumbers = context.Packages.Select(p => new { p.TrackingNumber, p.Status }).ToList();
                foreach (var pkg in trackingNumbers)
                {
                    logger.LogInformation($"* {pkg.TrackingNumber} - {pkg.Status}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error durante el seed de datos");
                throw;
            }
        }
    }
}