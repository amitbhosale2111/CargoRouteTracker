using Microsoft.AspNetCore.SignalR;
using CargoRouteTracker.Models;
using CargoRouteTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace CargoRouteTracker.Hubs
{
    public class TrackingHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public TrackingHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateVehicleLocation(int vehicleId, double latitude, double longitude)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.CurrentLatitude = latitude;
                vehicle.CurrentLongitude = longitude;
                vehicle.LastLocationUpdate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();

                // Broadcast location update to all connected clients
                await Clients.All.SendAsync("VehicleLocationUpdated", vehicleId, latitude, longitude, DateTime.UtcNow);
            }
        }

        public async Task UpdateDeliveryStatus(int deliveryId, DeliveryStatus status)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Vehicle)
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.Id == deliveryId);

            if (delivery != null)
            {
                delivery.Status = status;
                
                if (status == DeliveryStatus.Delivered)
                {
                    delivery.ActualDeliveryTime = DateTime.UtcNow;
                }
                else if (status == DeliveryStatus.InTransit)
                {
                    delivery.ActualPickupTime = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Broadcast status update to all connected clients
                await Clients.All.SendAsync("DeliveryStatusUpdated", deliveryId, status.ToString(), DateTime.UtcNow);
            }
        }

        public async Task CreateAlert(int vehicleId, string title, string message, AlertType type, AlertSeverity severity)
        {
            var alert = new VehicleAlert
            {
                Title = title,
                Message = message,
                Type = type,
                Severity = severity,
                VehicleId = vehicleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.VehicleAlerts.Add(alert);
            await _context.SaveChangesAsync();

            // Broadcast alert to all connected clients
            await Clients.All.SendAsync("NewAlert", alert.Id, title, message, type.ToString(), severity.ToString(), DateTime.UtcNow);
        }

        public async Task JoinVehicleGroup(int vehicleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Vehicle_{vehicleId}");
        }

        public async Task LeaveVehicleGroup(int vehicleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Vehicle_{vehicleId}");
        }

        public async Task JoinDeliveryGroup(int deliveryId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Delivery_{deliveryId}");
        }

        public async Task LeaveDeliveryGroup(int deliveryId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Delivery_{deliveryId}");
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
} 