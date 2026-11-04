using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

/// <summary>
/// Befehl zum Anlegen eines Fahrzeugs. Das Ergebnis ist die Id des neuen Fahrzeugs.
/// </summary>
public record CreateVehicleCommand : VehicleCommandBase, IRequest<Guid>;
