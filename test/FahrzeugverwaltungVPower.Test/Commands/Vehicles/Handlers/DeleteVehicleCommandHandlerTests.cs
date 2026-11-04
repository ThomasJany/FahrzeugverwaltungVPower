using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;
using FahrzeugverwaltungVPower.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Handlers;

[TestClass]
public class DeleteVehicleCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithExistingVin_DeletesVehicleAndReturnsTrue()
    {
        using TestDatabase database = new();
        await database.AddAsync(
            VehicleTestData.CreateVehicle(VehicleTestData.Vin),
            VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45"));
        DeleteVehicleCommandHandler handler = new(database.Context);

        // Kleinschreibung und Leerzeichen müssen normalisiert werden.
        bool deleted = await handler.Handle(new DeleteVehicleCommand(" tmbzzz1kzaw123456 "), CancellationToken.None);

        deleted.ShouldBeTrue();
        List<string> remaining = await database.Db.Vehicles.Select(v => v.VIN).ToListAsync();
        remaining.ShouldBe([VehicleTestData.OtherVin]);
        await database.Context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WithUnknownVin_ReturnsFalseAndDoesNotSave()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        DeleteVehicleCommandHandler handler = new(database.Context);

        bool deleted = await handler.Handle(new DeleteVehicleCommand(VehicleTestData.OtherVin), CancellationToken.None);

        deleted.ShouldBeFalse();
        (await database.Db.Vehicles.CountAsync()).ShouldBe(1);
        await database.Context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
