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

    /// <summary>Rollen, die Fahrzeuge ansehen dürfen.</summary>
    public const string CanView = Admin + "," + Creator + "," + Editor + "," + Viewer;

    /// <summary>Rollen, die Fahrzeuge bearbeiten dürfen.</summary>
    public const string CanEdit = Admin + "," + Creator + "," + Editor;

    /// <summary>Rollen, die Fahrzeuge anlegen dürfen.</summary>
    public const string CanCreate = Admin + "," + Creator;

    /// <summary>Rollen, die Fahrzeuge löschen dürfen.</summary>
    public const string CanDelete = Admin + "," + Creator;

    /// <summary>Rollen, die Benutzer verwalten dürfen.</summary>
    public const string CanManageUsers = Admin;

    /// <summary>
    /// Gibt an, ob der Benutzer mindestens eine der kommagetrennten Rollen hat.
    /// </summary>
    /// <param name="user">Der angemeldete Benutzer.</param>
    /// <param name="roles">Kommagetrennte Rollenliste, zum Beispiel <see cref="CanEdit"/>.</param>
    public static bool IsInAny(System.Security.Claims.ClaimsPrincipal user, string roles) =>
        roles.Split(',').Any(user.IsInRole);

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
