using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles;

/// <summary>
/// Abfrage aller Fahrzeuge.
/// </summary>
public class GetAllVehiclesQuery : IRequest<ICollection<Vehicle>>;
