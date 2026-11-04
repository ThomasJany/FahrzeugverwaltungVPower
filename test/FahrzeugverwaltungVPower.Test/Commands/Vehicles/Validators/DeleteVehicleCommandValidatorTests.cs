using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Validators;

[TestClass]
public class DeleteVehicleCommandValidatorTests
{
    [TestMethod]
    public async Task Validate_WithExistingId_IsValid()
    {
        using TestDatabase database = new();
        Vehicle vehicle = VehicleTestData.CreateVehicle(VehicleTestData.Vin);
        await database.AddAsync(vehicle);
        DeleteVehicleCommandValidator validator = new(database.Context);

        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(vehicle.Id));

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task Validate_IdEmpty_IsInvalid()
    {
        using TestDatabase database = new();
        DeleteVehicleCommandValidator validator = new(database.Context);

        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(Guid.Empty));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteVehicleCommand.Id) && e.ErrorCode == ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_VehicleDoesNotExist_IsInvalid()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        DeleteVehicleCommandValidator validator = new(database.Context);

        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(Guid.NewGuid()));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteVehicleCommand.Id) && e.ErrorCode == ValidationErrorCodes.NotFound);
    }
}