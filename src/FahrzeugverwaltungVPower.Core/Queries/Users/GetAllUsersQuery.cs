using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users;

public class GetAllUsersQuery : IRequest<ICollection<ApplicationUser>>
{
}
