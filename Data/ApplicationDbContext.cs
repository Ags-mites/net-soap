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

                entity.HasOne(e => e.Package)
                      .WithMany(p => p.History)
                      .HasForeignKey(e => e.TrackingNumber)
                      .HasPrincipalKey(p => p.TrackingNumber)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TrackingNumber);
                entity.HasIndex(e => e.Date);
            });
        }
    }
}