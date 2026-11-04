namespace FahrzeugverwaltungVPower.Core.Validation;

/// <summary>
/// Eigene Fehlercodes für Validierungsfehler, die über die Standardcodes von FluentValidation hinausgehen.
/// </summary>
public static class CustomErrorCodes
{
    /// <summary>Die angefragte Ressource existiert nicht, die API antwortet darauf mit 404 statt 400.</summary>
    public const string NotFound = "NotFound";
}
