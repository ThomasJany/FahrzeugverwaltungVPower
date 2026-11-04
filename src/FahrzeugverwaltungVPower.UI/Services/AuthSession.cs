namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Hält die aktuelle Anmeldung im Speicher und synchronisiert sie mit dem <see cref="ITokenStore"/>.
/// </summary>
/// <param name="store">Der Speicher für die Anmeldung des jeweiligen Hosts.</param>
/// <param name="timeProvider">Liefert die aktuelle Zeit, austauschbar für Tests.</param>
public class AuthSession(ITokenStore store, TimeProvider timeProvider)
{
    private StoredSession? _current;
    private bool _loaded;

    /// <summary>
    /// Wird ausgelöst, wenn sich die Anmeldung geändert hat (Anmelden, Abmelden, Ablauf).
    /// </summary>
    public event Action? Changed;

    /// <summary>
    /// Liefert die aktuelle Anmeldung, oder <see langword="null"/>, wenn niemand angemeldet oder das Token abgelaufen ist.
    /// </summary>
    public async ValueTask<StoredSession?> GetAsync()
    {
        if (!_loaded)
        {
            _current = await store.LoadAsync();
            _loaded = true;
        }

        if (_current is not null && _current.ExpiresAt <= timeProvider.GetUtcNow())
        {
            await SignOutAsync();
        }

        return _current;
    }

    /// <summary>
    /// Speichert eine neue Anmeldung und meldet die Änderung.
    /// </summary>
    /// <param name="session">Die neue Anmeldung.</param>
    public async Task SignInAsync(StoredSession session)
    {
        await store.SaveAsync(session);

        _current = session;
        _loaded = true;
        Changed?.Invoke();
    }

    /// <summary>
    /// Löscht die Anmeldung und meldet die Änderung.
    /// </summary>
    public async Task SignOutAsync()
    {
        _current = null;
        _loaded = true;

        await store.ClearAsync();
        Changed?.Invoke();
    }
}
