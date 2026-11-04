using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles;

/// <summary>
/// Abfrage eines Fahrzeugs anhand der Id. Das Ergebnis ist <see langword="null"/>, wenn es keines gibt.
/// </summary>
/// <param name="Id">Id des gesuchten Fahrzeugs.</param>
public record GetVehicleQuery(Guid Id) : IRequest<Vehicle?>;
