using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

/// <summary>
/// Befehl zum Löschen eines Fahrzeugs. Das Ergebnis ist <see langword="false"/>, wenn es kein Fahrzeug mit der Id gab.
/// </summary>
/// <param name="Id">Id des zu löschenden Fahrzeugs.</param>
public record DeleteVehicleCommand(Guid Id) : IRequest<bool>;
