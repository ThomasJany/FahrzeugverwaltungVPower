using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Validators;

// Tests für die gemeinsamen Regeln aus VehicleCommandValidatorBase werden in CreateVehicleCommandValidatorTests durchgeführt.

[TestClass]
public class UpdateVehicleCommandValidatorTests
{
    [TestMethod]
    public async Task Validate_WithValidCommand_IsValid()
    {
        Vehicle existing = VehicleTestData.CreateVehicle(VehicleTestData.OtherVin);
        UpdateVehicleCommand command = VehicleTestData.UpdateCommand(existing.Id) with { VIN = VehicleTestData.Vin };

        ValidationResult result = await ValidateAsync(command, existing);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task Validate_VinUnchangedForSameVehicle_IsValid()
    {
        // Das Fahrzeug behält seine eigene VIN, das darf kein Duplikat sein.
        Vehicle existing = VehicleTestData.CreateVehicle(VehicleTestData.Vin);
        UpdateVehicleCommand command = VehicleTestData.UpdateCommand(existing.Id);

        ValidationResult result = await ValidateAsync(command, existing);

        result.IsValid.ShouldBeTrue();
    }

    [TestMethod]
    public async Task Validate_VinBelongsToOtherVehicle_IsInvalid()
    {
        Vehicle existing = VehicleTestData.CreateVehicle(VehicleTestData.Vin);
        Vehicle other = VehicleTestData.CreateVehicle(VehicleTestData.OtherVin, "OHZ CD 45");
        // Das erste Fahrzeug soll die VIN des zweiten bekommen.
        UpdateVehicleCommand command = VehicleTestData.UpdateCommand(existing.Id) with { VIN = VehicleTestData.OtherVin };

        ValidationResult result = await ValidateAsync(command, existing, other);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateVehicleCommand.VIN) && e.ErrorCode == ValidationErrorCodes.AsyncPredicate);
    }

    private static async Task<ValidationResult> ValidateAsync(UpdateVehicleCommand command, params Vehicle[] existingVehicles)
    {
        using TestDatabase database = new();
        await database.AddAsync(existingVehicles);
        UpdateVehicleCommandValidator validator = new(database.Context);

        return await validator.ValidateAsync(command);
    }
}