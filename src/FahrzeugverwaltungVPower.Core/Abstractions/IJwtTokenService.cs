using FahrzeugverwaltungVPower.Core.Entities;

namespace FahrzeugverwaltungVPower.Core.Abstractions;

/// <summary>
/// Erzeugt Zugriffstokens für angemeldete Benutzer.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Erzeugt ein signiertes Zugriffstoken mit Benutzer-Id, E-Mail und Rollen.
    /// </summary>
    /// <param name="user">Der angemeldete Benutzer.</param>
    /// <param name="roles">Die Rollen des Benutzers.</param>
    AccessToken CreateToken(ApplicationUser user, IEnumerable<string> roles);
}

/// <summary>
/// Ein ausgestelltes Zugriffstoken mit seinem Ablaufzeitpunkt.
/// </summary>
/// <param name="Token">Der signierte Tokentext.</param>
/// <param name="ExpiresAt">Zeitpunkt, an dem das Token abläuft.</param>
public record AccessToken(string Token, DateTimeOffset ExpiresAt);