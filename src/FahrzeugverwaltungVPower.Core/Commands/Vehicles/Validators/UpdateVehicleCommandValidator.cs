using FahrzeugverwaltungVPower.Core.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;

/// <summary>
/// Prüft die Eingaben zum Ändern eines Fahrzeugs (gemeinsame Regeln aus der Basisklasse, VIN eines anderen Fahrzeugs).
/// </summary>
public class UpdateVehicleCommandValidator : VehicleCommandValidatorBase<UpdateVehicleCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Ergänzt die gemeinsamen Regeln um die Prüfung auf die VIN eines anderen Fahrzeugs.
    /// </summary>
    /// <param name="context">Datenbankkontext zur Prüfung auf doppelte VIN.</param>
    public UpdateVehicleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.VIN)
            .MustAsync((command, vin, cancellationToken) => IsVinUniqueAsync(command.Id, vin, cancellationToken))
            .WithMessage("Ein anderes Fahrzeug mit dieser VIN existiert bereits.");
    }

    /// <summary>
    /// Prüft, ob kein anderes Fahrzeug als das mit der angegebenen Id die (normalisierte) VIN trägt.
    /// </summary>
    /// <param name="id">Id des zu ändernden Fahrzeugs.</param>
    /// <param name="vin">Die zu prüfende VIN.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> IsVinUniqueAsync(Guid id, string vin, CancellationToken cancellationToken)
    {
        string normalized = vin.Trim().ToUpperInvariant();

        return !await _context.Vehicles.AnyAsync(v => v.VIN == normalized && v.Id != id, cancellationToken);
    }
}