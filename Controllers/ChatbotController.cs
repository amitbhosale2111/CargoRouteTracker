using Microsoft.AspNetCore.Mvc;
using CargoRouteTracker.Services;
using CargoRouteTracker.Models;
using CargoRouteTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace CargoRouteTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;
        private readonly ApplicationDbContext _context;

        public ChatbotController(IChatbotService chatbotService, ApplicationDbContext context)
        {
            _chatbotService = chatbotService;
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message cannot be empty");
            }

            // Save user message
            var userMessage = new ChatMessage
            {
                Message = request.Message,
                Type = MessageType.User,
                SessionId = request.SessionId ?? Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(userMessage);
            await _context.SaveChangesAsync();

            // Process message and get bot response
            var botResponse = await _chatbotService.ProcessMessageAsync(request.Message, userMessage.SessionId);

            return Ok(new ChatResponse
            {
                BotMessage = botResponse.Message,
                SessionId = userMessage.SessionId,
                Timestamp = botResponse.Timestamp
            });
        }

        [HttpGet("history/{sessionId}")]
        public async Task<IActionResult> GetChatHistory(string sessionId)
        {
            var messages = await _chatbotService.GetChatHistoryAsync(sessionId);
            return Ok(messages);
        }

        [HttpGet("quick-stats")]
        public async Task<IActionResult> GetQuickStats()
        {
            var stats = new
            {
                TotalVehicles = await _context.Vehicles.CountAsync(v => v.IsActive),
                ActiveDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.Status == DeliveryStatus.InTransit || d.Status == DeliveryStatus.OutForDelivery),
                ActiveAlerts = await _context.VehicleAlerts.CountAsync(a => !a.IsResolved) +
                              await _context.DeliveryAlerts.CountAsync(a => !a.IsResolved),
                CompletedToday = await _context.Deliveries.CountAsync(d => 
                    d.ActualDeliveryTime.HasValue && d.ActualDeliveryTime.Value.Date == DateTime.Today)
            };

            return Ok(stats);
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }

    public class ChatResponse
    {
        public string BotMessage { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
} 