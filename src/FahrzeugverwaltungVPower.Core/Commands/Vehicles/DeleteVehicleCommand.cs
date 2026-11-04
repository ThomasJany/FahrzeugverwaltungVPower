using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles;

/// <summary>
/// Befehl zum Löschen eines Fahrzeugs. Das Ergebnis ist <see langword="false"/>, wenn es kein Fahrzeug mit der VIN gab.
/// </summary>
/// <param name="Vin">VIN des zu löschenden Fahrzeugs.</param>
public record DeleteVehicleCommand(string Vin) : IRequest<bool>;
