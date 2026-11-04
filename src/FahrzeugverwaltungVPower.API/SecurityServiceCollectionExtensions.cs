using System.Text;
using FahrzeugverwaltungVPower.Core.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace FahrzeugverwaltungVPower.API;

/// <summary>
/// Erweiterungsmethoden zur Registrierung von Authentifizierung und Berechtigungen.
/// </summary>
public static class SecurityServiceCollectionExtensions
{
    /// <summary>
    /// Registriert die JWT-Prüfung, die Berechtigungsrichtlinien und die Pflicht zur Anmeldung für alle Endpunkte.
    /// </summary>
    /// <param name="services">Die Dienstesammlung der Anwendung.</param>
    /// <param name="configuration">Die Konfiguration mit dem Abschnitt Jwt.</param>
    public static IServiceCollection AddApiSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        JwtOptions jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException($"Abschnitt '{JwtOptions.SectionName}' ist nicht konfiguriert.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services
            .AddAuthorizationBuilder()
            // Alle Endpunkte ohne eigenes [Authorize] oder [AllowAnonymous] verlangen eine Anmeldung.
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            .AddPolicy(PolicyNames.CanView, p => p.RequireRole(RoleNames.Admin, RoleNames.Creator, RoleNames.Editor, RoleNames.Viewer))
            .AddPolicy(PolicyNames.CanEdit, p => p.RequireRole(RoleNames.Admin, RoleNames.Creator, RoleNames.Editor))
            .AddPolicy(PolicyNames.CanCreate, p => p.RequireRole(RoleNames.Admin, RoleNames.Creator))
            .AddPolicy(PolicyNames.CanDelete, p => p.RequireRole(RoleNames.Admin, RoleNames.Creator))
            .AddPolicy(PolicyNames.CanManageUsers, p => p.RequireRole(RoleNames.Admin));

        return services;
    }
}