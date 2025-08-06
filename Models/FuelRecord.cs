using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class FuelRecord
    {
        public int Id { get; set; }
        
        [Required]
        public int VehicleId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Fuel amount must be positive")]
        public double FuelAmount { get; set; } // in liters/gallons
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Cost must be positive")]
        public decimal Cost { get; set; }
        
        [Required]
        public DateTime RefuelDate { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Location { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? FuelType { get; set; } // Diesel, Gasoline, Electric, etc.
        
        public double? OdometerReading { get; set; }
        
        public double? DistanceTraveled { get; set; } // since last refuel
        
        public double? FuelEfficiency { get; set; } // km/l or mpg
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation property
        public Vehicle Vehicle { get; set; } = null!;
    }
} 