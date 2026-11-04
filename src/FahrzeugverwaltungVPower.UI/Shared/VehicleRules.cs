using System.Text.RegularExpressions;

namespace FahrzeugverwaltungVPower.UI.Shared;

/// <summary>
/// Die Prüfregeln für die Felder des Fahrzeugformulars. Jede Regel liefert eine Fehlermeldung oder
/// <see langword="null"/>, wenn die Eingabe in Ordnung ist.
/// </summary>
/// <remarks>
/// Die Regeln sind eine Kopie der Validatoren in Core (VehicleCommandValidatorBase), damit Fehler schon bei der
/// Eingabe auffallen. Ändert sich eine Regel dort, muss sie hier nachgezogen werden (siehe README, Ausblick).
/// Die API prüft trotzdem jeden Aufruf selbst, auch auf eine doppelte VIN.
/// </remarks>
public static partial class VehicleRules
{
    /// <summary>
    /// Prüft das Kennzeichen (Feld A): 2 bis 11 Zeichen, Buchstaben, Ziffern, Leerzeichen, Bindestrich.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? LicensePlate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Das Kennzeichen ist erforderlich.";
        }

        return PlatePattern().IsMatch(value.Trim())
            ? null
            : "Das Kennzeichen darf 2 bis 11 Zeichen lang sein (Buchstaben, Ziffern, Leerzeichen, Bindestrich).";
    }

    /// <summary>
    /// Prüft die Erstzulassung (Feld B): erforderlich und nicht in der Zukunft.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    /// <param name="today">Das heutige Datum.</param>
    public static string? FirstRegistrationDate(DateTime? value, DateOnly today)
    {
        if (value is null)
        {
            return "Die Erstzulassung ist erforderlich.";
        }

        return DateOnly.FromDateTime(value.Value) > today ? "Die Erstzulassung darf nicht in der Zukunft liegen." : null;
    }

    /// <summary>
    /// Prüft das Datum der HU: erforderlich.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? InspectionValidUntil(DateTime? value) =>
        value is null ? "Das Datum der HU ist erforderlich." : null;

    /// <summary>
    /// Prüft ein Pflichtfeld mit Höchstlänge.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    /// <param name="label">Bezeichnung des Feldes mit Artikel für die Meldung, zum Beispiel "Der Hersteller".</param>
    /// <param name="maxLength">Höchstlänge.</param>
    public static string? Required(string? value, string label, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"{label} ist erforderlich.";
        }

        return value.Trim().Length > maxLength ? $"{label} darf höchstens {maxLength} Zeichen lang sein." : null;
    }

    /// <summary>
    /// Prüft ein optionales Feld mit Höchstlänge, zum Beispiel den Vornamen des Halters.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    /// <param name="label">Bezeichnung des Feldes mit Artikel für die Meldung.</param>
    /// <param name="maxLength">Höchstlänge.</param>
    public static string? Optional(string? value, string label, int maxLength) =>
        value is not null && value.Trim().Length > maxLength ? $"{label} darf höchstens {maxLength} Zeichen lang sein." : null;

    /// <summary>
    /// Prüft die VIN (Feld E): 17 Zeichen, Buchstaben ohne I, O, Q und Ziffern.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? Vin(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Die VIN ist erforderlich.";
        }

        return VinPattern().IsMatch(value.Trim())
            ? null
            : "Die VIN muss 17 Zeichen lang sein (Buchstaben ohne I, O, Q, und Ziffern).";
    }

    /// <summary>
    /// Prüft die HSN (Feld 2.1): genau 4 Ziffern.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? Hsn(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Die HSN ist erforderlich.";
        }

        return HsnPattern().IsMatch(value.Trim()) ? null : "Die HSN besteht aus genau 4 Ziffern.";
    }

    /// <summary>
    /// Prüft die TSN (Feld 2.2): bis zu 9 Zeichen, Buchstaben, Ziffern, Strich.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? Tsn(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Die TSN ist erforderlich.";
        }

        return TsnPattern().IsMatch(value.Trim()) ? null : "Die TSN darf bis zu 9 Zeichen lang sein (Buchstaben, Ziffern, Strich).";
    }

    [GeneratedRegex(@"^[A-Za-zÄÖÜäöü0-9 \-]{2,11}$")]
    private static partial Regex PlatePattern();

    [GeneratedRegex("^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$")]
    private static partial Regex VinPattern();

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex HsnPattern();

    [GeneratedRegex(@"^[A-Za-z0-9\-]{1,9}$")]
    private static partial Regex TsnPattern();
}
