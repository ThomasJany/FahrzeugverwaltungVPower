using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Handlers;

/// <summary>
/// Legt ein Fahrzeug an und gibt dessen Id zurück.
/// </summary>
/// <param name="context">Datenbankkontext zum Speichern.</param>
/// <param name="currentUser">Liefert die Id des angemeldeten Benutzers für das Feld CreatedBy.</param>
public class CreateVehicleCommandHandler(IApplicationDbContext context, ICurrentUser currentUser) : IRequestHandler<CreateVehicleCommand, Guid>
{
    /// <summary>
    /// Normalisiert die Eingaben, speichert das Fahrzeug und gibt die Id zurück.
    /// </summary>
    /// <param name="command">Die Daten des neuen Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<Guid> Handle(CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        Vehicle vehicle = new()
        {
            LicensePlate = command.LicensePlate.Trim().ToUpperInvariant(),
            FirstRegistrationDate = command.FirstRegistrationDate,
            Holder = command.Holder.Trim(),
            HolderFirstName = command.HolderFirstName?.Trim(),
            Manufacturer = command.Manufacturer.Trim(),
            TypeDesignation = command.TypeDesignation.Trim(),
            Model = command.Model.Trim(),
            VIN = command.VIN.Trim().ToUpperInvariant(),
            InspectionValidUntil = command.InspectionValidUntil,
            HSN = command.HSN.Trim(),
            TSN = command.TSN.Trim().ToUpperInvariant(),
            CreatedAt = now,
            CreatedBy = currentUser.UserId ?? throw new InvalidOperationException("Es ist kein Benutzer angemeldet."),
            ModifiedAt = now
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}
