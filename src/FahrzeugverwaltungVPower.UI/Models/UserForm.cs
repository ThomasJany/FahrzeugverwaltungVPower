namespace FahrzeugverwaltungVPower.UI.Models;

/// <summary>
/// Die Eingaben im Formular zum Anlegen und Bearbeiten eines Benutzers.
/// </summary>
public class UserForm
{
    /// <summary>Vorname.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Nachname.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>E-Mail-Adresse, zugleich der Anmeldename.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Passwort. Beim Bearbeiten bedeutet leer: unverändert.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Wiederholung des Passworts.</summary>
    public string PasswordConfirmation { get; set; } = string.Empty;

    /// <summary>Die ausgewählten Rollen.</summary>
    public HashSet<string> Roles { get; set; } = [];

    /// <summary>
    /// Übernimmt die Daten eines vorhandenen Benutzers. Das Passwort bleibt leer.
    /// </summary>
    /// <param name="user">Der Benutzer.</param>
    public static UserForm From(UserModel user) => new()
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Roles = [.. user.Roles]
    };
}
