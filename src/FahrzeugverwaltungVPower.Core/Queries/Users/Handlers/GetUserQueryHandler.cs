using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries;
using FahrzeugverwaltungVPower.Core.Queries.Users;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users.Handlers;

public class GetUserQueryHandler(IMediator mediator) : QueryHandlerBase<GetUserQuery, ApplicationUser>(mediator)
{
    public override Task<ApplicationUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
