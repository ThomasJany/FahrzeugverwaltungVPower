using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

/// <summary>
/// Löscht ein Fahrzeug anhand der Id.
/// </summary>
/// <param name="context">Datenbankkontext zum Löschen.</param>
public class DeleteVehicleCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteVehicleCommand, bool>
{
    /// <summary>
    /// Gibt <see langword="true"/> zurück, wenn das Fahrzeug gelöscht wurde. Ansonsten <see langword="false"/>.
    /// </summary>
    /// <param name="request">Der Löschbefehl mit der Id.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<bool> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        Vehicle? vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vehicle is null)
        {
            return false;
        }

        context.Vehicles.Remove(vehicle);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
