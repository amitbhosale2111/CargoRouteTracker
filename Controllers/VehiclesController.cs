using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CargoRouteTracker.Data;
using CargoRouteTracker.Models;

namespace CargoRouteTracker.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Deliveries.Where(d => d.Status != DeliveryStatus.Delivered))
                .Include(v => v.Alerts.Where(a => !a.IsResolved))
                .OrderBy(v => v.VehicleNumber)
                .ToListAsync();

            return View(vehicles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Deliveries)
                .ThenInclude(d => d.Customer)
                .Include(v => v.Alerts.OrderByDescending(a => a.CreatedAt))
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleNumber,DriverName,PhoneNumber,VehicleType")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                vehicle.Status = VehicleStatus.Available;
                vehicle.IsActive = true;
                vehicle.LastLocationUpdate = DateTime.UtcNow;
                
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleNumber,DriverName,PhoneNumber,VehicleType,Status,IsActive")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
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
            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocation(int id, double latitude, double longitude)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                vehicle.CurrentLatitude = latitude;
                vehicle.CurrentLongitude = longitude;
                vehicle.LastLocationUpdate = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, VehicleStatus status)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                vehicle.Status = status;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
} 