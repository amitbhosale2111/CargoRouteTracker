using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class DeliveryAlert
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public DeliveryAlertType Type { get; set; }
        
        public AlertSeverity Severity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ResolvedAt { get; set; }
        
        public bool IsResolved { get; set; } = false;
        
        // Foreign key
        public int DeliveryId { get; set; }
        
        // Navigation property
        public Delivery Delivery { get; set; } = null!;
    }
    
    public enum DeliveryAlertType
    {
        Delay,
        RouteChange,
        CustomerNotAvailable,
        AddressIssue,
        PackageDamage,
        WeatherDelay,
        TrafficDelay
    }
} 