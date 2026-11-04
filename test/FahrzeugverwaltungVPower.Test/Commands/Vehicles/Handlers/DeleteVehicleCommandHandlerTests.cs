using FahrzeugverwaltungVPower.Core.Entities;
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
    public async Task Handle_WithExistingId_DeletesOnlyThatVehicleAndReturnsTrue()
    {
        using TestDatabase database = new();
        Vehicle first = VehicleTestData.CreateVehicle(VehicleTestData.Vin);
        Vehicle second = VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45");
        await database.AddAsync(first, second);
        DeleteVehicleCommandHandler handler = new(database.Context);

        bool deleted = await handler.Handle(new DeleteVehicleCommand(first.Id), CancellationToken.None);

        deleted.ShouldBeTrue();
        List<Guid> remaining = await database.Db.Vehicles.Select(v => v.Id).ToListAsync();
        remaining.ShouldBe([second.Id]);
        await database.Context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsFalseAndDoesNotSave()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        DeleteVehicleCommandHandler handler = new(database.Context);

        bool deleted = await handler.Handle(new DeleteVehicleCommand(Guid.NewGuid()), CancellationToken.None);

        deleted.ShouldBeFalse();
        (await database.Db.Vehicles.CountAsync()).ShouldBe(1);
        await database.Context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}