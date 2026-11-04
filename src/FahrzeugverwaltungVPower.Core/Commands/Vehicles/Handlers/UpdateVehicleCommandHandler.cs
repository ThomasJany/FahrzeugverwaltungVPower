using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

/// <summary>
/// Ändert ein vorhandenes Fahrzeug anhand der Id.
/// </summary>
/// <param name="context">Datenbankkontext zum Lesen und Speichern.</param>
public class UpdateVehicleCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateVehicleCommand, bool>
{
    /// <summary>
    /// Übernimmt die normalisierten Eingaben, gibt <see langword="false"/> zurück, wenn es das Fahrzeug nicht gibt.
    /// </summary>
    /// <param name="request">Die neuen Daten mit der Id des Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<bool> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        Vehicle? vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vehicle is null)
        {
            return false;
        }

        vehicle.LicensePlate = request.LicensePlate.Trim().ToUpperInvariant();
        vehicle.FirstRegistrationDate = request.FirstRegistrationDate;
        vehicle.Holder = request.Holder.Trim();
        vehicle.HolderFirstName = request.HolderFirstName?.Trim();
        vehicle.Manufacturer = request.Manufacturer.Trim();
        vehicle.TypeDesignation = request.TypeDesignation.Trim();
        vehicle.Model = request.Model.Trim();
        vehicle.VIN = request.VIN.Trim().ToUpperInvariant();
        vehicle.InspectionValidUntil = request.InspectionValidUntil;
        vehicle.HSN = request.HSN.Trim();
        vehicle.TSN = request.TSN.Trim().ToUpperInvariant();
        vehicle.ModifiedAt = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
