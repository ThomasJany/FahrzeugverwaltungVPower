using FahrzeugverwaltungVPower.UI.Models;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Anmelden und Abmelden.
/// </summary>
/// <param name="api">Der Client für die Aufrufe an die API.</param>
/// <param name="session">Die Anmeldesitzung des Benutzers.</param>
public class AuthService(ApiClient api, AuthSession session)
{
    /// <summary>
    /// Meldet den Benutzer an der API an und merkt sich bei Erfolg die Anmeldung.
    /// </summary>
    /// <param name="email">E-Mail-Adresse des Benutzers.</param>
    /// <param name="password">Passwort des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<ApiResult> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        ApiResult<LoginResponse> result = await api.SendAsync<LoginResponse>(
            HttpMethod.Post, "api/auth/Login", new { email, password }, authenticated: false, cancellationToken);

        if (!result.Success)
        {
            return result;
        }

        if (result.Value is null)
        {
            return ApiResult.Failure(result.StatusCode, [new ApiError("", "Die API hat keine Anmeldedaten geliefert.")]);
        }

        await session.SignInAsync(new StoredSession(result.Value.Token, result.Value.ExpiresAt, $"{result.Value.FirstName} {result.Value.LastName}"));

        return result;
    }

    /// <summary>
    /// Meldet den Benutzer ab und vergisst die Anmeldung.
    /// </summary>
    public Task LogoutAsync() => session.SignOutAsync();
}
