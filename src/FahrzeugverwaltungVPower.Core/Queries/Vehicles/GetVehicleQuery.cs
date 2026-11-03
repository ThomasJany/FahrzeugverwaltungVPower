using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles;

public class GetVehicleQuery : IRequest<Vehicle>
{
    public string Vin { get; init; } = default!;
}
