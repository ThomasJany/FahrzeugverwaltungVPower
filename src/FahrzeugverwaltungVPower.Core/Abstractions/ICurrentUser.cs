namespace FahrzeugverwaltungVPower.Core.Abstractions;

/// <summary>
/// Liefert Angaben zum Benutzer, der die aktuelle Anfrage gestellt hat.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Id des angemeldeten Benutzers, oder <see langword="null"/>, wenn niemand angemeldet ist.
    /// </summary>
    Guid? UserId { get; }
}