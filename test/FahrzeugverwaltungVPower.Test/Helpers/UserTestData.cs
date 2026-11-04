using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Security;

namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Gültige Beispieldaten für die Benutzer-Tests. Die Werte sind erfunden.
/// </summary>
internal static class UserTestData
{
    /// <summary>
    /// Neue E-Mail-Adresse, die kein Testbenutzer trägt.
    /// </summary>
    public const string NewEmail = "maximilian@mustermann.de";

    /// <summary>
    /// Neues Passwort, das die Passwortregeln erfüllt.
    /// </summary>
    public const string NewPassword = "NeuesPasswort1234!";

    /// <summary>
    /// Ein Änderungsbefehl, der alle Validierungsregeln erfüllt. Alle Felder weichen vom Benutzer aus
    /// <see cref="TestIdentity.CreateUserAsync"/> ab, das Passwort bleibt unverändert.
    /// </summary>
    /// <param name="id">Id des zu ändernden Benutzers.</param>
    public static UpdateUserCommand UpdateCommand(Guid id) => new()
    {
        Id = id,
        Email = NewEmail,
        FirstName = "Maximilian",
        LastName = "Musterfrau",
        Roles = [RoleNames.Editor]
    };

    /// <summary>
    /// Ein Befehl zum Anlegen, der alle Validierungsregeln erfüllt.
    /// </summary>
    public static CreateUserCommand CreateCommand() => new()
    {
        Email = NewEmail,
        FirstName = "Maximilian",
        LastName = "Musterfrau",
        Password = NewPassword,
        Roles = [RoleNames.Editor]
    };
}
