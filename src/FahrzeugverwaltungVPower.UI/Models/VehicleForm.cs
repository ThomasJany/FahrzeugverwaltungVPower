namespace FahrzeugverwaltungVPower.UI.Models;

/// <summary>
/// Die Eingaben im Formular zum Anlegen und Bearbeiten eines Fahrzeugs.
/// </summary>
public class VehicleForm
{
    /// <summary>Kennzeichen (Feld A).</summary>
    public string LicensePlate { get; set; } = string.Empty;

    /// <summary>Erstzulassung (Feld B).</summary>
    public DateTime? FirstRegistrationDate { get; set; }

    /// <summary>Name oder Firma des Halters (Feld C.1.1).</summary>
    public string Holder { get; set; } = string.Empty;

    /// <summary>Vorname des Halters (Feld C.1.2), bei Firmen leer.</summary>
    public string HolderFirstName { get; set; } = string.Empty;

    /// <summary>Hersteller (Feld D.1).</summary>
    public string Manufacturer { get; set; } = string.Empty;

    /// <summary>Typ (Feld D.2).</summary>
    public string TypeDesignation { get; set; } = string.Empty;

    /// <summary>Modell (Feld D.3).</summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>Fahrzeug-Identifizierungsnummer (Feld E).</summary>
    public string Vin { get; set; } = string.Empty;

    /// <summary>HU gültig bis.</summary>
    public DateTime? InspectionValidUntil { get; set; }

    /// <summary>Herstellerschlüsselnummer (Feld 2.1).</summary>
    public string Hsn { get; set; } = string.Empty;

    /// <summary>Typschlüsselnummer (Feld 2.2).</summary>
    public string Tsn { get; set; } = string.Empty;

    /// <summary>
    /// Übernimmt die Daten eines vorhandenen Fahrzeugs.
    /// </summary>
    /// <param name="vehicle">Das Fahrzeug.</param>
    public static VehicleForm From(VehicleModel vehicle) => new()
    {
        LicensePlate = vehicle.LicensePlate,
        FirstRegistrationDate = vehicle.FirstRegistrationDate.ToDateTime(TimeOnly.MinValue),
        Holder = vehicle.Holder,
        HolderFirstName = vehicle.HolderFirstName ?? string.Empty,
        Manufacturer = vehicle.Manufacturer,
        TypeDesignation = vehicle.TypeDesignation,
        Model = vehicle.Model,
        Vin = vehicle.VIN,
        InspectionValidUntil = vehicle.InspectionValidUntil.ToDateTime(TimeOnly.MinValue),
        Hsn = vehicle.HSN,
        Tsn = vehicle.TSN
    };
}
