using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;

/// <summary>
/// Liefert einen Benutzer mit Rollen anhand der ID.
/// </summary>
/// <param name="userManager">Liest Benutzer und Rollen.</param>
public class GetUserQueryHandler(UserManager<ApplicationUser> userManager) : IRequestHandler<GetUserQuery, UserDto?>
{
    /// <summary>
    /// Gibt <see langword="null"/> zurück, wenn es den Benutzer nicht gibt.
    /// </summary>
    /// <param name="request">Die Abfrage mit der Id.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
        {
            return null;
        }

        IList<string> roles = await userManager.GetRolesAsync(user);

        return UserDto.From(user, roles);
    }
}