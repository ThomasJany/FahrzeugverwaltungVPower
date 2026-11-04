using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FahrzeugverwaltungVPower.Infrastructure;

/// <summary>
/// Erweiterungsmethoden zur Registrierung der Infrastruktur.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registriert den Datenbankkontext mit SQL Server und das Interface für Core.
    /// </summary>
    /// <param name="services">Die Dienstesammlung der Anwendung.</param>
    /// <param name="connectionString">Connection String zur SQL-Server-Datenbank.</param>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, o =>
            {
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                o.CommandTimeout(300);
            }));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
