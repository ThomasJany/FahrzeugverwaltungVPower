using System.Security.Claims;
using FahrzeugverwaltungVPower.Core.Abstractions;

namespace FahrzeugverwaltungVPower.API.Security;

/// <summary>
/// Liest den angemeldeten Benutzer aus den Claims der aktuellen Anfrage.
/// </summary>
/// <param name="httpContextAccessor">Liefert die aktuelle Anfrage.</param>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    /// <summary>
    /// Id des angemeldeten Benutzers, oder <see langword="null"/>, wenn niemand angemeldet ist.
    /// </summary>
    public Guid? UserId =>
        Guid.TryParse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id) ? id : null;
}