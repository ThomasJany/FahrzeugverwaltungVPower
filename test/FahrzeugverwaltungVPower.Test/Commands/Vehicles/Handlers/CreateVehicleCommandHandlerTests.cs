using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Handlers;

[TestClass]
public class CreateVehicleCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_WithValidCommand_SavesNormalizedVehicleAndReturnsId()
    {
        using TestDatabase database = new();
        CreateVehicleCommandHandler handler = new(database.Context, CreateCurrentUser());
        CreateVehicleCommand command = VehicleTestData.CreateCommand() with
        {
            LicensePlate = " hvl ab 123 ",
            Holder = " Mustermann ",
            HolderFirstName = " Max ",
            VIN = " tmbzzz1kzaw123456 ",
            TSN = " ajo000180 "
        };

        Guid id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);

        Vehicle saved = await database.Db.Vehicles.AsNoTracking().SingleAsync(CancellationToken.None);

        saved.Id.ShouldBe(id);
        saved.LicensePlate.ShouldBe("HVL AB 123");
        saved.Holder.ShouldBe("Mustermann");
        saved.HolderFirstName.ShouldBe("Max");
        saved.VIN.ShouldBe(VehicleTestData.Vin);
        saved.TSN.ShouldBe("AJO000180");
        saved.CreatedBy.ShouldBe(VehicleTestData.CreatedBy);
        saved.CreatedAt.ShouldBe(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        saved.ModifiedAt.ShouldBe(saved.CreatedAt);
        
        await database.Context.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_WhenSavingFails_PropagatesException()
    {
        using TestDatabase database = new();

        IApplicationDbContext context = Substitute.For<IApplicationDbContext>();
        context.Vehicles.Returns(database.Db.Vehicles);
        context.SaveChangesAsync(Arg.Any<CancellationToken>())
               .ThrowsAsync(new DbUpdateException("Speichern fehlgeschlagen."));

        CreateVehicleCommandHandler handler = new(context, CreateCurrentUser());

        await Should.ThrowAsync<DbUpdateException>(
            () => handler.Handle(VehicleTestData.CreateCommand(), CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_WithoutLoggedInUser_ThrowsAndSavesNothing()
    {
        using TestDatabase database = new();
        // Ein Mock ohne Vorgabe liefert für UserId null, also ist niemand angemeldet.
        ICurrentUser nobody = Substitute.For<ICurrentUser>();
        CreateVehicleCommandHandler handler = new(database.Context, nobody);

        await Should.ThrowAsync<InvalidOperationException>(
            () => handler.Handle(VehicleTestData.CreateCommand(), CancellationToken.None));

        (await database.Db.Vehicles.CountAsync()).ShouldBe(0);
    }

    private static ICurrentUser CreateCurrentUser()
    {
        ICurrentUser currentUser = Substitute.For<ICurrentUser>();
        currentUser.UserId.Returns(VehicleTestData.CreatedBy);

        return currentUser;
    }
}
