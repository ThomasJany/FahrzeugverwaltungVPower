using FahrzeugverwaltungVPower.Core.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;

/// <summary>
/// Prüft die Eingaben zum Anlegen eines Fahrzeugs (gemeinsame Regeln aus der Basisklasse, doppelte VIN).
/// </summary>
public class CreateVehicleCommandValidator : VehicleCommandValidatorBase<CreateVehicleCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Ergänzt die gemeinsamen Regeln um die Prüfung auf eine doppelte VIN.
    /// </summary>
    /// <param name="context">Datenbankkontext zur Prüfung auf doppelte VIN.</param>
    public CreateVehicleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.VIN)
            .MustAsync(IsVinUniqueAsync)
            .WithMessage("Ein Fahrzeug mit dieser VIN existiert bereits.");
    }

    /// <summary>
    /// Prüft, ob noch kein Fahrzeug mit der (normalisierten) VIN existiert.
    /// </summary>
    /// <param name="vin">Die zu prüfende VIN.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> IsVinUniqueAsync(string vin, CancellationToken cancellationToken)
    {
        string normalized = vin.Trim().ToUpperInvariant();

        return !await _context.Vehicles.AnyAsync(v => v.VIN == normalized, cancellationToken);
    }
}