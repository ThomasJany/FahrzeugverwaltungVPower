namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

/// <summary>
/// Gemeinsame Daten der Befehle zum Anlegen und Ändern eines Fahrzeugs.
/// </summary>
public abstract record VehicleCommandBase
{
    /// <summary>
    /// Amtliches Kennzeichen (Feld A).
    /// </summary>
    public required string LicensePlate { get; init; }

    /// <summary>
    /// Datum der Erstzulassung (Feld B).
    /// </summary>
    public required DateOnly FirstRegistrationDate { get; init; }

    /// <summary>
    /// Familien- oder Firmenname des Halters (Feld C.1.1).
    /// </summary>
    public required string Holder { get; init; }

    /// <summary>
    /// Vorname des Halters (Feld C.1.2), bei Firmen leer.
    /// </summary>
    public string? HolderFirstName { get; init; }

    /// <summary>
    /// Hersteller bzw. Marke (Feld D.1).
    /// </summary>
    public required string Manufacturer { get; init; }

    /// <summary>
    /// Fahrzeugtyp (Feld D.2).
    /// </summary>
    public required string TypeDesignation { get; init; }

    /// <summary>
    /// Handelsbezeichnung (Feld D.3).
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    /// Fahrzeug-Identifizierungsnummer (Feld E).
    /// </summary>
    public required string VIN { get; init; }

    /// <summary>
    /// Datum, bis zu dem die Hauptuntersuchung gültig ist.
    /// </summary>
    public required DateOnly InspectionValidUntil { get; init; }

    /// <summary>
    /// Herstellerschlüsselnummer (Feld 2.1).
    /// </summary>
    public required string HSN { get; init; }

    /// <summary>
    /// Typschlüsselnummer mit Variante und Prüfziffer (Feld 2.2).
    /// </summary>
    public required string TSN { get; init; }
}