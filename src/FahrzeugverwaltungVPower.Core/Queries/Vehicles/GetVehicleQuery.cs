using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles;

/// <summary>
/// Abfrage eines Fahrzeugs anhand der VIN. Das Ergebnis ist <see langword="null"/>, wenn es keines gibt.
/// </summary>
/// <param name="Vin">VIN des gesuchten Fahrzeugs.</param>
public record GetVehicleQuery(string Vin) : IRequest<Vehicle?>;
