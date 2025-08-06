# Cargo Route Tracker - MVP

A minimal viable product (MVP) web application for real-time cargo delivery tracking and fleet management built with .NET Core MVC and SQL Server.

## Features

### 🚛 Live Fleet Tracker
- Real-time GPS tracking of delivery vehicles
- Interactive map view (Google Maps integration ready)
- Vehicle status monitoring (Available, InTransit, Delivering, Maintenance, Offline)

### 🚨 Alert System
- Proactive notifications for delays, rerouting, and maintenance
- Real-time alerts via SignalR
- Alert severity levels (Low, Medium, High, Critical)
- Vehicle and delivery-specific alerts

### 📊 Dashboard
- Centralized view for dispatchers with KPIs
- Real-time delivery statistics
- Fleet overview and status
- Recent deliveries and alerts

### 📦 Delivery Management
- Complete delivery lifecycle tracking
- Customer management
- Route optimization tools (framework ready)
- Priority-based delivery scheduling

## Technology Stack

- **Backend**: .NET Core 9.0 MVC
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core
- **Real-time Communication**: SignalR
- **Frontend**: Bootstrap 5, Font Awesome, JavaScript
- **Maps**: Google Maps API (ready for integration)

## Getting Started

### Prerequisites

- .NET Core 9.0 SDK
- SQL Server LocalDB (included with Visual Studio)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CargoRouteTracker
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Create and update the database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
   - The dashboard will be the default landing page

### Database Configuration

The application uses LocalDB by default. To change the database connection:

1. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=your-server;Database=CargoRouteTracker;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

2. Run the migration:
   ```bash
   dotnet ef database update
   ```

## Project Structure

```
CargoRouteTracker/
├── Controllers/           # MVC Controllers
│   ├── DashboardController.cs
│   ├── VehiclesController.cs
│   ├── DeliveriesController.cs
│   └── CustomersController.cs
├── Models/               # Entity Models
│   ├── Vehicle.cs
│   ├── Delivery.cs
│   ├── Customer.cs
│   ├── VehicleAlert.cs
│   └── DeliveryAlert.cs
├── Data/                 # Data Access
│   └── ApplicationDbContext.cs
├── Hubs/                 # SignalR Hubs
│   └── TrackingHub.cs
├── Views/                # Razor Views
│   ├── Dashboard/
│   ├── Vehicles/
│   ├── Deliveries/
│   └── Customers/
└── wwwroot/             # Static Files
```

## Key Features Implementation

### Real-time Tracking
- SignalR hub for real-time communication
- Vehicle location updates
- Delivery status changes
- Live alerts and notifications

### Database Schema
- **Vehicles**: Fleet management with GPS coordinates
- **Deliveries**: Complete delivery lifecycle
- **Customers**: Customer information and history
- **Alerts**: Vehicle and delivery alerts with severity levels

### API Endpoints
- `GET /Dashboard` - Main dashboard with KPIs
- `GET /Dashboard/GetVehicleLocations` - Vehicle locations for map
- `GET /Dashboard/GetDeliveryStats` - Delivery statistics
- `POST /Vehicles/UpdateLocation` - Update vehicle GPS
- `POST /Deliveries/UpdateStatus` - Update delivery status

## Google Maps Integration

To enable full map functionality:

1. Get a Google Maps API key from the [Google Cloud Console](https://console.cloud.google.com/)
2. Update the script tag in `Views/Dashboard/Index.cshtml`:
   ```html
   <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=geometry"></script>
   ```
3. Uncomment the map initialization code in the JavaScript section

## Future Enhancements

### Phase 2 Features
- [ ] Route optimization algorithms
- [ ] Driver mobile app integration
- [ ] Customer portal for tracking
- [ ] Advanced analytics and reporting
- [ ] Weather integration for route planning
- [ ] Fuel consumption tracking
- [ ] Maintenance scheduling

### Phase 3 Features
- [ ] Machine learning for delivery time prediction
- [ ] Automated route assignment
- [ ] Integration with external logistics APIs
- [ ] Mobile-responsive driver interface
- [ ] Advanced notification system (SMS, Email)

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation in the `/docs` folder

---

**Built with ❤️ using .NET Core and modern web technologies** 