using System.Security.Cryptography;
using System.Text.Json;
using FahrzeugverwaltungVPower.UI.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FahrzeugverwaltungVPower.Blazor.Services;

/// <summary>
/// Speichert die Anmeldung verschlüsselt im Session-Speicher des Browsers (pro Tab).
/// </summary>
/// <remarks>
/// Die Anmeldung übersteht damit ein Neuladen der Seite, geht aber beim Schließen des Tabs verloren. Der Inhalt
/// ist mit den Datenschutzschlüsseln des Servers verschlüsselt und im Browser nicht als Klartext lesbar.
/// Diese Klasse funktioniert nur in einer interaktiven Verbindung, nicht beim Vorab-Rendern.
/// </remarks>
/// <param name="storage">Der verschlüsselte Session-Speicher von Blazor Server.</param>
public class SessionStorageTokenStore(ProtectedSessionStorage storage) : ITokenStore
{
    private const string Key = "fahrzeugverwaltung.session";

    /// <summary>
    /// Liest die gespeicherte Anmeldung, oder <see langword="null"/>, wenn es keine (lesbare) gibt.
    /// </summary>
    public async ValueTask<StoredSession?> LoadAsync()
    {
        try
        {
            ProtectedBrowserStorageResult<StoredSession> result = await storage.GetAsync<StoredSession>(Key);

            return result.Success ? result.Value : null;
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException)
        {
            // Nicht lesbar, zum Beispiel nach einem Neustart des Servers mit neuen Schlüsseln: wie "nicht angemeldet" behandeln.
            return null;
        }
    }

    /// <summary>
    /// Speichert die Anmeldung.
    /// </summary>
    /// <param name="session">Die zu speichernde Anmeldung.</param>
    public ValueTask SaveAsync(StoredSession session) => storage.SetAsync(Key, session);

    /// <summary>
    /// Löscht die gespeicherte Anmeldung.
    /// </summary>
    public ValueTask ClearAsync() => storage.DeleteAsync(Key);
}
