using System.Security.Claims;
using System.Text;
using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FahrzeugverwaltungVPower.Infrastructure.Security;

/// <summary>
/// Erzeugt signierte JWT-Zugriffstokens.
/// </summary>
/// <param name="options">Die JWT-Einstellungen.</param>
/// <param name="timeProvider">Liefert die aktuelle Zeit.</param>
public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    /// <summary>
    /// Erzeugt ein signiertes Zugriffstoken mit Benutzer-Id, E-Mail und Rollen.
    /// </summary>
    /// <param name="user">Der angemeldete Benutzer.</param>
    /// <param name="roles">Die Rollen des Benutzers.</param>
    public AccessToken CreateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        DateTimeOffset expiresAt = timeProvider.GetUtcNow().AddMinutes(_options.ExpiresMinutes);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            .. roles.Select(role => new Claim(ClaimTypes.Role, role))
        ];

        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        string token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessToken(token, expiresAt);
    }
}