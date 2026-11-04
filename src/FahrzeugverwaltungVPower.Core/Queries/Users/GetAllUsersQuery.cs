using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries.Users;

/// <summary>
/// Abfrage aller Benutzer.
/// </summary>
public record GetAllUsersQuery : IRequest<IReadOnlyList<UserDto>>;