using FahrzeugverwaltungVPower.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Validators;

/// <summary>
/// Prüft die Eingaben zum Anlegen eines Benutzers (gemeinsame Regeln aus der Basisklasse, Passwort, doppelte E-Mail).
/// </summary>
public class CreateUserCommandValidator : UserCommandValidatorBase<CreateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Ergänzt die gemeinsamen Regeln um das Passwort und die Prüfung auf eine doppelte E-Mail-Adresse.
    /// </summary>
    /// <param name="userManager">Sucht vorhandene Benutzer.</param>
    public CreateUserCommandValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Email)
            .MustAsync(IsEmailUniqueAsync)
            .WithMessage("Ein Benutzer mit dieser E-Mail-Adresse existiert bereits.");

        // Die Komplexitätsregeln prüft Identity beim Anlegen, hier nur Pflicht und Obergrenze.
        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(128);
    }

    /// <summary>
    /// Prüft, ob noch kein Benutzer mit dieser E-Mail-Adresse existiert.
    /// </summary>
    /// <param name="email">Die zu prüfende E-Mail-Adresse.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken) =>
        await _userManager.FindByEmailAsync(email.Trim()) is null;
}