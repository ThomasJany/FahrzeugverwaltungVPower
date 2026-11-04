using System.ComponentModel.DataAnnotations;

namespace FahrzeugverwaltungVPower.Core.Security;

/// <summary>
/// Einstellungen für die Ausstellung und Prüfung der JWT-Zugriffstokens.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Name des Konfigurationsabschnitts.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Aussteller des Tokens.
    /// </summary>
    [Required]
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// Empfänger des Tokens.
    /// </summary>
    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// Geheimer Schlüssel zum Signieren, mindestens 32 Zeichen.
    /// </summary>
    [Required, MinLength(32)]
    public string Key { get; init; } = string.Empty;

    /// <summary>
    /// Gültigkeit des Tokens in Minuten.
    /// </summary>
    [Range(1, 1440)]
    public int ExpiresMinutes { get; init; } = 60;
}