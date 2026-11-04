using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Users;

/// <summary>
/// Befehl zum Löschen eines Benutzers. Das Ergebnis ist <see langword="false"/>, wenn es den Benutzer nicht gibt.
/// </summary>
/// <param name="Id">Id des zu löschenden Benutzers.</param>
public record DeleteUserCommand(Guid Id) : IRequest<bool>;