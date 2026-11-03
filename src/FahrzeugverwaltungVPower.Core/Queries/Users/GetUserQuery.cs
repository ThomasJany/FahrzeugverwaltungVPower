using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users;

public class GetUserQuery : IRequest<ApplicationUser>
{
    public Guid Guid { get; init; } = default!;
}
