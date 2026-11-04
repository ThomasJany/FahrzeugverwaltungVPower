using FluentValidation;

namespace FahrzeugverwaltungVPower.Core.Commands.Auth.Validators;

/// <summary>
/// Prüft Format und Länge von E-Mail und Passwort bei der Anmeldung.
/// </summary>
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Definiert die Regeln für E-Mail und Passwort.
    /// </summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();

        // Die Obergrenze verhindert, dass absurd lange Eingaben das Hashing belasten.
        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }
}