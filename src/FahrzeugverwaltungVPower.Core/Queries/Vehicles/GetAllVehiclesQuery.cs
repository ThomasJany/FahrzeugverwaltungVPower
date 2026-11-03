using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles;

public class GetAllVehiclesQuery : IRequest<ICollection<Vehicle>>
{
}
