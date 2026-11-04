using FahrzeugverwaltungVPower.Core.Abstractions;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

public class UpdateVehicleCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateVehicleCommand, bool>
{
    public Task<bool> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
