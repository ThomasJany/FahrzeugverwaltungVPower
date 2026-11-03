using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;

public class GetVehicleQueryHandler(IMediator mediator) : QueryHandlerBase<GetVehicleQuery, Vehicle>(mediator)
{
    public override Task<Vehicle> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
