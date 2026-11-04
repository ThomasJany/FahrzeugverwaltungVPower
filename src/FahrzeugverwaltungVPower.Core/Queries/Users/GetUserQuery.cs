using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users;

/// <summary>
/// Abfrage eines Benutzers anhand der Id. Das Ergebnis ist <see langword="null"/>, wenn es diesen nicht gibt.
/// </summary>
/// <param name="Id">Id des gesuchten Benutzers.</param>
public record GetUserQuery(Guid Id) : IRequest<UserDto?>;