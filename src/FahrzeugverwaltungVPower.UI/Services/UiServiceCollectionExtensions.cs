using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Die Einstellungen der Oberfläche, die der jeweilige Host festlegt.
/// </summary>
public class UiOptions
{
    /// <summary>Adresse der API, zum Beispiel https://localhost:7139/.</summary>
    public string ApiBaseUrl { get; set; } = "https://localhost:7139/";
}

/// <summary>
/// Erweiterungsmethoden zur Registrierung der Dienste der Oberfläche.
/// </summary>
public static class UiServiceCollectionExtensions
{
    /// <summary>
    /// Registriert Anmeldung, API-Zugriff und Berechtigungsprüfung der Oberfläche.
    /// </summary>
    /// <remarks>
    /// Der Host muss zusätzlich einen <see cref="ITokenStore"/> registrieren, denn wo die Anmeldung
    /// gespeichert wird, hängt vom Host ab.
    /// </remarks>
    /// <param name="services">Die Dienstesammlung des Hosts.</param>
    /// <param name="configure">Legt die Einstellungen der Oberfläche fest, vor allem die Adresse der API.</param>
    public static IServiceCollection AddFahrzeugverwaltungUi(this IServiceCollection services, Action<UiOptions> configure)
    {
        UiOptions options = new();
        configure(options);

        services.TryAddSingleton(TimeProvider.System);

        services.AddMudServices();

        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddScoped<AuthSession>();
        services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();

        // Ein HttpClient pro Benutzersitzung. Er ruft die API serverseitig auf, im Browser ist dafür kein CORS nötig.
        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(options.ApiBaseUrl) });

        services.AddScoped<ApiClient>();
        services.AddScoped<AuthService>();

        return services;
    }
}
