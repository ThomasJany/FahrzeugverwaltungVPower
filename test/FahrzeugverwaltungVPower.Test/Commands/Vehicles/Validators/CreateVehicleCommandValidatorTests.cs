using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Test.Helpers;
using FluentValidation.Results;
using Shouldly;

namespace FahrzeugverwaltungVPower.Test.Commands.Vehicles.Validators;

[TestClass]
public class CreateVehicleCommandValidatorTests
{
    // ---------- Positivfall ----------

    [TestMethod]
    public async Task Validate_WithValidCommand_IsValid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand());

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    // ---------- Kennzeichen (Feld A) ----------

    [TestMethod]
    public async Task Validate_LicensePlateEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { LicensePlate = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.LicensePlate), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_LicensePlateWithInvalidCharacter_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { LicensePlate = "AB#123" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.LicensePlate), ValidationErrorCodes.RegularExpression);
    }

    // ---------- Erstzulassung (Feld B) ----------

    [TestMethod]
    public async Task Validate_FirstRegistrationDateInFuture_IsInvalid()
    {
        DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { FirstRegistrationDate = tomorrow });

        ShouldHaveError(result, nameof(CreateVehicleCommand.FirstRegistrationDate), ValidationErrorCodes.Predicate);
    }

    // ---------- Halter (Feld C.1.1, C.1.2) ----------

    [TestMethod]
    public async Task Validate_HolderEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Holder = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Holder), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_HolderTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Holder = new string('A', 201) });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Holder), ValidationErrorCodes.MaxLength);
    }

    [TestMethod]
    public async Task Validate_HolderFirstNameTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { HolderFirstName = new string('A', 201) });

        ShouldHaveError(result, nameof(CreateVehicleCommand.HolderFirstName), ValidationErrorCodes.MaxLength);
    }

    // ---------- Hersteller (Feld D.1) ----------

    [TestMethod]
    public async Task Validate_ManufacturerEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Manufacturer = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Manufacturer), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_ManufacturerTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Manufacturer = new string('A', 26) });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Manufacturer), ValidationErrorCodes.MaxLength);
    }

    // ---------- Typ (Feld D.2) ----------

    [TestMethod]
    public async Task Validate_TypeDesignationEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { TypeDesignation = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.TypeDesignation), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_TypeDesignationTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { TypeDesignation = new string('A', 26) });

        ShouldHaveError(result, nameof(CreateVehicleCommand.TypeDesignation), ValidationErrorCodes.MaxLength);
    }

    // ---------- Handelsbezeichnung (Feld D.3) ----------

    [TestMethod]
    public async Task Validate_ModelEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Model = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Model), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_ModelTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { Model = new string('A', 26) });

        ShouldHaveError(result, nameof(CreateVehicleCommand.Model), ValidationErrorCodes.MaxLength);
    }

    // ---------- VIN (Feld E) ----------

    [TestMethod]
    public async Task Validate_VinEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { VIN = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.VIN), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_VinWithForbiddenLetter_IsInvalid()
    {
        // 17 Zeichen, aber mit dem Buchstaben "O", der in einer VIN nicht vorkommt.
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { VIN = "TMBZZZ1KZAW12345O" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.VIN), ValidationErrorCodes.RegularExpression);
    }

    [TestMethod]
    public async Task Validate_VinAlreadyExists_IsInvalid()
    {
        // Die Eingabe in Kleinschreibung muss ebenfalls als Duplikat erkannt werden.
        ValidationResult result = await ValidateAsync(
            VehicleTestData.CreateCommand() with { VIN = VehicleTestData.Vin.ToLowerInvariant() },
            VehicleTestData.CreateVehicle(VehicleTestData.Vin));

        ShouldHaveError(result, nameof(CreateVehicleCommand.VIN), ValidationErrorCodes.AsyncPredicate);
    }

    // ---------- HSN (Feld 2.1) ----------

    [TestMethod]
    public async Task Validate_HsnEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { HSN = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.HSN), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_HsnWithLetters_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { HSN = "12AB" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.HSN), ValidationErrorCodes.RegularExpression);
    }

    // ---------- TSN (Feld 2.2) ----------

    [TestMethod]
    public async Task Validate_TsnEmpty_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { TSN = "" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.TSN), ValidationErrorCodes.NotEmpty);
    }

    [TestMethod]
    public async Task Validate_TsnTooLong_IsInvalid()
    {
        ValidationResult result = await ValidateAsync(VehicleTestData.CreateCommand() with { TSN = "ABCDEFGHIJ" });

        ShouldHaveError(result, nameof(CreateVehicleCommand.TSN), ValidationErrorCodes.RegularExpression);
    }

    // ---------- Hilfsmethoden ----------

    private static async Task<ValidationResult> ValidateAsync(CreateVehicleCommand command, params Vehicle[] existingVehicles)
    {
        using TestDatabase database = new();
        await database.AddAsync(existingVehicles);
        CreateVehicleCommandValidator validator = new(database.Context);

        return await validator.ValidateAsync(command);
    }

    private static void ShouldHaveError(ValidationResult result, string propertyName, string errorCode)
    {
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == propertyName && e.ErrorCode == errorCode);
    }
}
