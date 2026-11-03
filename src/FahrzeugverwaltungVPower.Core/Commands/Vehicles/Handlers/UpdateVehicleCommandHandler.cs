using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

public class UpdateVehicleCommandHandler(IMediator mediator) : CommandHandlerBase<UpdateVehicleCommand>(mediator)
{
    public override Task Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
