using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Entities;

namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Gültige Beispieldaten für die Tests.
/// </summary>
internal static class VehicleTestData
{
    /// <summary>
    /// Eine formal gültige VIN (17 Zeichen, ohne I, O, Q).
    /// </summary>
    public const string Vin = "TMBZZZ1KZAW123456";

    /// <summary>
    /// Eine zweite, davon verschiedene gültige VIN.
    /// </summary>
    public const string OtherVin = "WVWZZZ1KZAW654321";

    /// <summary>
    /// Ein Command, der alle Validierungsregeln erfüllt.
    /// </summary>
    public static CreateVehicleCommand CreateCommand() => new()
    {
        LicensePlate = "HVL AB 123",
        FirstRegistrationDate = new DateOnly(2020, 5, 14),
        Holder = "Mustermann",
        HolderFirstName = "Max",
        Manufacturer = "Skoda",
        TypeDesignation = "NX",
        Model = "Octavia",
        VIN = Vin,
        InspectionValidUntil = new DateOnly(2027, 5, 31),
        HSN = "8004",
        TSN = "AJO000180"
    };

    /// <summary>
    /// Ein bereits gespeichertes Fahrzeug, wie es der Create-Handler erzeugen würde.
    /// </summary>
    public static Vehicle CreateVehicle(string vin = Vin, string licensePlate = "HVL AB 123") => new()
    {
        LicensePlate = licensePlate,
        FirstRegistrationDate = new DateOnly(2020, 5, 14),
        Holder = "Mustermann",
        HolderFirstName = "Max",
        Manufacturer = "Skoda",
        TypeDesignation = "NX",
        Model = "Octavia",
        VIN = vin,
        InspectionValidUntil = new DateOnly(2027, 5, 31),
        HSN = "8004",
        TSN = "AJO000180",
        CreatedAt = DateTimeOffset.UtcNow,
        ModifiedAt = DateTimeOffset.UtcNow
    };
}
