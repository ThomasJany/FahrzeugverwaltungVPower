namespace FahrzeugverwaltungVPower.UI.Models;

/// <summary>
/// Zustand der Hauptuntersuchung (HU) eines Fahrzeugs.
/// </summary>
public enum InspectionStatus
{
    /// <summary>Die HU ist noch länger als drei Monate gültig.</summary>
    Valid,

    /// <summary>Die HU ist innerhalb der nächsten drei Monate fällig.</summary>
    DueSoon,

    /// <summary>Die HU ist abgelaufen.</summary>
    Expired
}

/// <summary>
/// Ein Fahrzeug, wie es die API liefert.
/// </summary>
/// <param name="Id">Eindeutige Id des Fahrzeugs.</param>
/// <param name="LicensePlate">Kennzeichen (Feld A).</param>
/// <param name="FirstRegistrationDate">Erstzulassung (Feld B).</param>
/// <param name="Holder">Name oder Firma des Halters (Feld C.1.1).</param>
/// <param name="HolderFirstName">Vorname des Halters (Feld C.1.2), bei Firmen leer.</param>
/// <param name="Manufacturer">Hersteller (Feld D.1).</param>
/// <param name="TypeDesignation">Typ (Feld D.2).</param>
/// <param name="Model">Modell (Feld D.3).</param>
/// <param name="VIN">Fahrzeug-Identifizierungsnummer (Feld E).</param>
/// <param name="InspectionValidUntil">HU gültig bis.</param>
/// <param name="HSN">Herstellerschlüsselnummer (Feld 2.1).</param>
/// <param name="TSN">Typschlüsselnummer (Feld 2.2).</param>
public record VehicleModel(
    Guid Id,
    string LicensePlate,
    DateOnly FirstRegistrationDate,
    string Holder,
    string? HolderFirstName,
    string Manufacturer,
    string TypeDesignation,
    string Model,
    string VIN,
    DateOnly InspectionValidUntil,
    string HSN,
    string TSN)
{
    /// <summary>Vorname und Name bzw. Firma des Halters für die Anzeige.</summary>
    public string HolderDisplayName => $"{HolderFirstName} {Holder}".Trim();

    /// <summary>Hersteller und Modell für die Anzeige, zum Beispiel "Volkswagen Golf".</summary>
    public string VehicleText => $"{Manufacturer} {Model}".Trim();

    /// <summary>
    /// Bestimmt den Zustand der HU: abgelaufen, innerhalb von drei Monaten fällig oder gültig.
    /// </summary>
    /// <param name="today">Das heutige Datum.</param>
    public InspectionStatus InspectionStatus(DateOnly today)
    {
        if (InspectionValidUntil < today)
        {
            return Models.InspectionStatus.Expired;
        }

        return InspectionValidUntil <= today.AddMonths(3) ? Models.InspectionStatus.DueSoon : Models.InspectionStatus.Valid;
    }
}
