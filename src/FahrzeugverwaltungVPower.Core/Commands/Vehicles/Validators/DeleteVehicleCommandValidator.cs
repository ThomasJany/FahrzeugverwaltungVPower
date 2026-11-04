using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;

/// <summary>
/// Prüft, ob das zu löschende Fahrzeug existiert.
/// </summary>
public class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Definiert die Regeln für die VIN des zu löschenden Fahrzeugs.
    /// </summary>
    /// <param name="context">Datenbankkontext zur Prüfung, ob das Fahrzeug existiert.</param>
    public DeleteVehicleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Vin)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(VehicleExistsAsync)
            .WithMessage("Ein Fahrzeug mit dieser VIN existiert nicht.")
            .WithErrorCode(CustomErrorCodes.NotFound);
    }

    /// <summary>
    /// Prüft, ob ein Fahrzeug mit der (normalisierten) VIN existiert.
    /// </summary>
    /// <param name="vin">Die zu prüfende VIN.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> VehicleExistsAsync(string vin, CancellationToken cancellationToken)
    {
        string normalized = vin.Trim().ToUpperInvariant();

        return await _context.Vehicles.AnyAsync(v => v.VIN == normalized, cancellationToken);
    }
}