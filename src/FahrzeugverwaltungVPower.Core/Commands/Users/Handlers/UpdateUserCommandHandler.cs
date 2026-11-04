using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;

/// <summary>
/// Ändert Stammdaten, Rollen und optional das Passwort eines Benutzers.
/// </summary>
/// <param name="userManager">Liest und ändert Benutzer und Rollen.</param>
public class UpdateUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<UpdateUserCommand, bool>
{
    /// <summary>
    /// Gibt <see langword="false"/> zurück, wenn es den Benutzer nicht gibt.
    /// </summary>
    /// <param name="request">Die neuen Daten mit der Id des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return false;
        }

        if (request.NewPassword is not null)
        {
            // Zuerst prüfen, damit ein schwaches Passwort den Benutzer nicht halb geändert zurücklässt.
            await ValidatePasswordAsync(user, request.NewPassword);

            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.NewPassword);
            user.SecurityStamp = Guid.NewGuid().ToString();
        }

        string email = request.Email.Trim();
        user.UserName = email;
        user.Email = email;
        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();

        (await userManager.UpdateAsync(user)).ThrowIfFailed();

        await UpdateRolesAsync(user, request.Roles);

        return true;
    }

    /// <summary>
    /// Prüft das Passwort mit den Regeln, die Identity auch beim Anlegen anwendet.
    /// </summary>
    /// <param name="user">Der Benutzer, für den das Passwort gesetzt wird.</param>
    /// <param name="password">Das neue Passwort.</param>
    private async Task ValidatePasswordAsync(ApplicationUser user, string password)
    {
        foreach (IPasswordValidator<ApplicationUser> validator in userManager.PasswordValidators)
        {
            (await validator.ValidateAsync(userManager, user, password)).ThrowIfFailed();
        }
    }

    /// <summary>
    /// Gleicht die Rollen des Benutzers mit der gewünschten Liste ab: fehlende hinzufügen, überzählige entfernen.
    /// </summary>
    /// <param name="user">Der Benutzer.</param>
    /// <param name="roles">Die gewünschten Rollen.</param>
    private async Task UpdateRolesAsync(ApplicationUser user, IReadOnlyList<string> roles)
    {
        IList<string> currentRoles = await userManager.GetRolesAsync(user);
        List<string> wantedRoles = [.. roles.Distinct()];

        string[] toAdd = [.. wantedRoles.Except(currentRoles)];
        string[] toRemove = [.. currentRoles.Except(wantedRoles)];

        if (toAdd.Length > 0)
        {
            (await userManager.AddToRolesAsync(user, toAdd)).ThrowIfFailed();
        }

        if (toRemove.Length > 0)
        {
            (await userManager.RemoveFromRolesAsync(user, toRemove)).ThrowIfFailed();
        }
    }
}