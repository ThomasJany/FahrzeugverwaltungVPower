using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;
using FahrzeugverwaltungVPower.Test.Helpers;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Queries.Vehicles.Handlers;

[TestClass]
public class GetAllVehiclesQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WithStoredVehicles_ReturnsAllVehicles()
    {
        using TestDatabase database = new();
        await database.AddAsync(
            VehicleTestData.CreateVehicle(VehicleTestData.Vin, "HVL AB 123"),
            VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45"));
        GetAllVehiclesQueryHandler handler = new(database.Context);

        ICollection<Vehicle> result = await handler.Handle(new GetAllVehiclesQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
        result.Select(v => v.VIN).ShouldBe([VehicleTestData.Vin, VehicleTestData.OtherVin], ignoreOrder: true);
    }

    [TestMethod]
    public async Task Handle_WithoutVehicles_ReturnsEmptyList()
    {
        using TestDatabase database = new();
        GetAllVehiclesQueryHandler handler = new(database.Context);

        ICollection<Vehicle> result = await handler.Handle(new GetAllVehiclesQuery(), CancellationToken.None);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }
}
