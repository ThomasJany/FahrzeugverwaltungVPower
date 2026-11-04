using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Commands.Users.Validators;

/// <summary>
/// Prüft, ob der zu löschende Benutzer existiert und nicht der angemeldete Benutzer selbst ist.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    /// <summary>
    /// Definiert die Regeln für die Id des zu löschenden Benutzers.
    /// </summary>
    /// <param name="userManager">Sucht den Benutzer.</param>
    /// <param name="currentUser">Liefert den angemeldeten Benutzer.</param>
    public DeleteUserCommandValidator(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;

        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .MustAsync(UserExistsAsync)
            .WithMessage("Ein Benutzer mit dieser Id existiert nicht.")
            .WithErrorCode(CustomErrorCodes.NotFound)
            .Must(id => id != _currentUser.UserId)
            .WithMessage("Sie können sich nicht selbst löschen.");
    }

    /// <summary>
    /// Prüft, ob es einen Benutzer mit dieser Id gibt.
    /// </summary>
    /// <param name="id">Id des zu löschenden Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private async Task<bool> UserExistsAsync(Guid id, CancellationToken cancellationToken) =>
        await _userManager.FindByIdAsync(id.ToString()) is not null;
}