using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FahrzeugverwaltungVPower.Infrastructure.Data;

/// <summary>
/// Legt beim ersten Start den Administrator an, sofern es noch keinen gibt.
/// </summary>
/// <param name="UserManager">Legt den Benutzer an und weist die Rolle zu.</param>
/// <param name="Configuration">Liefert die Einstellungen im Abschnitt Seed.</param>
/// <param name="Logger">Protokolliert das Ergebnis.</param>
public class AdminSeeder(
    UserManager<ApplicationUser> UserManager,
    IConfiguration Configuration,
    ILogger<AdminSeeder> Logger)
{
    /// <summary>
    /// Legt den Administrator an, wenn es noch keinen gibt und E-Mail und Passwort konfiguriert sind.
    /// </summary>
    public async Task SeedAsync()
    {
        IList<ApplicationUser> admins = await UserManager.GetUsersInRoleAsync(RoleNames.Admin);

        if (admins.Count > 0)
        {
            return;
        }

        string? email = Configuration["Seed:AdminEmail"];
        string? password = Configuration["Seed:AdminPassword"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Logger.LogWarning("Es gibt keinen Administrator, und Seed:AdminEmail oder Seed:AdminPassword sind nicht gesetzt.");
            return;
        }

        ApplicationUser admin = new()
        {
            UserName = email,
            Email = email,
            FirstName = Configuration["Seed:AdminFirstName"] ?? "Admin",
            LastName = Configuration["Seed:AdminLastName"] ?? "Administrator",
            EmailConfirmed = true
        };

        IdentityResult created = await UserManager.CreateAsync(admin, password);

        if (!created.Succeeded)
        {
            throw new InvalidOperationException(
                "Der Administrator konnte nicht angelegt werden: " + string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        IdentityResult roleAdded = await UserManager.AddToRoleAsync(admin, RoleNames.Admin);

        if (!roleAdded.Succeeded)
        {
            throw new InvalidOperationException(
                "Dem Administrator konnte die Rolle nicht zugewiesen werden: " + string.Join("; ", roleAdded.Errors.Select(e => e.Description)));
        }

        Logger.LogInformation("Administrator {Email} wurde angelegt.", email);
    }
}
