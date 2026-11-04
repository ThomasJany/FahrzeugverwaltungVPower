using FahrzeugverwaltungVPower.API.Security;
using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Security;
using FahrzeugverwaltungVPower.Infrastructure;
using FluentValidation;

namespace FahrzeugverwaltungVPower.API;

/// <summary>
/// Erweiterungsmethoden zur Registrierung der API-Dienste.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registriert Controller, Infrastruktur, MediatR und die Validatoren.
    /// </summary>
    /// <param name="services">Die Dienstesammlung der Anwendung.</param>
    /// <param name="configuration">Die Konfiguration mit dem Connection String.</param>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddHttpContextAccessor();
                
        string connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddInfrastructureServices(connectionString);

        services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        
        // MediatR trotz geänderter Lizenz
        //
        // MediatR 14.2.0 steht laut Reciprocal Public License 1.5 (RPL-1.5). Für Einzelpersonen,
        // Bildung und kleine Firmen gibt es laut Hersteller eine kostenlose Community-Lizenz; die genauen
        // Bedingungen stehen nicht im Paket, sondern auf der Seite des Herstellers. Ein Lizenzschlüssel ist
        // laut Hersteller nur im produktiven kommerziellen Einsatz nötig, ohne Schlüssel erscheint eine
        // Warnung im Log.
        //
        // Warum trotzdem MediatR: Das Muster aus Command, Query, Handler und Pipeline (hier für die
        // Validierung) ist mir vertraut und spart in diesem Übungsprojekt viel Code. Die Nutzung hier ist ein
        // nicht-kommerzielles Lernprojekt.
        //
        // Für Produktion: Lizenz beschaffen oder wechseln, zum Beispiel Handler direkt per Dependency
        // Injection aufrufen oder ein freies Mediator-Paket verwenden. Die Handler hängen nur an
        // IRequestHandler, ein Wechsel betrifft vor allem diese Registrierung und die Pipeline.
        //
        // TODO: Alternativen für MediatR prüfen.
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Core.Entities.Entity).Assembly);
            cfg.AddOpenBehavior(typeof(Core.Behaviors.ValidationBehavior<,>));
        });
        
        services.AddValidatorsFromAssembly(typeof(Core.Entities.Entity).Assembly);
        
        services.AddApiSecurity(configuration);
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }
}
