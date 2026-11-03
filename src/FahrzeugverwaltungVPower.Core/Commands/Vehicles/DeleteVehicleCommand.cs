using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

public record DeleteVehicleCommand : IRequest
{
    public int Vin { get; init; } = default!;
}
