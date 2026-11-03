using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

public class DeleteVehicleCommandHandler(IMediator mediator) : CommandHandlerBase<DeleteVehicleCommand>(mediator)
{
    public override Task Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
