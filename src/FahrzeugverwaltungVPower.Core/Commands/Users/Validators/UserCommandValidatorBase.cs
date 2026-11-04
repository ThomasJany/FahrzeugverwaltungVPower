using FahrzeugverwaltungVPower.Core.Security;
using FluentValidation;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Validators;

/// <summary>
/// Gemeinsame Regeln für die Befehle zum Anlegen und Ändern eines Benutzers.
/// </summary>
/// <typeparam name="T">Typ des zu prüfenden Befehls.</typeparam>
public abstract class UserCommandValidatorBase<T> : AbstractValidator<T>
    where T : UserCommandBase
{
    protected UserCommandValidatorBase()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Roles).NotEmpty();

        RuleForEach(x => x.Roles)
            .Must(role => RoleNames.All.Contains(role))
            .WithMessage("Die Rolle '{PropertyValue}' existiert nicht. Erlaubt sind: " + string.Join(", ", RoleNames.All) + ".");
    }
}