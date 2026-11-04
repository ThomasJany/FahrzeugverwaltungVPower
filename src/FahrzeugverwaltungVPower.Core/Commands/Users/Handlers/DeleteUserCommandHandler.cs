using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Handlers;

/// <summary>
/// Löscht einen Benutzer endgültig.
/// </summary>
/// <param name="userManager">Sucht und löscht den Benutzer.</param>
public class DeleteUserCommandHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<DeleteUserCommand, bool>
{
    /// <summary>
    /// Gibt <see langword="false"/> zurück, wenn es den Benutzer nicht gibt.
    /// </summary>
    /// <param name="request">Der Löschbefehl mit der Id.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return false;
        }

        (await userManager.DeleteAsync(user)).ThrowIfFailed();

        return true;
    }
}