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
        // EmailAddress() prüft nur grob (ein @ in der Mitte) und lässt "name@host" oder Leerzeichen durch.
        // Die zweite Regel verlangt keine Leerzeichen und eine Domain mit Punkt und mindestens zwei Zeichen am Ende.
        // Beide Regeln melden denselben Fehlercode, für den Aufrufer ist es ein ungültiges Format.
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress()
            .Must(HasPlausibleDomain)
            .WithErrorCode("EmailValidator")
            .WithMessage("'{PropertyName}' ist keine gültige E-Mail-Adresse, zum Beispiel name@beispiel.de.");

        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Roles).NotEmpty();

        RuleForEach(x => x.Roles)
            .Must(role => RoleNames.All.Contains(role))
            .WithMessage("Die Rolle '{PropertyValue}' existiert nicht. Erlaubt sind: " + string.Join(", ", RoleNames.All) + ".");
    }

    private static bool HasPlausibleDomain(string email)
    {
        if (email.Any(char.IsWhiteSpace))
        {
            return false;
        }

        string domain = email[(email.LastIndexOf('@') + 1)..];
        int lastDot = domain.LastIndexOf('.');

        return lastDot >= 1 && domain.Length - lastDot - 1 >= 2;
    }
}