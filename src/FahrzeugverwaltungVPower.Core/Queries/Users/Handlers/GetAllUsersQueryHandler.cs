using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;

/// <summary>
/// Liefert alle Benutzer mit ihren Rollen.
/// </summary>
/// <param name="userManager">Liest Benutzer und Rollen.</param>
public class GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    /// <summary>
    /// Liest alle Benutzer sortiert nach Nachname und Vorname.
    /// </summary>
    /// <param name="request">Die Abfrage, sie enthält keine Parameter.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        List<ApplicationUser> users = await userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);

        List<UserDto> result = [];

        foreach (ApplicationUser user in users)
        {
            IList<string> roles = await userManager.GetRolesAsync(user);
            result.Add(UserDto.From(user, roles));
        }

        return result;
    }
}