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
    public async Task Handle_WithExistingVin_ReturnsVehicle()
    {
        using TestDatabase database = new();
        await database.AddAsync(
            VehicleTestData.CreateVehicle(VehicleTestData.Vin),
            VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45"));
        GetVehicleQueryHandler handler = new(database.Context);

        // Kleinschreibung und Leerzeichen müssen normalisiert werden.
        Vehicle? result = await handler.Handle(new GetVehicleQuery(" tmbzzz1kzaw123456 "), CancellationToken.None);

        result.ShouldNotBeNull();
        result.VIN.ShouldBe(VehicleTestData.Vin);
        result.LicensePlate.ShouldBe("HVL AB 123");
    }

    [TestMethod]
    public async Task Handle_WithUnknownVin_ReturnsNull()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        GetVehicleQueryHandler handler = new(database.Context);

        Vehicle? result = await handler.Handle(new GetVehicleQuery(VehicleTestData.OtherVin), CancellationToken.None);

        result.ShouldBeNull();
    }
}
