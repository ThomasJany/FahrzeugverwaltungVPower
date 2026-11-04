using FahrzeugverwaltungVPower.Core.Entities;

namespace FahrzeugverwaltungVPower.Core.Queries.Users;

/// <summary>
/// Öffentliche Darstellung eines Benutzers, ohne Passwort-Hash und andere interne Daten.
/// </summary>
/// <param name="Id">Eindeutige Id des Benutzers.</param>
/// <param name="Email">E-Mail-Adresse.</param>
/// <param name="FirstName">Vorname.</param>
/// <param name="LastName">Nachname.</param>
/// <param name="Roles">Rollen des Benutzers, alphabetisch sortiert.</param>
public record UserDto(Guid Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles)
{
    /// <summary>
    /// Erzeugt das DTO aus einem Benutzer und seinen Rollen.
    /// </summary>
    /// <param name="user">Der Benutzer.</param>
    /// <param name="roles">Die Rollen des Benutzers.</param>
    public static UserDto From(ApplicationUser user, IEnumerable<string> roles) =>
        new(user.Id, user.Email ?? string.Empty, user.FirstName, user.LastName, roles.Order().ToList());
}