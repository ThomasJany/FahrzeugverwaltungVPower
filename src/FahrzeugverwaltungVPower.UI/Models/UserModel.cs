namespace FahrzeugverwaltungVPower.UI.Models;

/// <summary>
/// Ein Benutzer, wie ihn die API liefert.
/// </summary>
/// <param name="Id">Eindeutige Id des Benutzers.</param>
/// <param name="Email">E-Mail-Adresse, zugleich der Anmeldename.</param>
/// <param name="FirstName">Vorname.</param>
/// <param name="LastName">Nachname.</param>
/// <param name="Roles">Rollen des Benutzers.</param>
public record UserModel(Guid Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles)
{
    /// <summary>Vor- und Nachname für die Anzeige.</summary>
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    /// <summary>Die Rollen als ein Text, zum Sortieren und Filtern in der Tabelle.</summary>
    public string RolesText => string.Join(", ", Roles);
}
