using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Queries.Vehicles.Handlers;

/// <summary>
/// Liefert ein Fahrzeug anhand der VIN.
/// </summary>
/// <param name="context">Datenbankkontext zum Lesen.</param>
public class GetVehicleQueryHandler(IApplicationDbContext context) : IRequestHandler<GetVehicleQuery, Vehicle?>
{
    /// <summary>
    /// Normalisiert die VIN und sucht das Fahrzeug, <see langword="null"/> wenn es keines gibt.
    /// </summary>
    /// <param name="request">Die Abfrage mit der VIN.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<Vehicle?> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        string vin = request.Vin.Trim().ToUpperInvariant();

        return await context.Vehicles.AsNoTracking()
                                     .FirstOrDefaultAsync(v => v.VIN == vin, cancellationToken);
    }
}
