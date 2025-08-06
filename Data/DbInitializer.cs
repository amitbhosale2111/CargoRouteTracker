using CargoRouteTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoRouteTracker.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Customers.Any())
            {
                return; // Database has been seeded
            }

            // Seed Customers
            var customers = new Customer[]
            {
                new Customer { Name = "John Smith", Email = "john.smith@email.com", PhoneNumber = "555-0101", Address = "123 Main St, New York, NY 10001", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Customer { Name = "Jane Doe", Email = "jane.doe@email.com", PhoneNumber = "555-0102", Address = "456 Oak Ave, Los Angeles, CA 90210", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Customer { Name = "Bob Johnson", Email = "bob.johnson@email.com", PhoneNumber = "555-0103", Address = "789 Pine Rd, Chicago, IL 60601", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Customer { Name = "Alice Brown", Email = "alice.brown@email.com", PhoneNumber = "555-0104", Address = "321 Elm St, Houston, TX 77001", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Customer { Name = "Charlie Wilson", Email = "charlie.wilson@email.com", PhoneNumber = "555-0105", Address = "654 Maple Dr, Phoenix, AZ 85001", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Customer { Name = "Diana Davis", Email = "diana.davis@email.com", PhoneNumber = "555-0106", Address = "987 Cedar Ln, Philadelphia, PA 19101", IsActive = true, CreatedDate = DateTime.UtcNow }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();

            // Seed Vehicles
            var vehicles = new Vehicle[]
            {
                new Vehicle 
                { 
                    VehicleNumber = "TRK-001", 
                    DriverName = "Mike Wilson", 
                    PhoneNumber = "555-0201", 
                    VehicleType = "Truck",
                    CurrentLatitude = 40.7128,
                    CurrentLongitude = -74.0060,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.Available,
                    IsActive = true
                },
                new Vehicle 
                { 
                    VehicleNumber = "VAN-001", 
                    DriverName = "Sarah Davis", 
                    PhoneNumber = "555-0202", 
                    VehicleType = "Van",
                    CurrentLatitude = 34.0522,
                    CurrentLongitude = -118.2437,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.InTransit,
                    IsActive = true
                },
                new Vehicle 
                { 
                    VehicleNumber = "TRK-002", 
                    DriverName = "Tom Brown", 
                    PhoneNumber = "555-0203", 
                    VehicleType = "Truck",
                    CurrentLatitude = 41.8781,
                    CurrentLongitude = -87.6298,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.Delivering,
                    IsActive = true
                },
                new Vehicle 
                { 
                    VehicleNumber = "VAN-002", 
                    DriverName = "Lisa Anderson", 
                    PhoneNumber = "555-0204", 
                    VehicleType = "Van",
                    CurrentLatitude = 29.7604,
                    CurrentLongitude = -95.3698,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.Available,
                    IsActive = true
                },
                new Vehicle 
                { 
                    VehicleNumber = "TRK-003", 
                    DriverName = "David Miller", 
                    PhoneNumber = "555-0205", 
                    VehicleType = "Truck",
                    CurrentLatitude = 33.4484,
                    CurrentLongitude = -112.0740,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.Maintenance,
                    IsActive = true
                },
                new Vehicle 
                { 
                    VehicleNumber = "VAN-003", 
                    DriverName = "Emma Taylor", 
                    PhoneNumber = "555-0206", 
                    VehicleType = "Van",
                    CurrentLatitude = 39.9526,
                    CurrentLongitude = -75.1652,
                    LastLocationUpdate = DateTime.UtcNow,
                    Status = VehicleStatus.Offline,
                    IsActive = false
                }
            };

            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();

            // Seed Deliveries
            var deliveries = new Delivery[]
            {
                new Delivery
                {
                    TrackingNumber = "TRK123456789",
                    PickupAddress = "123 Main St, New York, NY 10001",
                    DeliveryAddress = "456 Oak Ave, Brooklyn, NY 11201",
                    PickupLatitude = 40.7128,
                    PickupLongitude = -74.0060,
                    DeliveryLatitude = 40.7182,
                    DeliveryLongitude = -73.9584,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(1),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(3),
                    Status = DeliveryStatus.Assigned,
                    Priority = 2,
                    Notes = "Fragile items - handle with care",
                    VehicleId = vehicles[0].Id,
                    CustomerId = customers[0].Id
                },
                new Delivery
                {
                    TrackingNumber = "TRK987654321",
                    PickupAddress = "789 Pine Rd, Chicago, IL 60601",
                    DeliveryAddress = "321 Elm St, Evanston, IL 60201",
                    PickupLatitude = 41.8781,
                    PickupLongitude = -87.6298,
                    DeliveryLatitude = 42.0451,
                    DeliveryLongitude = -87.6877,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(2),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(4),
                    Status = DeliveryStatus.InTransit,
                    Priority = 3,
                    Notes = "Express delivery - customer waiting",
                    VehicleId = vehicles[1].Id,
                    CustomerId = customers[1].Id
                },
                new Delivery
                {
                    TrackingNumber = "TRK456789123",
                    PickupAddress = "654 Maple Dr, Phoenix, AZ 85001",
                    DeliveryAddress = "987 Cedar Ln, Scottsdale, AZ 85250",
                    PickupLatitude = 33.4484,
                    PickupLongitude = -112.0740,
                    DeliveryLatitude = 33.4942,
                    DeliveryLongitude = -111.9261,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(-1),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(1),
                    Status = DeliveryStatus.OutForDelivery,
                    Priority = 1,
                    Notes = "Standard delivery",
                    VehicleId = vehicles[2].Id,
                    CustomerId = customers[2].Id
                },
                new Delivery
                {
                    TrackingNumber = "TRK789123456",
                    PickupAddress = "321 Elm St, Houston, TX 77001",
                    DeliveryAddress = "654 Maple Dr, Sugar Land, TX 77478",
                    PickupLatitude = 29.7604,
                    PickupLongitude = -95.3698,
                    DeliveryLatitude = 29.6197,
                    DeliveryLongitude = -95.6349,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(3),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(5),
                    Status = DeliveryStatus.Scheduled,
                    Priority = 2,
                    Notes = "Large package - requires lift gate",
                    VehicleId = vehicles[3].Id,
                    CustomerId = customers[3].Id
                },
                new Delivery
                {
                    TrackingNumber = "TRK321654987",
                    PickupAddress = "987 Cedar Ln, Philadelphia, PA 19101",
                    DeliveryAddress = "123 Main St, Wilmington, DE 19801",
                    PickupLatitude = 39.9526,
                    PickupLongitude = -75.1652,
                    DeliveryLatitude = 39.7447,
                    DeliveryLongitude = -75.5484,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(-2),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(-1),
                    ActualPickupTime = DateTime.UtcNow.AddHours(-2),
                    ActualDeliveryTime = DateTime.UtcNow.AddHours(-1),
                    Status = DeliveryStatus.Delivered,
                    Priority = 1,
                    Notes = "Successfully delivered",
                    VehicleId = vehicles[0].Id,
                    CustomerId = customers[4].Id
                },
                new Delivery
                {
                    TrackingNumber = "TRK147258369",
                    PickupAddress = "456 Oak Ave, Los Angeles, CA 90210",
                    DeliveryAddress = "789 Pine Rd, Beverly Hills, CA 90210",
                    PickupLatitude = 34.0522,
                    PickupLongitude = -118.2437,
                    DeliveryLatitude = 34.0736,
                    DeliveryLongitude = -118.4004,
                    ScheduledPickupTime = DateTime.UtcNow.AddHours(-3),
                    ScheduledDeliveryTime = DateTime.UtcNow.AddHours(-1),
                    Status = DeliveryStatus.Failed,
                    Priority = 3,
                    Notes = "Customer not available - reschedule needed",
                    VehicleId = vehicles[1].Id,
                    CustomerId = customers[5].Id
                }
            };

            context.Deliveries.AddRange(deliveries);
            context.SaveChanges();

            // Seed Vehicle Alerts
            var vehicleAlerts = new VehicleAlert[]
            {
                new VehicleAlert
                {
                    Title = "Low Fuel Warning",
                    Message = "Vehicle TRK-001 fuel level is below 20%. Please refuel soon.",
                    Type = AlertType.FuelLow,
                    Severity = AlertSeverity.Medium,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    IsResolved = false,
                    VehicleId = vehicles[0].Id
                },
                new VehicleAlert
                {
                    Title = "Maintenance Due",
                    Message = "Vehicle TRK-003 requires scheduled maintenance. Oil change and inspection needed.",
                    Type = AlertType.Maintenance,
                    Severity = AlertSeverity.High,
                    CreatedAt = DateTime.UtcNow.AddHours(-4),
                    IsResolved = false,
                    VehicleId = vehicles[4].Id
                },
                new VehicleAlert
                {
                    Title = "Traffic Delay",
                    Message = "Vehicle VAN-001 experiencing traffic delay on I-95. Estimated 30-minute delay.",
                    Type = AlertType.Traffic,
                    Severity = AlertSeverity.Low,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    IsResolved = false,
                    VehicleId = vehicles[1].Id
                },
                new VehicleAlert
                {
                    Title = "Engine Warning",
                    Message = "Vehicle TRK-002 engine temperature is high. Immediate attention required.",
                    Type = AlertType.EngineIssue,
                    Severity = AlertSeverity.Critical,
                    CreatedAt = DateTime.UtcNow.AddHours(-30),
                    IsResolved = true,
                    ResolvedAt = DateTime.UtcNow.AddHours(-25),
                    VehicleId = vehicles[2].Id
                },
                new VehicleAlert
                {
                    Title = "Route Change",
                    Message = "Vehicle VAN-002 route updated due to road closure on Main St.",
                    Type = AlertType.RouteChange,
                    Severity = AlertSeverity.Low,
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    IsResolved = false,
                    VehicleId = vehicles[3].Id
                }
            };

            context.VehicleAlerts.AddRange(vehicleAlerts);
            context.SaveChanges();

            // Seed Delivery Alerts
            var deliveryAlerts = new DeliveryAlert[]
            {
                new DeliveryAlert
                {
                    Title = "Delivery Delay",
                    Message = "Delivery TRK123456789 delayed due to traffic. New ETA: 2:30 PM",
                    Type = DeliveryAlertType.Delay,
                    Severity = AlertSeverity.Medium,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    IsResolved = false,
                    DeliveryId = deliveries[0].Id
                },
                new DeliveryAlert
                {
                    Title = "Customer Not Available",
                    Message = "Customer not available for delivery TRK147258369. Attempted delivery at 1:00 PM.",
                    Type = DeliveryAlertType.CustomerNotAvailable,
                    Severity = AlertSeverity.High,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    IsResolved = false,
                    DeliveryId = deliveries[5].Id
                },
                new DeliveryAlert
                {
                    Title = "Address Issue",
                    Message = "Delivery address for TRK456789123 could not be located. GPS coordinates may be incorrect.",
                    Type = DeliveryAlertType.AddressIssue,
                    Severity = AlertSeverity.High,
                    CreatedAt = DateTime.UtcNow.AddHours(-3),
                    IsResolved = false,
                    DeliveryId = deliveries[2].Id
                },
                new DeliveryAlert
                {
                    Title = "Weather Delay",
                    Message = "Delivery TRK987654321 delayed due to severe weather conditions.",
                    Type = DeliveryAlertType.WeatherDelay,
                    Severity = AlertSeverity.Medium,
                    CreatedAt = DateTime.UtcNow.AddHours(-4),
                    IsResolved = false,
                    DeliveryId = deliveries[1].Id
                }
            };

            context.DeliveryAlerts.AddRange(deliveryAlerts);
            context.SaveChanges();

            // Update vehicles with fuel data
            foreach (var vehicle in vehicles)
            {
                vehicle.CurrentFuelLevel = Random.Shared.Next(20, 95);
                vehicle.FuelCapacity = Random.Shared.Next(80, 150);
                vehicle.AverageFuelEfficiency = Random.Shared.Next(8, 15) + Random.Shared.NextDouble();
                vehicle.TotalFuelCost = Random.Shared.Next(500, 2000);
                vehicle.LastFuelUpdate = DateTime.UtcNow.AddHours(-Random.Shared.Next(1, 24));
            }
            context.SaveChanges();

            // Seed Fuel Records
            var fuelRecords = new FuelRecord[]
            {
                new FuelRecord
                {
                    VehicleId = vehicles[0].Id,
                    FuelAmount = 45.5,
                    Cost = 85.50m,
                    RefuelDate = DateTime.UtcNow.AddDays(-2),
                    Location = "Shell Station, Downtown",
                    FuelType = "Diesel",
                    OdometerReading = 125000,
                    DistanceTraveled = 450,
                    FuelEfficiency = 9.9,
                    Notes = "Regular refuel"
                },
                new FuelRecord
                {
                    VehicleId = vehicles[1].Id,
                    FuelAmount = 38.2,
                    Cost = 72.80m,
                    RefuelDate = DateTime.UtcNow.AddDays(-1),
                    Location = "BP Station, Highway 101",
                    FuelType = "Diesel",
                    OdometerReading = 89000,
                    DistanceTraveled = 380,
                    FuelEfficiency = 9.9,
                    Notes = "Highway driving"
                },
                new FuelRecord
                {
                    VehicleId = vehicles[2].Id,
                    FuelAmount = 52.0,
                    Cost = 98.00m,
                    RefuelDate = DateTime.UtcNow.AddDays(-3),
                    Location = "Exxon Station, Industrial Area",
                    FuelType = "Diesel",
                    OdometerReading = 156000,
                    DistanceTraveled = 520,
                    FuelEfficiency = 10.0,
                    Notes = "Heavy load delivery"
                }
            };

            context.FuelRecords.AddRange(fuelRecords);
            context.SaveChanges();

            // Seed Cost Analytics
            var costAnalytics = new CostAnalytics[]
            {
                new CostAnalytics
                {
                    VehicleId = vehicles[0].Id,
                    Date = DateTime.Today,
                    FuelConsumption = 45.5,
                    FuelCost = 85.50m,
                    MaintenanceCost = 120.00m,
                    OperationalCost = 200.00m,
                    TotalCost = 405.50m,
                    DistanceTraveled = 450,
                    FuelEfficiency = 9.9,
                    CostPerKm = 0.90,
                    EnergyScore = 85,
                    CostSavingsPercentage = 8.2,
                    Recommendations = "Optimize route planning to reduce fuel consumption by 5%"
                },
                new CostAnalytics
                {
                    VehicleId = vehicles[1].Id,
                    Date = DateTime.Today,
                    FuelConsumption = 38.2,
                    FuelCost = 72.80m,
                    MaintenanceCost = 95.00m,
                    OperationalCost = 180.00m,
                    TotalCost = 347.80m,
                    DistanceTraveled = 380,
                    FuelEfficiency = 9.9,
                    CostPerKm = 0.92,
                    EnergyScore = 82,
                    CostSavingsPercentage = 7.8,
                    Recommendations = "Reduce idle time to improve fuel efficiency"
                },
                new CostAnalytics
                {
                    VehicleId = vehicles[2].Id,
                    Date = DateTime.Today,
                    FuelConsumption = 52.0,
                    FuelCost = 98.00m,
                    MaintenanceCost = 150.00m,
                    OperationalCost = 250.00m,
                    TotalCost = 498.00m,
                    DistanceTraveled = 520,
                    FuelEfficiency = 10.0,
                    CostPerKm = 0.96,
                    EnergyScore = 88,
                    CostSavingsPercentage = 9.1,
                    Recommendations = "Excellent performance, maintain current driving habits"
                }
            };

            context.CostAnalytics.AddRange(costAnalytics);
            context.SaveChanges();

            // Seed Energy Management
            var energyManagement = new EnergyManagement[]
            {
                new EnergyManagement
                {
                    VehicleId = vehicles[0].Id,
                    Date = DateTime.Today,
                    IdleTimePercentage = 15.2,
                    AggressiveDrivingScore = 25.0,
                    RouteOptimizationScore = 85.0,
                    MaintenanceScore = 90.0,
                    OverallEfficiencyScore = 85.0,
                    ImprovementSuggestions = "Reduce idle time by 5% to achieve 12% fuel cost reduction"
                },
                new EnergyManagement
                {
                    VehicleId = vehicles[1].Id,
                    Date = DateTime.Today,
                    IdleTimePercentage = 18.5,
                    AggressiveDrivingScore = 30.0,
                    RouteOptimizationScore = 80.0,
                    MaintenanceScore = 85.0,
                    OverallEfficiencyScore = 82.0,
                    ImprovementSuggestions = "Implement eco-driving techniques to reduce aggressive driving score"
                },
                new EnergyManagement
                {
                    VehicleId = vehicles[2].Id,
                    Date = DateTime.Today,
                    IdleTimePercentage = 12.0,
                    AggressiveDrivingScore = 20.0,
                    RouteOptimizationScore = 90.0,
                    MaintenanceScore = 95.0,
                    OverallEfficiencyScore = 88.0,
                    ImprovementSuggestions = "Maintain current excellent performance standards"
                }
            };

            context.EnergyManagement.AddRange(energyManagement);
            context.SaveChanges();
        }
    }
} 