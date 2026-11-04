using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Stellt pro Test einen echten <see cref="UserManager{TUser}"/> auf einer isolierten In-Memory-Datenbank bereit.
/// </summary>
/// <remarks>
/// Ein Mock von <see cref="UserManager{TUser}"/> wäre aufwendig und würde das eigentliche Verhalten
/// (Passwortprüfung, Sperre nach Fehlversuchen, Rollen) nicht prüfen. Die Passwort- und Sperrregeln
/// entsprechen denen in <c>AddInfrastructureServices</c>. Ändern sie sich dort, müssen sie hier mitgezogen werden.
/// </remarks>
public sealed class TestIdentity : IDisposable
{
    /// <summary>E-Mail-Adresse des Testbenutzers.</summary>
    public const string Email = "max@mustermann.de";

    /// <summary>E-Mail-Adresse eines zweiten Testbenutzers, zum Prüfen von Doppelungen.</summary>
    public const string OtherEmail = "erika@mustermann.de";

    /// <summary>Passwort des Testbenutzers, erfüllt die Passwortregeln.</summary>
    public const string Password = "Testpasswort1234";

    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;

    public TestIdentity()
    {
        string databaseName = Guid.NewGuid().ToString();

        ServiceCollection services = new();
        services.AddLogging();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 12;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        _provider = services.BuildServiceProvider();
        _scope = _provider.CreateScope();

        // Legt das Modell an und übernimmt dabei die per HasData vorbelegten Rollen (die Migration tut das in der echten Datenbank).
        _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();

        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    /// <summary>
    /// Der Benutzerverwalter, wie ihn die Handler bekommen.
    /// </summary>
    public UserManager<ApplicationUser> UserManager { get; }

    /// <summary>
    /// Legt den Testbenutzer mit <see cref="Email"/> und <see cref="Password"/> und den angegebenen Rollen an.
    /// </summary>
    /// <param name="roles">Die Rollen des Benutzers, sie sind in der Datenbank vorbelegt.</param>
    /// <param name="email">E-Mail-Adresse und Login-Name, Standard ist <see cref="Email"/>.</param>
    /// <param name="firstName">Vorname, Standard ist "Max".</param>
    /// <param name="lastName">Nachname, Standard ist "Mustermann".</param>
    public async Task<ApplicationUser> CreateUserAsync(
        string[] roles, string email = Email, string firstName = "Max", string lastName = "Mustermann")
    {
        ApplicationUser user = new()
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        IdentityResult created = await UserManager.CreateAsync(user, Password);

        if (!created.Succeeded)
        {
            throw new InvalidOperationException(
                "Testbenutzer konnte nicht angelegt werden: " + string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        if (roles.Length > 0)
        {
            IdentityResult rolesAdded = await UserManager.AddToRolesAsync(user, roles);

            if (!rolesAdded.Succeeded)
            {
                throw new InvalidOperationException(
                    "Rollen konnten nicht zugewiesen werden: " + string.Join("; ", rolesAdded.Errors.Select(e => e.Description)));
            }
        }

        return user;
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
    }
}
