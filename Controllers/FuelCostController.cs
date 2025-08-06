using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CargoRouteTracker.Data;
using CargoRouteTracker.Models;

namespace CargoRouteTracker.Controllers
{
    public class FuelCostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FuelCostController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new FuelCostViewModel
            {
                TotalFuelCost = await _context.FuelRecords
                    .Where(f => f.RefuelDate >= DateTime.Today.AddDays(-30))
                    .SumAsync(f => f.Cost),
                
                TotalFuelConsumption = await _context.FuelRecords
                    .Where(f => f.RefuelDate >= DateTime.Today.AddDays(-30))
                    .SumAsync(f => f.FuelAmount),
                
                AverageFuelEfficiency = await _context.FuelRecords
                    .Where(f => f.FuelEfficiency.HasValue && f.RefuelDate >= DateTime.Today.AddDays(-30))
                    .AverageAsync(f => f.FuelEfficiency) ?? 0,
                
                CostSavings = await CalculateCostSavings(),
                
                FuelRecords = await _context.FuelRecords
                    .Include(f => f.Vehicle)
                    .OrderByDescending(f => f.RefuelDate)
                    .Take(10)
                    .ToListAsync(),
                
                CostAnalytics = await _context.CostAnalytics
                    .Include(c => c.Vehicle)
                    .OrderByDescending(c => c.Date)
                    .Take(10)
                    .ToListAsync(),
                
                EnergyManagement = await _context.EnergyManagement
                    .Include(e => e.Vehicle)
                    .OrderByDescending(e => e.Date)
                    .Take(10)
                    .ToListAsync(),
                
                Vehicles = await _context.Vehicles
                    .Where(v => v.IsActive)
                    .Include(v => v.FuelRecords.OrderByDescending(f => f.RefuelDate).Take(1))
                    .ToListAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> FuelRecords()
        {
            var fuelRecords = await _context.FuelRecords
                .Include(f => f.Vehicle)
                .OrderByDescending(f => f.RefuelDate)
                .ToListAsync();

            return View(fuelRecords);
        }

        public async Task<IActionResult> CreateFuelRecord()
        {
            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFuelRecord([Bind("VehicleId,FuelAmount,Cost,RefuelDate,Location,FuelType,OdometerReading,Notes")] FuelRecord fuelRecord)
        {
            if (ModelState.IsValid)
            {
                // Calculate fuel efficiency if odometer reading is provided
                if (fuelRecord.OdometerReading.HasValue)
                {
                    var previousRecord = await _context.FuelRecords
                        .Where(f => f.VehicleId == fuelRecord.VehicleId)
                        .OrderByDescending(f => f.RefuelDate)
                        .FirstOrDefaultAsync();

                    if (previousRecord?.OdometerReading.HasValue == true)
                    {
                        fuelRecord.DistanceTraveled = fuelRecord.OdometerReading.Value - previousRecord.OdometerReading.Value;
                        if (fuelRecord.DistanceTraveled > 0 && fuelRecord.FuelAmount > 0)
                        {
                            fuelRecord.FuelEfficiency = fuelRecord.DistanceTraveled / fuelRecord.FuelAmount;
                        }
                    }
                }

                _context.Add(fuelRecord);
                await _context.SaveChangesAsync();

                // Update vehicle fuel level
                await UpdateVehicleFuelLevel(fuelRecord.VehicleId);

                return RedirectToAction(nameof(FuelRecords));
            }

            ViewBag.Vehicles = await _context.Vehicles
                .Where(v => v.IsActive)
                .ToListAsync();

            return View(fuelRecord);
        }

        public async Task<IActionResult> CostAnalytics()
        {
            var analytics = await _context.CostAnalytics
                .Include(c => c.Vehicle)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return View(analytics);
        }

        public async Task<IActionResult> EnergyManagement()
        {
            var energyData = await _context.EnergyManagement
                .Include(e => e.Vehicle)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            return View(energyData);
        }

        [HttpGet]
        public async Task<IActionResult> GetFuelChartData()
        {
            var last30Days = await _context.FuelRecords
                .Where(f => f.RefuelDate >= DateTime.Today.AddDays(-30))
                .GroupBy(f => f.RefuelDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    FuelAmount = g.Sum(f => f.FuelAmount),
                    Cost = g.Sum(f => f.Cost)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Json(last30Days);
        }

        [HttpGet]
        public async Task<IActionResult> GetCostSavingsData()
        {
            var savings = await _context.CostAnalytics
                .Where(c => c.Date >= DateTime.Today.AddDays(-30))
                .GroupBy(c => c.VehicleId)
                .Select(g => new
                {
                    VehicleId = g.Key,
                    AverageSavings = g.Average(c => c.CostSavingsPercentage),
                    TotalSavings = g.Sum(c => c.TotalCost * (decimal)(c.CostSavingsPercentage / 100))
                })
                .ToListAsync();

            return Json(savings);
        }

        [HttpGet]
        public async Task<IActionResult> GetEnergyEfficiencyData()
        {
            var efficiency = await _context.EnergyManagement
                .Where(e => e.Date >= DateTime.Today.AddDays(-30))
                .GroupBy(e => e.VehicleId)
                .Select(g => new
                {
                    VehicleId = g.Key,
                    AverageEfficiency = g.Average(e => e.OverallEfficiencyScore),
                    IdleTime = g.Average(e => e.IdleTimePercentage),
                    AggressiveDriving = g.Average(e => e.AggressiveDrivingScore)
                })
                .ToListAsync();

            return Json(efficiency);
        }

        private async Task<decimal> CalculateCostSavings()
        {
            // Calculate cost savings compared to baseline
            var baselineCost = 10000m; // Example baseline cost
            var currentCost = await _context.CostAnalytics
                .Where(c => c.Date >= DateTime.Today.AddDays(-30))
                .SumAsync(c => c.TotalCost);

            return baselineCost - currentCost;
        }

        private async Task UpdateVehicleFuelLevel(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle != null)
            {
                var latestRecord = await _context.FuelRecords
                    .Where(f => f.VehicleId == vehicleId)
                    .OrderByDescending(f => f.RefuelDate)
                    .FirstOrDefaultAsync();

                if (latestRecord != null && vehicle.FuelCapacity.HasValue)
                {
                    vehicle.CurrentFuelLevel = (latestRecord.FuelAmount / vehicle.FuelCapacity.Value) * 100;
                    vehicle.LastFuelUpdate = DateTime.UtcNow;
                    vehicle.TotalFuelCost = await _context.FuelRecords
                        .Where(f => f.VehicleId == vehicleId && f.RefuelDate >= DateTime.Today.AddDays(-30))
                        .SumAsync(f => f.Cost);

                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    public class FuelCostViewModel
    {
        public decimal TotalFuelCost { get; set; }
        public double TotalFuelConsumption { get; set; }
        public double AverageFuelEfficiency { get; set; }
        public decimal CostSavings { get; set; }
        public List<FuelRecord> FuelRecords { get; set; } = new List<FuelRecord>();
        public List<CostAnalytics> CostAnalytics { get; set; } = new List<CostAnalytics>();
        public List<EnergyManagement> EnergyManagement { get; set; } = new List<EnergyManagement>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
} 