using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;

/// <summary>
/// Legt einen Benutzer mit Passwort und Rollen an.
/// </summary>
/// <param name="userManager">Legt den Benutzer an und weist die Rollen zu.</param>
public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<CreateUserCommand, Guid>
{
    /// <summary>
    /// Legt den Benutzer an, zugleich Login-Name ist die E-Mail-Adresse, und gibt dessen Id zurück.
    /// </summary>
    /// <param name="request">Die Daten des neuen Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        string email = request.Email.Trim();

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            EmailConfirmed = true
        };

        (await userManager.CreateAsync(user, request.Password)).ThrowIfFailed();

        try
        {
            (await userManager.AddToRolesAsync(user, request.Roles.Distinct())).ThrowIfFailed();
        }
        catch
        {
            // Keinen Benutzer ohne Rollen zurücklassen.
            await userManager.DeleteAsync(user);
            throw;
        }

        return user.Id;

    }
}