namespace FahrzeugverwaltungVPower.UI.Models;

/// <summary>
/// Die Rollennamen der Anwendung.
/// </summary>
/// <remarks>
/// Die Rollen müssen zu den Policies der API passen (Datei SecurityServiceCollectionExtensions). Die API prüft
/// jeden Aufruf selbst, die Oberfläche blendet nur aus, was der Benutzer ohnehin nicht darf.
/// </remarks>
public static class UiRoles
{
    /// <summary>Administrator: Benutzerverwaltung und alle Fahrzeugaktionen.</summary>
    public const string Admin = "Admin";

    /// <summary>Creator: Fahrzeuge anlegen, bearbeiten und löschen.</summary>
    public const string Creator = "Creator";

    /// <summary>Editor: Fahrzeuge bearbeiten.</summary>
    public const string Editor = "Editor";

    /// <summary>Viewer: Fahrzeuge ansehen.</summary>
    public const string Viewer = "Viewer";

    /// <summary>Alle Rollen, in der Reihenfolge der Anzeige.</summary>
    public static readonly IReadOnlyList<string> All = [Admin, Creator, Editor, Viewer];

    /// <summary>Rollen, die Benutzer verwalten dürfen.</summary>
    public const string CanManageUsers = Admin;

    /// <summary>
    /// Kurze Erklärung zu einer Rolle für die Anzeige.
    /// </summary>
    /// <param name="role">Name der Rolle.</param>
    public static string Description(string role) => role switch
    {
        Admin => "Benutzerverwaltung und alle Fahrzeugaktionen",
        Creator => "Fahrzeuge ansehen, bearbeiten, anlegen und löschen",
        Editor => "Fahrzeuge ansehen und bearbeiten",
        _ => "Fahrzeuge ansehen"
    };
}
