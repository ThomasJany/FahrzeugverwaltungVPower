using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;

public class GetAllVehiclesQueryHandler(IMediator mediator) : QueryHandlerBase<GetAllVehiclesQuery, ICollection<Vehicle>>(mediator)
{
    public override Task<ICollection<Vehicle>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
