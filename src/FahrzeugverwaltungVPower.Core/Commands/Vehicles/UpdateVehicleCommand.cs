using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

public record UpdateVehicleCommand : IRequest
{
    public Vehicle Vehicle { get; init; } = default!;
}
