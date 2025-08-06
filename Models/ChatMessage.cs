using System.ComponentModel.DataAnnotations;

namespace CargoRouteTracker.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public MessageType Type { get; set; } = MessageType.User;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? SessionId { get; set; }
        
        public bool IsProcessed { get; set; } = false;
    }
    
    public enum MessageType
    {
        User,
        Bot,
        System
    }
} 