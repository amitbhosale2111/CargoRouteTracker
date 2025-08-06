using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CargoRouteTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddFuelCostManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageFuelEfficiency",
                table: "Vehicles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentFuelLevel",
                table: "Vehicles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelCapacity",
                table: "Vehicles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastFuelUpdate",
                table: "Vehicles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalFuelCost",
                table: "Vehicles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CostAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    FuelConsumption = table.Column<double>(type: "float", nullable: false),
                    FuelCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaintenanceCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperationalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistanceTraveled = table.Column<double>(type: "float", nullable: false),
                    FuelEfficiency = table.Column<double>(type: "float", nullable: false),
                    CostPerKm = table.Column<double>(type: "float", nullable: false),
                    EnergyScore = table.Column<double>(type: "float", nullable: false),
                    CostSavingsPercentage = table.Column<double>(type: "float", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostAnalytics_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnergyManagement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    IdleTimePercentage = table.Column<double>(type: "float", nullable: false),
                    AggressiveDrivingScore = table.Column<double>(type: "float", nullable: false),
                    RouteOptimizationScore = table.Column<double>(type: "float", nullable: false),
                    MaintenanceScore = table.Column<double>(type: "float", nullable: false),
                    OverallEfficiencyScore = table.Column<double>(type: "float", nullable: false),
                    ImprovementSuggestions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnergyManagement_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FuelRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    FuelAmount = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RefuelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OdometerReading = table.Column<double>(type: "float", nullable: true),
                    DistanceTraveled = table.Column<double>(type: "float", nullable: true),
                    FuelEfficiency = table.Column<double>(type: "float", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelRecords_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostAnalytics_VehicleId",
                table: "CostAnalytics",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyManagement_VehicleId",
                table: "EnergyManagement",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRecords_VehicleId",
                table: "FuelRecords",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostAnalytics");

            migrationBuilder.DropTable(
                name: "EnergyManagement");

            migrationBuilder.DropTable(
                name: "FuelRecords");

            migrationBuilder.DropColumn(
                name: "AverageFuelEfficiency",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CurrentFuelLevel",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FuelCapacity",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastFuelUpdate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TotalFuelCost",
                table: "Vehicles");
        }
    }
}
