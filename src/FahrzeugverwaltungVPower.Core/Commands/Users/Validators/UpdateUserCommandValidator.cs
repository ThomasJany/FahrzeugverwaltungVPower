using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Validators;

/// <summary>
/// Prüft die Eingaben zum Ändern eines Benutzers (gemeinsame Regeln aus der Basisklasse, Existenz, E-Mail, eigene Rollen).
/// </summary>
public class UpdateUserCommandValidator : UserCommandValidatorBase<UpdateUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Ergänzt die gemeinsamen Regeln um die Prüfungen, die nur beim Ändern gelten.
    /// </summary>
    /// <param name="userManager">Sucht vorhandene Benutzer und ihre Rollen.</param>
    /// <param name="currentUser">Liefert den angemeldeten Benutzer.</param>
    public UpdateUserCommandValidator(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;

        RuleFor(x => x.Id)
            .MustAsync(UserExistsAsync)
            .WithMessage("Ein Benutzer mit dieser Id existiert nicht.")
            .WithErrorCode(CustomErrorCodes.NotFound);

        RuleFor(x => x.Email)
            .MustAsync((command, email, _) => IsEmailUniqueAsync(command.Id, email))
            .WithMessage("Ein anderer Benutzer mit dieser E-Mail-Adresse existiert bereits.");

        RuleFor(x => x.Roles)
            .MustAsync((command, roles, _) => KeepsOwnRolesAsync(command.Id, roles))
            .WithMessage("Sie können Ihre eigenen Rollen nicht ändern.");

        // Ohne Angabe bleibt das Passwort unverändert, die Komplexitätsregeln prüft der Handler über Identity.
        When(x => x.NewPassword is not null, () =>
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MaximumLength(128));
    }

    /// <summary>
    /// Prüft, ob es einen Benutzer mit dieser Id gibt.
    /// </summary>
    /// <param name="id">Id des zu ändernden Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken) =>
        await _userManager.FindByIdAsync(id.ToString()) is not null;

    /// <summary>
    /// Prüft, dass kein anderer Benutzer als der mit der angegebenen Id diese E-Mail-Adresse trägt.
    /// </summary>
    /// <param name="id">Id des zu ändernden Benutzers.</param>
    /// <param name="email">Die zu prüfende E-Mail-Adresse.</param>
    private async Task<bool> IsEmailUniqueAsync(Guid id, string email)
    {
        ApplicationUser? existing = await _userManager.FindByEmailAsync(email.Trim());

        return existing is null || existing.Id == id;
    }

    /// <summary>
    /// Prüft, dass ein Benutzer, der sich selbst ändert, seine Rollen nicht verändert.
    /// </summary>
    /// <param name="id">Id des zu ändernden Benutzers.</param>
    /// <param name="roles">Die neu gewünschten Rollen.</param>
    private async Task<bool> KeepsOwnRolesAsync(Guid id, IReadOnlyList<string> roles)
    {
        if (_currentUser.UserId != id)
        {
            return true;
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(id.ToString());

        if (user is null)
        {
            return true;
        }

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);

        return roles.ToHashSet().SetEquals(currentRoles);
    }
}