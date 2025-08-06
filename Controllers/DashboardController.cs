using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CargoRouteTracker.Data;
using CargoRouteTracker.Models;

namespace CargoRouteTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalVehicles = await _context.Vehicles.CountAsync(v => v.IsActive),
                ActiveDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.Status == DeliveryStatus.InTransit || d.Status == DeliveryStatus.OutForDelivery),
                PendingDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.Status == DeliveryStatus.Scheduled || d.Status == DeliveryStatus.Assigned),
                CompletedDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.Status == DeliveryStatus.Delivered),
                ActiveAlerts = await _context.VehicleAlerts.CountAsync(a => !a.IsResolved),
                
                Vehicles = await _context.Vehicles
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.VehicleNumber)
                    .ToListAsync(),
                
                RecentDeliveries = await _context.Deliveries
                    .Include(d => d.Vehicle)
                    .Include(d => d.Customer)
                    .OrderByDescending(d => d.ScheduledPickupTime)
                    .Take(10)
                    .ToListAsync(),
                
                RecentAlerts = await _context.VehicleAlerts
                    .Include(a => a.Vehicle)
                    .Where(a => !a.IsResolved)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleLocations()
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .Select(v => new
                {
                    v.Id,
                    v.VehicleNumber,
                    v.DriverName,
                    v.CurrentLatitude,
                    v.CurrentLongitude,
                    v.Status,
                    v.LastLocationUpdate
                })
                .ToListAsync();

            return Json(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryStats()
        {
            var today = DateTime.Today;
            var stats = new
            {
                TodayDelivered = await _context.Deliveries.CountAsync(d => 
                    d.ActualDeliveryTime.HasValue && d.ActualDeliveryTime.Value.Date == today),
                TodayScheduled = await _context.Deliveries.CountAsync(d => 
                    d.ScheduledDeliveryTime.Date == today),
                OnTimeDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.ActualDeliveryTime.HasValue && 
                    d.ActualDeliveryTime <= d.ScheduledDeliveryTime),
                DelayedDeliveries = await _context.Deliveries.CountAsync(d => 
                    d.ActualDeliveryTime.HasValue && 
                    d.ActualDeliveryTime > d.ScheduledDeliveryTime)
            };

            return Json(stats);
        }
    }

    public class DashboardViewModel
    {
        public int TotalVehicles { get; set; }
        public int ActiveDeliveries { get; set; }
        public int PendingDeliveries { get; set; }
        public int CompletedDeliveries { get; set; }
        public int ActiveAlerts { get; set; }
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<Delivery> RecentDeliveries { get; set; } = new List<Delivery>();
        public List<VehicleAlert> RecentAlerts { get; set; } = new List<VehicleAlert>();
    }
} 