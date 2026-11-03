using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Users;
using MediatR;

namespace FahrzeugverwaltungVPower.API.Commands.Users.Handlers;

public class UpdateUserCommandHandler(IMediator mediator) : CommandHandlerBase<UpdateUserCommand>(mediator)
{
    public override Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
