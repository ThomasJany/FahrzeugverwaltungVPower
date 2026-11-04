namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Eine gespeicherte Anmeldung: das Zugriffstoken, sein Ablauf und der Anzeigename des Benutzers.
/// </summary>
/// <param name="Token">Das Zugriffstoken (JWT) der API.</param>
/// <param name="ExpiresAt">Zeitpunkt, an dem das Token abläuft.</param>
/// <param name="DisplayName">Vor- und Nachname des angemeldeten Benutzers.</param>
public record StoredSession(string Token, DateTimeOffset ExpiresAt, string DisplayName);

/// <summary>
/// Speichert die Anmeldung dauerhaft genug, um ein Neuladen der Seite zu überstehen.
/// </summary>
/// <remarks>
/// Jeder Host liefert seine eigene Umsetzung, weil der sichere Speicherort vom Host abhängt:
/// Blazor Server nutzt den verschlüsselten Session-Speicher des Browsers, die MAUI-App würde den
/// SecureStorage des Geräts nehmen. Die Seiten und Dienste der Oberfläche kennen nur diese Schnittstelle.
/// </remarks>
public interface ITokenStore
{
    /// <summary>
    /// Liest die gespeicherte Anmeldung, oder <see langword="null"/>, wenn es keine (lesbare) gibt.
    /// </summary>
    ValueTask<StoredSession?> LoadAsync();

    /// <summary>
    /// Speichert die Anmeldung.
    /// </summary>
    /// <param name="session">Die zu speichernde Anmeldung.</param>
    ValueTask SaveAsync(StoredSession session);

    /// <summary>
    /// Löscht die gespeicherte Anmeldung.
    /// </summary>
    ValueTask ClearAsync();
}
