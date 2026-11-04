using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using FahrzeugverwaltungVPower.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // AddIdentityCore statt AddIdentity, damit die Services mit JWT funktionieren (für MAUI).
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 12;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<AdminSeeder>();

        return services;
    }
}
