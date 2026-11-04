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
        
        // Die Lizenz von MediatR hat sich vor Kurzem geändert, so dass es nicht mehr 100% kostenlos ist. Ich habe mich entschieden, es trotzdem zu verwenden, da ich es bereits kenne und es mir die Arbeit erleichtert. Die Community-Edition ist jedoch weiterhin kostenlos und darf für dieses Projekt verwendet werden. Im PROD-Code würde ich ein anderes Paket nutzen, oder es müsste eine Lizenz beschafft werden.
        // TODO: Alternativen für MediatR suchen.
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
