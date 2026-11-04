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
    /// Definiert die Regeln für die Id des zu löschenden Fahrzeugs.
    /// </summary>
    /// <param name="context">Datenbankkontext zur Prüfung, ob das Fahrzeug existiert.</param>
    public DeleteVehicleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(VehicleExistsAsync)
            .WithMessage("Ein Fahrzeug mit dieser Id existiert nicht.")
            .WithErrorCode(CustomErrorCodes.NotFound);
    }

    /// <summary>
    /// Prüft, ob ein Fahrzeug mit dieser Id existiert.
    /// </summary>
    /// <param name="id">Die zu prüfende Id.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> VehicleExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Vehicles.AnyAsync(v => v.Id == id, cancellationToken);
    }
}