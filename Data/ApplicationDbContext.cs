using Microsoft.EntityFrameworkCore;
using EnviosExpressAPI.Models;

namespace EnviosExpressAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; } = null!;
        public DbSet<TrackingEvent> TrackingEvents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Package
            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.TrackingNumber);
                entity.Property(e => e.TrackingNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.SenderName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.ReceiverName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Origin)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Destination)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.CurrentLocation)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Dimensions)
                    .HasMaxLength(50);

                entity.Property(e => e.Weight)
                    .HasPrecision(10, 2)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Índice para mejorar performance
                entity.HasIndex(e => e.Status);
            });

            // Configuración de TrackingEvent
            modelBuilder.Entity<TrackingEvent>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TrackingNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Date)
                    .IsRequired();

                // Configurar la relación con Package
                entity.HasOne(e => e.Package)
                      .WithMany(p => p.History)
                      .HasForeignKey(e => e.TrackingNumber)
                      .HasPrincipalKey(p => p.TrackingNumber)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índices para mejorar performance
                entity.HasIndex(e => e.TrackingNumber);
                entity.HasIndex(e => e.Date);
            });

            // Datos de prueba
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
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
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3),
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
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
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            );

            // Eventos de seguimiento
            modelBuilder.Entity<TrackingEvent>().HasData(
                new TrackingEvent
                {
                    Id = 1,
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.UtcNow.AddDays(-2),
                    Description = "Paquete recibido en bodega central",
                    Location = "Lima"
                },
                new TrackingEvent
                {
                    Id = 2,
                    TrackingNumber = "PE1234567890",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Description = "Salida hacia destino",
                    Location = "Lima"
                },
                new TrackingEvent
                {
                    Id = 3,
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.UtcNow.AddDays(-5),
                    Description = "Paquete recibido en bodega central",
                    Location = "Arequipa"
                },
                new TrackingEvent
                {
                    Id = 4,
                    TrackingNumber = "PE0987654321",
                    Date = DateTime.UtcNow.AddDays(-1),
                    Description = "Paquete entregado exitosamente",
                    Location = "Guayaquil"
                }
            );
        }
    }
}