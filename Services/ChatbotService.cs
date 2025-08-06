using CargoRouteTracker.Data;
using CargoRouteTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoRouteTracker.Services
{
    public interface IChatbotService
    {
        Task<ChatMessage> ProcessMessageAsync(string userMessage, string sessionId);
        Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId);
    }

    public class ChatbotService : IChatbotService
    {
        private readonly ApplicationDbContext _context;

        public ChatbotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessage> ProcessMessageAsync(string userMessage, string sessionId)
        {
            var response = await GenerateResponseAsync(userMessage);
            
            var botMessage = new ChatMessage
            {
                Message = response,
                Type = MessageType.Bot,
                SessionId = sessionId,
                Timestamp = DateTime.UtcNow,
                IsProcessed = true
            };

            _context.ChatMessages.Add(botMessage);
            await _context.SaveChangesAsync();

            return botMessage;
        }

        public async Task<List<ChatMessage>> GetChatHistoryAsync(string sessionId)
        {
            return await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        private async Task<string> GenerateResponseAsync(string userMessage)
        {
            var message = userMessage.ToLower().Trim();

            // Delivery tracking queries
            if (message.Contains("track") || message.Contains("where") || message.Contains("delivery"))
            {
                return await HandleDeliveryTrackingQuery(message);
            }

            // Vehicle status queries
            if (message.Contains("vehicle") || message.Contains("truck") || message.Contains("van") || message.Contains("driver"))
            {
                return await HandleVehicleQuery(message);
            }

            // Alert queries
            if (message.Contains("alert") || message.Contains("problem") || message.Contains("issue") || message.Contains("delay"))
            {
                return await HandleAlertQuery(message);
            }

            // General help
            if (message.Contains("help") || message.Contains("what can you do"))
            {
                return GetHelpResponse();
            }

            // Greeting
            if (message.Contains("hello") || message.Contains("hi") || message.Contains("hey"))
            {
                return "Hello! I'm your cargo delivery assistant. I can help you track deliveries, check vehicle status, and get information about alerts. How can I help you today?";
            }

            // Default response
            return "I'm not sure I understand. You can ask me about:\n" +
                   "• Delivery tracking (e.g., 'Track delivery TRK123456789')\n" +
                   "• Vehicle status (e.g., 'Where is vehicle TRK-001?')\n" +
                   "• Alerts and issues (e.g., 'Any alerts today?')\n" +
                   "• General help (e.g., 'What can you do?')";
        }

        private async Task<string> HandleDeliveryTrackingQuery(string message)
        {
            // Extract tracking number if present
            var trackingNumber = ExtractTrackingNumber(message);
            
            if (!string.IsNullOrEmpty(trackingNumber))
            {
                var delivery = await _context.Deliveries
                    .Include(d => d.Vehicle)
                    .Include(d => d.Customer)
                    .FirstOrDefaultAsync(d => d.TrackingNumber == trackingNumber);

                if (delivery != null)
                {
                    return $"📦 **Delivery {delivery.TrackingNumber}**\n" +
                           $"Status: {delivery.Status}\n" +
                           $"Customer: {delivery.Customer?.Name}\n" +
                           $"Vehicle: {delivery.Vehicle?.VehicleNumber ?? "Unassigned"}\n" +
                           $"Scheduled Delivery: {delivery.ScheduledDeliveryTime:MMM dd, yyyy 'at' HH:mm}\n" +
                           $"From: {delivery.PickupAddress}\n" +
                           $"To: {delivery.DeliveryAddress}";
                }
                else
                {
                    return $"❌ Sorry, I couldn't find delivery {trackingNumber}. Please check the tracking number and try again.";
                }
            }

            // General delivery status
            var activeDeliveries = await _context.Deliveries
                .Include(d => d.Customer)
                .Where(d => d.Status != DeliveryStatus.Delivered && d.Status != DeliveryStatus.Failed)
                .CountAsync();

            var completedToday = await _context.Deliveries
                .Where(d => d.ActualDeliveryTime.HasValue && 
                           d.ActualDeliveryTime.Value.Date == DateTime.Today)
                .CountAsync();

            return $"📊 **Delivery Summary**\n" +
                   $"Active Deliveries: {activeDeliveries}\n" +
                   $"Completed Today: {completedToday}\n\n" +
                   $"To track a specific delivery, please provide the tracking number (e.g., 'Track TRK123456789').";
        }

        private async Task<string> HandleVehicleQuery(string message)
        {
            // Extract vehicle number if present
            var vehicleNumber = ExtractVehicleNumber(message);
            
            if (!string.IsNullOrEmpty(vehicleNumber))
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.Alerts.Where(a => !a.IsResolved))
                    .FirstOrDefaultAsync(v => v.VehicleNumber == vehicleNumber);

                if (vehicle != null)
                {
                    var alertCount = vehicle.Alerts.Count;
                    var alertText = alertCount > 0 ? $"\n⚠️ {alertCount} active alert(s)" : "";

                    return $"🚛 **Vehicle {vehicle.VehicleNumber}**\n" +
                           $"Driver: {vehicle.DriverName}\n" +
                           $"Status: {vehicle.Status}\n" +
                           $"Type: {vehicle.VehicleType}\n" +
                           $"Last Location: {vehicle.CurrentLatitude:F4}, {vehicle.CurrentLongitude:F4}\n" +
                           $"Last Update: {vehicle.LastLocationUpdate:MMM dd, HH:mm}{alertText}";
                }
                else
                {
                    return $"❌ Sorry, I couldn't find vehicle {vehicleNumber}. Please check the vehicle number and try again.";
                }
            }

            // General fleet status
            var availableVehicles = await _context.Vehicles
                .Where(v => v.Status == VehicleStatus.Available && v.IsActive)
                .CountAsync();

            var inTransitVehicles = await _context.Vehicles
                .Where(v => v.Status == VehicleStatus.InTransit && v.IsActive)
                .CountAsync();

            var maintenanceVehicles = await _context.Vehicles
                .Where(v => v.Status == VehicleStatus.Maintenance && v.IsActive)
                .CountAsync();

            return $"🚛 **Fleet Status**\n" +
                   $"Available: {availableVehicles}\n" +
                   $"In Transit: {inTransitVehicles}\n" +
                   $"Maintenance: {maintenanceVehicles}\n\n" +
                   $"To check a specific vehicle, please provide the vehicle number (e.g., 'Where is TRK-001?').";
        }

        private async Task<string> HandleAlertQuery(string message)
        {
            var activeAlerts = await _context.VehicleAlerts
                .Include(a => a.Vehicle)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            var deliveryAlerts = await _context.DeliveryAlerts
                .Include(a => a.Delivery)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            if (!activeAlerts.Any() && !deliveryAlerts.Any())
            {
                return "✅ **No Active Alerts**\nEverything is running smoothly! No active alerts at the moment.";
            }

            var response = "🚨 **Active Alerts**\n\n";

            if (activeAlerts.Any())
            {
                response += "**Vehicle Alerts:**\n";
                foreach (var alert in activeAlerts)
                {
                    response += $"• {alert.Title} - {alert.Vehicle?.VehicleNumber} ({alert.Severity})\n";
                }
                response += "\n";
            }

            if (deliveryAlerts.Any())
            {
                response += "**Delivery Alerts:**\n";
                foreach (var alert in deliveryAlerts)
                {
                    response += $"• {alert.Title} - {alert.Delivery?.TrackingNumber} ({alert.Severity})\n";
                }
            }

            return response;
        }

        private string GetHelpResponse()
        {
            return "🤖 **Cargo Assistant Help**\n\n" +
                   "I can help you with:\n\n" +
                   "**📦 Delivery Tracking**\n" +
                   "• 'Track delivery TRK123456789'\n" +
                   "• 'Where is my delivery?'\n" +
                   "• 'Delivery status'\n\n" +
                   "**🚛 Vehicle Information**\n" +
                   "• 'Where is vehicle TRK-001?'\n" +
                   "• 'Vehicle status'\n" +
                   "• 'Fleet overview'\n\n" +
                   "**🚨 Alerts & Issues**\n" +
                   "• 'Any alerts today?'\n" +
                   "• 'Show me problems'\n" +
                   "• 'Vehicle issues'\n\n" +
                   "**📊 General Information**\n" +
                   "• 'How many deliveries today?'\n" +
                   "• 'Fleet status'\n" +
                   "• 'What can you do?'\n\n" +
                   "Just ask me anything about your cargo operations!";
        }

        private string ExtractTrackingNumber(string message)
        {
            // Look for tracking number pattern (TRK followed by numbers)
            var match = System.Text.RegularExpressions.Regex.Match(message, @"TRK\d+");
            return match.Success ? match.Value : string.Empty;
        }

        private string ExtractVehicleNumber(string message)
        {
            // Look for vehicle number pattern (TRK- or VAN- followed by numbers)
            var match = System.Text.RegularExpressions.Regex.Match(message, @"(TRK|VAN)-\d+");
            return match.Success ? match.Value : string.Empty;
        }
    }
} 