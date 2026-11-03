using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries;
using FahrzeugverwaltungVPower.Core.Queries.Users;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;

public class GetAllUsersQueryHandler(IMediator mediator) : QueryHandlerBase<GetAllUsersQuery, ICollection<ApplicationUser>>(mediator)
{
    public override Task<ICollection<ApplicationUser>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
