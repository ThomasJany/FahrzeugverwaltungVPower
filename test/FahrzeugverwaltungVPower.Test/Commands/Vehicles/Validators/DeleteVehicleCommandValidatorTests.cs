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
    public async Task Validate_WithExistingVin_IsValid()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        DeleteVehicleCommandValidator validator = new(database.Context);

        // Kleinschreibung und Leerzeichen müssen normalisiert werden.
        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(" tmbzzz1kzaw123456 "));

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task Validate_VinEmpty_IsInvalid()
    {
        using TestDatabase database = new();
        DeleteVehicleCommandValidator validator = new(database.Context);

        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(""));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteVehicleCommand.Vin) && e.ErrorCode == ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_VehicleDoesNotExist_IsInvalid()
    {
        using TestDatabase database = new();
        await database.AddAsync(VehicleTestData.CreateVehicle(VehicleTestData.Vin));
        DeleteVehicleCommandValidator validator = new(database.Context);

        ValidationResult result = await validator.ValidateAsync(new DeleteVehicleCommand(VehicleTestData.OtherVin));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteVehicleCommand.Vin) && e.ErrorCode == ValidationErrorCodes.NotFound);
    }
}