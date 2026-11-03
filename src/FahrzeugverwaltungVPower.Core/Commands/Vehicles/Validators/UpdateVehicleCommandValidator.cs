using FahrzeugverwaltungVPower.Core.Commands;
using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FluentValidation;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Vehicles.Validators;

public class UpdateVehicleCommandValidator : CommandValidatorBase<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator(IMediator mediator) : base(mediator)
    {
        RuleFor(c => c.Vehicle.Holder)
            .NotEmpty()
            .WithMessage("Vehicle holder is required.");

        RuleFor(c => c.Vehicle.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required.");

        RuleFor(c => c.Vehicle.VIN)
            .NotEmpty()
            .WithMessage("Vehicle VIN is required.");

        RuleFor(c => c.Vehicle.LicensePlate)
            .NotEmpty()
            .WithMessage("Vehicle license plate is required.");

        RuleFor(c => c.Vehicle.FirstRegistrationDate)
            .NotNull()
            .WithMessage("Vehicle first registration date is required.");

        RuleFor(c => c.Vehicle.InspectionValidUntil)
            .NotNull()
            .WithMessage("Vehicle inspection valid until date is required.");

        RuleFor(c => c.Vehicle.Manufacturer)
            .NotEmpty()
            .WithMessage("Vehicle manufacturer is required.");

        RuleFor(c => c.Vehicle.HSN)
            .NotEmpty()
            .WithMessage("Vehicle HSN is required.");

        RuleFor(c => c.Vehicle.TSN)
            .NotEmpty()
            .WithMessage("Vehicle TSN is required.");
    }
}
