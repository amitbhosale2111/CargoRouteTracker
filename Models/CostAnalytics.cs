using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class CostAnalytics
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public int VehicleId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public double FuelConsumption { get; set; } // liters/gallons
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal FuelCost { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaintenanceCost { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal OperationalCost { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalCost { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public double DistanceTraveled { get; set; } // km/miles
        
        [Required]
        [Range(0, double.MaxValue)]
        public double FuelEfficiency { get; set; } // km/l or mpg
        
        [Required]
        [Range(0, 100)]
        public double CostPerKm { get; set; } // cost per kilometer/mile
        
        [Required]
        [Range(0, 100)]
        public double EnergyScore { get; set; } // 0-100 energy efficiency score
        
        [Required]
        [Range(0, 100)]
        public double CostSavingsPercentage { get; set; } // compared to baseline
        
        [StringLength(500)]
        public string? Recommendations { get; set; }
        
        // Navigation property
        public Vehicle Vehicle { get; set; } = null!;
    }
    
    public class EnergyManagement
    {
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public int VehicleId { get; set; }
        
        [Required]
        [Range(0, 100)]
        public double IdleTimePercentage { get; set; }
        
        [Required]
        [Range(0, 100)]
        public double AggressiveDrivingScore { get; set; } // 0-100, lower is better
        
        [Required]
        [Range(0, 100)]
        public double RouteOptimizationScore { get; set; } // 0-100
        
        [Required]
        [Range(0, 100)]
        public double MaintenanceScore { get; set; } // 0-100
        
        [Required]
        [Range(0, 100)]
        public double OverallEfficiencyScore { get; set; } // 0-100
        
        [StringLength(500)]
        public string? ImprovementSuggestions { get; set; }
        
        // Navigation property
        public Vehicle Vehicle { get; set; } = null!;
    }
} 