using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string DriverName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty; // Truck, Van, etc.
        
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        
        public DateTime LastLocationUpdate { get; set; }
        
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;
        
        public bool IsActive { get; set; } = true;
        
        // Fuel and cost management properties
        public double? CurrentFuelLevel { get; set; } // percentage
        public double? FuelCapacity { get; set; } // liters/gallons
        public double? AverageFuelEfficiency { get; set; } // km/l or mpg
        public decimal? TotalFuelCost { get; set; } // this month
        public DateTime? LastFuelUpdate { get; set; }
        
        // Navigation properties
        public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
        public ICollection<VehicleAlert> Alerts { get; set; } = new List<VehicleAlert>();
        public ICollection<FuelRecord> FuelRecords { get; set; } = new List<FuelRecord>();
        public ICollection<CostAnalytics> CostAnalytics { get; set; } = new List<CostAnalytics>();
        public ICollection<EnergyManagement> EnergyManagement { get; set; } = new List<EnergyManagement>();
    }
    
    public enum VehicleStatus
    {
        Available,
        InTransit,
        Delivering,
        Maintenance,
        Offline
    }
} 