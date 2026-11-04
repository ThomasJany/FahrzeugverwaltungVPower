using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

/// <summary>
/// Befehl zum Ändern eines Fahrzeugs. Das Ergebnis ist <see langword="false"/>, wenn es das Fahrzeug nicht gibt.
/// </summary>
public record UpdateVehicleCommand : VehicleCommandBase, IRequest<bool>
{
    /// <summary>Id des zu ändernden Fahrzeugs.</summary>
    public Guid Id { get; init; }
}