using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CargoRouteTracker.Data;
using CargoRouteTracker.Models;

namespace CargoRouteTracker.Controllers
{
    public class DeliveriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeliveriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var deliveries = await _context.Deliveries
                .Include(d => d.Vehicle)
                .Include(d => d.Customer)
                .OrderByDescending(d => d.ScheduledPickupTime)
                .ToListAsync();

            return View(deliveries);
        }

        public async Task<IActionResult> Details(int id)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Vehicle)
                .Include(d => d.Customer)
                .Include(d => d.Alerts.OrderByDescending(a => a.CreatedAt))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
                .ToListAsync();
            
            ViewBag.Customers = await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrackingNumber,PickupAddress,DeliveryAddress,PickupLatitude,PickupLongitude,DeliveryLatitude,DeliveryLongitude,ScheduledPickupTime,ScheduledDeliveryTime,Notes,Priority,VehicleId,CustomerId")] Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                delivery.Status = DeliveryStatus.Scheduled;
                delivery.TrackingNumber = GenerateTrackingNumber();
                
                _context.Add(delivery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive && v.Status == VehicleStatus.Available)
                .ToListAsync();
            
            ViewBag.Customers = await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(delivery);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .ToListAsync();
            
            ViewBag.Customers = await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrackingNumber,PickupAddress,DeliveryAddress,PickupLatitude,PickupLongitude,DeliveryLatitude,DeliveryLongitude,ScheduledPickupTime,ScheduledDeliveryTime,Notes,Priority,VehicleId,CustomerId,Status")] Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryExists(delivery.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .ToListAsync();
            
            ViewBag.Customers = await _context.Customers
                .Where(c => c.IsActive)
                .ToListAsync();

            return View(delivery);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, DeliveryStatus status)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
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
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> AssignVehicle(int deliveryId, int vehicleId)
        {
            var delivery = await _context.Deliveries.FindAsync(deliveryId);
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);

            if (delivery != null && vehicle != null)
            {
                delivery.VehicleId = vehicleId;
                delivery.Status = DeliveryStatus.Assigned;
                vehicle.Status = VehicleStatus.InTransit;

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public async Task<IActionResult> Track(string trackingNumber)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Vehicle)
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.TrackingNumber == trackingNumber);

            if (delivery == null)
            {
                return NotFound();
            }

            return View(delivery);
        }

        private string GenerateTrackingNumber()
        {
            return "TRK" + DateTime.Now.ToString("yyyyMMdd") + Random.Shared.Next(1000, 9999);
        }

        private bool DeliveryExists(int id)
        {
            return _context.Deliveries.Any(e => e.Id == id);
        }
    }
} 