using System.Security.Claims;
using System.Text.Json;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Liest Benutzer und Rollen aus einem JWT, damit die Oberfläche Bereiche ein- und ausblenden kann.
/// </summary>
/// <remarks>
/// Die Signatur wird hier bewusst NICHT geprüft: Das Token ist nur Anzeigehilfe. Jeder Aufruf der API
/// wird dort vollständig geprüft, ein gefälschtes Token würde dort mit 401 abgelehnt.
/// </remarks>
public static class JwtClaims
{
    /// <summary>
    /// Erzeugt den Benutzer mit allen Claims aus dem Token der Anmeldung.
    /// </summary>
    /// <param name="session">Die Anmeldung mit Token und Anzeigename.</param>
    public static ClaimsPrincipal CreatePrincipal(StoredSession session)
    {
        List<Claim> claims = ReadClaims(session.Token);

        if (claims.Count == 0)
        {
            return Anonymous();
        }

        claims.RemoveAll(c => c.Type == ClaimTypes.Name);
        claims.Add(new Claim(ClaimTypes.Name, session.DisplayName));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role));
    }

    /// <summary>
    /// Ein Benutzer ohne Anmeldung.
    /// </summary>
    public static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());

    /// <summary>
    /// Liest die Claims aus dem mittleren Teil des Tokens, bei einem unlesbaren Token eine leere Liste.
    /// </summary>
    /// <param name="token">Das Zugriffstoken (JWT).</param>
    private static List<Claim> ReadClaims(string token)
    {
        List<Claim> claims = [];

        try
        {
            string[] parts = token.Split('.');

            if (parts.Length != 3)
            {
                return claims;
            }

            using JsonDocument payload = JsonDocument.Parse(DecodeBase64Url(parts[1]));

            foreach (JsonProperty property in payload.RootElement.EnumerateObject())
            {
                string type = NormalizeClaimType(property.Name);

                if (property.Value.ValueKind == JsonValueKind.Array)
                {
                    claims.AddRange(property.Value.EnumerateArray().Select(item => new Claim(type, item.ToString())));
                }
                else if (property.Value.ValueKind is JsonValueKind.String or JsonValueKind.Number)
                {
                    claims.Add(new Claim(type, property.Value.ToString()));
                }
            }
        }
        catch (Exception ex) when (ex is FormatException or JsonException or InvalidOperationException)
        {
            claims.Clear();
        }

        return claims;
    }

    /// <summary>
    /// Bildet die kurzen Rollennamen "role" und "roles" auf den üblichen Claim-Typ ab.
    /// </summary>
    /// <param name="name">Der Name des Claims im Token.</param>
    private static string NormalizeClaimType(string name) => name is "role" or "roles" ? ClaimTypes.Role : name;

    /// <summary>
    /// Dekodiert Base64 in der URL-sicheren Schreibweise, wie sie JWT benutzen.
    /// </summary>
    /// <param name="value">Der kodierte Text.</param>
    private static byte[] DecodeBase64Url(string value)
    {
        string base64 = value.Replace('-', '+').Replace('_', '/');
        base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');

        return Convert.FromBase64String(base64);
    }
}
