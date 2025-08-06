using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TrackingNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string PickupAddress { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string DeliveryAddress { get; set; } = string.Empty;
        
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public double DeliveryLatitude { get; set; }
        public double DeliveryLongitude { get; set; }
        
        public DateTime ScheduledPickupTime { get; set; }
        public DateTime ScheduledDeliveryTime { get; set; }
        public DateTime? ActualPickupTime { get; set; }
        public DateTime? ActualDeliveryTime { get; set; }
        
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Scheduled;
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public int Priority { get; set; } = 1; // 1 = Low, 2 = Medium, 3 = High
        
        // Foreign keys
        public int? VehicleId { get; set; }
        public int CustomerId { get; set; }
        
        // Navigation properties
        public Vehicle? Vehicle { get; set; }
        public Customer Customer { get; set; } = null!;
        public ICollection<DeliveryAlert> Alerts { get; set; } = new List<DeliveryAlert>();
    }
    
    public enum DeliveryStatus
    {
        Scheduled,
        Assigned,
        InTransit,
        OutForDelivery,
        Delivered,
        Failed,
        Cancelled
    }
} 