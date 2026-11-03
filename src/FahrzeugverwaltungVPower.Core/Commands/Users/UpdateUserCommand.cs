using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Users;

public class UpdateUserCommand : IRequest
{
    public ApplicationUser User { get; init; } = default!;
}
