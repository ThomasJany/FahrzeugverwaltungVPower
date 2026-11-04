using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Auth.Handlers;

/// <summary>
/// Meldet einen Benutzer an und stellt bei Erfolg ein Zugriffstoken aus.
/// </summary>
/// <param name="userManager">Sucht den Benutzer und prüft das Passwort.</param>
/// <param name="tokenService">Erzeugt das Zugriffstoken.</param>
public class LoginCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenService tokenService)
    : IRequestHandler<LoginCommand, LoginResult?>
{
    /// <summary>
    /// Gibt <see langword="null"/> zurück, wenn der Benutzer unbekannt oder gesperrt ist oder das Passwort nicht stimmt.
    /// </summary>
    /// <param name="request">E-Mail und Passwort.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<LoginResult?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByEmailAsync(request.Email.Trim());

        if (user is null || await userManager.IsLockedOutAsync(user))
        {
            return null;
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            // Zählt den Fehlversuch und sperrt das Konto nach der konfigurierten Anzahl.
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);

        IList<string> roles = await userManager.GetRolesAsync(user);
        AccessToken token = tokenService.CreateToken(user, roles);

        return new LoginResult(token.Token, token.ExpiresAt, user.Email ?? request.Email, user.FirstName, user.LastName, [.. roles]);
    }
}