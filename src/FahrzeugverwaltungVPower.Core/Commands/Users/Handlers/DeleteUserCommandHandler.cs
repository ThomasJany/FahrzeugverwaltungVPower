using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Users;
using MediatR;

namespace FahrzeugverwaltungVPower.API.Commands.Users.Handlers;

public class DeleteUserCommandHandler(IMediator mediator) : CommandHandlerBase<DeleteUserCommand>(mediator)
{
    public override Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
