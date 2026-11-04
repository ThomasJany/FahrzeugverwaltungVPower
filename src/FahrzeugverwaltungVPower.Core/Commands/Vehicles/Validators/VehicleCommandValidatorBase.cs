using FluentValidation;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;

/// <summary>
/// Gemeinsame Regeln für die Befehle zum Anlegen und Ändern eines Fahrzeugs.
/// </summary>
/// <typeparam name="T">Typ des zu prüfenden Befehls.</typeparam>
public abstract class VehicleCommandValidatorBase<T> : AbstractValidator<T>
    where T : VehicleCommandBase
{
    protected VehicleCommandValidatorBase()
    {
        // Feld A: bis 11 Zeichen, Buchstaben (inkl. Umlaute), Ziffern, Leerzeichen, Bindestrich.
        RuleFor(x => x.LicensePlate)
            .NotEmpty()
            .Matches(@"^[A-Za-zÄÖÜäöü0-9 \-]{2,11}$")
            .WithMessage("Das Kennzeichen darf 2 bis 11 Zeichen lang sein (Buchstaben, Ziffern, Leerzeichen, Bindestrich).");

        RuleFor(x => x.FirstRegistrationDate)
            .Must(d => d <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Die Erstzulassung darf nicht in der Zukunft liegen.");

        RuleFor(x => x.Holder).NotEmpty().MaximumLength(200);
        RuleFor(x => x.HolderFirstName).MaximumLength(200);

        RuleFor(x => x.Manufacturer).NotEmpty().MaximumLength(25);
        RuleFor(x => x.TypeDesignation).NotEmpty().MaximumLength(25);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(25);

        // Feld E: 17 Zeichen, ohne I, O, Q.
        RuleFor(x => x.VIN)
            .NotEmpty()
            .Matches("^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$")
            .WithMessage("Die VIN muss 17 Zeichen lang sein (Buchstaben ohne I, O, Q, und Ziffern).");

        // Feld 2.1: genau 4 Ziffern.
        RuleFor(x => x.HSN)
            .NotEmpty()
            .Matches(@"^\d{4}$")
            .WithMessage("Die HSN besteht aus genau 4 Ziffern.");

        // Feld 2.2: bis 9 Zeichen, die Prüfziffer kann auch ein Strich sein.
        RuleFor(x => x.TSN)
            .NotEmpty()
            .Matches(@"^[A-Za-z0-9\-]{1,9}$")
            .WithMessage("Die TSN darf bis zu 9 Zeichen lang sein (Buchstaben, Ziffern, Strich).");
    }
}