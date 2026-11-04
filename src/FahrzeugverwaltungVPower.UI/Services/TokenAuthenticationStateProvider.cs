using Microsoft.AspNetCore.Components.Authorization;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Stellt Blazor den angemeldeten Benutzer aus der Anmeldesitzung zur Verfügung.
/// </summary>
public class TokenAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly AuthSession _session;

    /// <summary>
    /// Meldet sich bei Änderungen der Anmeldung an, damit sich geschützte Bereiche sofort aktualisieren.
    /// </summary>
    /// <param name="session">Die Anmeldesitzung.</param>
    public TokenAuthenticationStateProvider(AuthSession session)
    {
        _session = session;
        _session.Changed += OnSessionChanged;
    }

    /// <summary>
    /// Liefert den aktuellen Benutzer, ohne Anmeldung einen anonymen.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        StoredSession? current = await _session.GetAsync();

        return new AuthenticationState(current is null ? JwtClaims.Anonymous() : JwtClaims.CreatePrincipal(current));
    }

    /// <summary>
    /// Gibt die Anmeldung an der Sitzung wieder frei.
    /// </summary>
    public void Dispose() => _session.Changed -= OnSessionChanged;

    private void OnSessionChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
