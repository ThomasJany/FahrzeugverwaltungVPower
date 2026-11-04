namespace FahrzeugverwaltungVPower.Core.Security;

/// <summary>
/// Die Namen der Berechtigungsrichtlinien der Anwendung.
/// </summary>
public static class PolicyNames
{
    /// <summary>
    /// Fahrzeuge ansehen.
    /// </summary>
    public const string CanView = "CanView";

    /// <summary>
    /// Fahrzeuge bearbeiten.
    /// </summary>
    public const string CanEdit = "CanEdit";

    /// <summary>
    /// Fahrzeuge anlegen.
    /// </summary>
    public const string CanCreate = "CanCreate";

    /// <summary>
    /// Fahrzeuge löschen.
    /// </summary>
    public const string CanDelete = "CanDelete";

    /// <summary>
    /// Benutzer verwalten.
    /// </summary>
    public const string CanManageUsers = "CanManageUsers";
}