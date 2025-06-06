using Microsoft.EntityFrameworkCore;
using EnviosExpressAPI.Models;

namespace EnviosExpressAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<TrackingEvent> TrackingEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Package
            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.TrackingNumber);
                entity.Property(e => e.TrackingNumber).HasMaxLength(50);
                entity.Property(e => e.SenderName).HasMaxLength(200);
                entity.Property(e => e.ReceiverName).HasMaxLength(200);
                entity.Property(e => e.Origin).HasMaxLength(100);
                entity.Property(e => e.Destination).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.CurrentLocation).HasMaxLength(100);
                entity.Property(e => e.Dimensions).HasMaxLength(50);
                entity.Property(e => e.Weight).HasPrecision(10, 2);
            });

            // Configuración de TrackingEvent
            modelBuilder.Entity<TrackingEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TrackingNumber).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Location).HasMaxLength(100);

                entity.HasOne(e => e.Package)
                      .WithMany(p => p.History)
                      .HasForeignKey(e => e.TrackingNumber)
                      .HasPrincipalKey(p => p.TrackingNumber);
            });

            // Datos de prueba
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Paquetes de prueba
            modelBuilder.Entity<Package>().HasData(
                new Package
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
                    EstimatedDeliveryDate = DateTime.Parse("2025-04-15"),
                    CreatedAt = DateTime.UtcNow
                },
                new Package
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
                    EstimatedDeliveryDate = DateTime.Parse("2025-04-10"),
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Eventos de seguimiento
            modelBuilder.Entity<TrackingEvent>().HasData(
                new TrackingEvent
                {
                    Id = 1,
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.Parse("2025-04-05"),
                    Description = "Paquete recibido en bodega central",
                    Location = "Lima"
                },
                new TrackingEvent
                {
                    Id = 2,
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.Parse("2025-04-07"),
                    Description = "Salida hacia destino",
                    Location = "Lima"
                },
                new TrackingEvent
                {
                    Id = 3,
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.Parse("2025-04-08"),
                    Description = "Paquete recibido en bodega central",
                    Location = "Arequipa"
                },
                new TrackingEvent
                {
                    Id = 4,
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.Parse("2025-04-10"),
                    Description = "Paquete entregado exitosamente",
                    Location = "Guayaquil"
                }
            );
        }
    }
}