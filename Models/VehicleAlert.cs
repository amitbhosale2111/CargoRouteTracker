using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class VehicleAlert
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public AlertType Type { get; set; }
        
        public AlertSeverity Severity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ResolvedAt { get; set; }
        
        public bool IsResolved { get; set; } = false;
        
        // Foreign key
        public int VehicleId { get; set; }
        
        // Navigation property
        public Vehicle Vehicle { get; set; } = null!;
    }
    
    public enum AlertType
    {
        Maintenance,
        Delay,
        RouteChange,
        FuelLow,
        EngineIssue,
        Traffic,
        Weather
    }
    
    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
} 