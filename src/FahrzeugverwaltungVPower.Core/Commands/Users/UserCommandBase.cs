namespace FahrzeugverwaltungVPower.Core.Commands.Users;

/// <summary>
/// Gemeinsame Daten der Befehle zum Anlegen und Ändern eines Benutzers.
/// </summary>
public abstract record UserCommandBase
{
    /// <summary>E-Mail-Adresse, zugleich Login-Name.</summary>
    public required string Email { get; init; }

    /// <summary>Vorname.</summary>
    public required string FirstName { get; init; }

    /// <summary>Nachname.</summary>
    public required string LastName { get; init; }

    /// <summary>Rollen des Benutzers, mindestens eine, nur bekannte Rollennamen.</summary>
    public required IReadOnlyList<string> Roles { get; init; }
}