namespace FahrzeugverwaltungVPower.Core.Commands.Auth;

/// <summary>
/// Ergebnis einer erfolgreichen Anmeldung.
/// </summary>
/// <param name="Token">Das signierte Zugriffstoken.</param>
/// <param name="ExpiresAt">Zeitpunkt, an dem das Token abläuft.</param>
/// <param name="Email">E-Mail-Adresse des angemeldeten Benutzers.</param>
/// <param name="FirstName">Vorname des angemeldeten Benutzers.</param>
/// <param name="LastName">Nachname des angemeldeten Benutzers.</param>
/// <param name="Roles">Rollen des angemeldeten Benutzers.</param>
public record LoginResult(
    string Token,
    DateTimeOffset ExpiresAt,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyList<string> Roles);