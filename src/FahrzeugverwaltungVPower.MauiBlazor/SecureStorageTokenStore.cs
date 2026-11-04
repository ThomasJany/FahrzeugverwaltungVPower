using System.Text.Json;
using FahrzeugverwaltungVPower.UI.Services;

namespace FahrzeugverwaltungVPower.MauiBlazor;

/// <summary>
/// Speichert die Anmeldung im sicheren Speicher des Geräts (Android Keystore, iOS Keychain, Windows-Datenschutz).
/// </summary>
/// <remarks>
/// Die Anmeldung übersteht damit auch einen Neustart der App. Ist der sichere Speicher nicht verfügbar, etwa bei
/// einer Windows-App ohne Paketidentität oder auf einem Gerät ohne Bildschirmsperre, bleibt die Anmeldung im
/// Arbeitsspeicher und gilt dann nur, bis die App geschlossen wird.
/// </remarks>
public class SecureStorageTokenStore : ITokenStore
{
    private const string Key = "fahrzeugverwaltung.session";

    private StoredSession? _inMemory;

    /// <summary>
    /// Liest die gespeicherte Anmeldung, oder <see langword="null"/>, wenn es keine (lesbare) gibt.
    /// </summary>
    public async ValueTask<StoredSession?> LoadAsync()
    {
        if (_inMemory is not null)
        {
            return _inMemory;
        }

        try
        {
            string? json = await SecureStorage.Default.GetAsync(Key);

            return json is null ? null : JsonSerializer.Deserialize<StoredSession>(json);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or PlatformNotSupportedException)
        {
            // Nicht lesbar oder kein sicherer Speicher: wie "nicht angemeldet" behandeln.
            return null;
        }
    }

    /// <summary>
    /// Speichert die Anmeldung.
    /// </summary>
    /// <param name="session">Die zu speichernde Anmeldung.</param>
    public async ValueTask SaveAsync(StoredSession session)
    {
        _inMemory = session;

        try
        {
            await SecureStorage.Default.SetAsync(Key, JsonSerializer.Serialize(session));
        }
        catch (Exception ex) when (ex is InvalidOperationException or PlatformNotSupportedException or UnauthorizedAccessException)
        {
            // Der Speicher ist nicht verfügbar: Die Anmeldung gilt nur im Arbeitsspeicher, siehe Hinweis oben.
        }
    }

    /// <summary>
    /// Löscht die gespeicherte Anmeldung.
    /// </summary>
    public ValueTask ClearAsync()
    {
        _inMemory = null;
        SecureStorage.Default.Remove(Key);

        return ValueTask.CompletedTask;
    }
}
