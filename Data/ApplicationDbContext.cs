using Microsoft.EntityFrameworkCore;
using CargoRouteTracker.Models;

namespace CargoRouteTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<VehicleAlert> VehicleAlerts { get; set; }
        public DbSet<DeliveryAlert> DeliveryAlerts { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<FuelRecord> FuelRecords { get; set; }
        public DbSet<CostAnalytics> CostAnalytics { get; set; }
        public DbSet<EnergyManagement> EnergyManagement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Deliveries)
                .WithOne(d => d.Vehicle)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.Alerts)
                .WithOne(a => a.Vehicle)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Customer)
                .WithMany(c => c.Deliveries)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Delivery>()
                .HasMany(d => d.Alerts)
                .WithOne(a => a.Delivery)
                .HasForeignKey(a => a.DeliveryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.FuelRecords)
                .WithOne(f => f.Vehicle)
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.CostAnalytics)
                .WithOne(c => c.Vehicle)
                .HasForeignKey(c => c.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.EnergyManagement)
                .WithOne(e => e.Vehicle)
                .HasForeignKey(e => e.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data will be added later
        }


    }
} 