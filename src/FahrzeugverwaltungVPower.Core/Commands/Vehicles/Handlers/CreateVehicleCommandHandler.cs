using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

public class CreateVehicleCommandHandler(IMediator mediator) : CommandHandlerBase<CreateVehicleCommand>(mediator)
{
    public override Task Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
