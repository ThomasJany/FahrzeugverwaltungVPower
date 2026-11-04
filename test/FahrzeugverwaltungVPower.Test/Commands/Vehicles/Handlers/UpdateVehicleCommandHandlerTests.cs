using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Handlers;

[TestClass]
public class UpdateVehicleCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithExistingId_UpdatesNormalizedFieldsAndReturnsTrue()
    {
        using TestDatabase database = new();
        DateTimeOffset past = DateTimeOffset.UtcNow.AddDays(-7);
        Vehicle existing = VehicleTestData.CreateVehicle() with { CreatedAt = past, ModifiedAt = past };
        await database.AddAsync(existing);
        UpdateVehicleCommandHandler handler = new(database.Context);
        UpdateVehicleCommand command = VehicleTestData.UpdateCommand(existing.Id) with
        {
            LicensePlate = " ohz cd 45 ",
            Holder = " Musterfrau ",
            HolderFirstName = " Erika ",
            VIN = " wvwzzz1kzaw654321 ",
            TSN = " abc123456 "
        };

        bool updated = await handler.Handle(command, CancellationToken.None);

        updated.ShouldBeTrue();
        Vehicle saved = await database.Db.Vehicles.AsNoTracking().SingleAsync();
        saved.Id.ShouldBe(existing.Id);
        saved.LicensePlate.ShouldBe("OHZ CD 45");
        saved.Holder.ShouldBe("Musterfrau");
        saved.HolderFirstName.ShouldBe("Erika");
        saved.VIN.ShouldBe(VehicleTestData.OtherVin);
        saved.TSN.ShouldBe("ABC123456");
        saved.Model.ShouldBe("Passat");
        saved.HSN.ShouldBe("0603");
        saved.CreatedAt.ShouldBe(past);
        saved.CreatedBy.ShouldBe(VehicleTestData.CreatedBy);
        saved.ModifiedAt.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        await database.Context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WithUnknownId_ReturnsFalseAndDoesNotSave()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle());
        UpdateVehicleCommandHandler handler = new(database.Context);

        bool updated = await handler.Handle(VehicleTestData.UpdateCommand(Guid.NewGuid()), CancellationToken.None);

        updated.ShouldBeFalse();
        Vehicle unchanged = await database.Db.Vehicles.AsNoTracking().SingleAsync();
        unchanged.Holder.ShouldBe("Mustermann");
        await database.Context.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}