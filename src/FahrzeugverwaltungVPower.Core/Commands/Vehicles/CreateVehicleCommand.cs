using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

public record CreateVehicleCommand : IRequest
{
    Vehicle Vehicle { get; init; } = default!;
}
