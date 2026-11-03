using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Users;
using MediatR;

namespace FahrzeugverwaltungVPower.API.Commands.Users.Handlers;

public class CreateUserCommandHandler(IMediator mediator) : CommandHandlerBase<CreateUserCommand>(mediator)
{
    public override Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
