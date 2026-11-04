using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;

/// <summary>
/// Liefert alle gespeicherten Fahrzeuge.
/// </summary>
/// <param name="context">Datenbankkontext zum Lesen.</param>
public class GetAllVehiclesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAllVehiclesQuery, ICollection<Vehicle>>
{
    /// <summary>
    /// Liest alle Fahrzeuge ohne Änderungsverfolgung.
    /// </summary>
    /// <param name="request">Die Abfrage, sie enthält keine Parameter.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<ICollection<Vehicle>> Handle(GetAllVehiclesQuery request, CancellationToken cancellationToken) =>
        await context.Vehicles.AsNoTracking().ToListAsync(cancellationToken);
}
