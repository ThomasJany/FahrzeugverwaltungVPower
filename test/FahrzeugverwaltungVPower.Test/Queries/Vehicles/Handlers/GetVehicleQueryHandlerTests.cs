using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;
using FahrzeugverwaltungVPower.Test.Helpers;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Queries.Vehicles.Handlers;

[TestClass]
public class GetVehicleQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_WithExistingId_ReturnsVehicle()
    {
        using TestDatabase database = new();
        Vehicle first = VehicleTestData.CreateVehicle(VehicleTestData.Vin);
        Vehicle second = VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45");
        await database.AddAsync(first, second);
        GetVehicleQueryHandler handler = new(database.Context);

        Vehicle? result = await handler.Handle(new GetVehicleQuery(first.Id), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(first.Id);
        result.VIN.ShouldBe(VehicleTestData.Vin);
        result.LicensePlate.ShouldBe("HVL AB 123");
    }

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsNull()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        GetVehicleQueryHandler handler = new(database.Context);

        Vehicle? result = await handler.Handle(new GetVehicleQuery(Guid.NewGuid()), CancellationToken.None);

        result.ShouldBeNull();
    }
}