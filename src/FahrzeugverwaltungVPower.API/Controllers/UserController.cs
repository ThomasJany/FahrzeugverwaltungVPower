using FahrzeugverwaltungVPower.Core.Commands.Users;
using FahrzeugverwaltungVPower.Core.Queries.Users;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

/// <summary>
/// REST-Endpunkte zur Verwaltung von Benutzern, nur für Administratoren.
/// </summary>
/// <param name="mediator">Sendet Befehle und Abfragen an die Handler.</param>
[ApiController]
[Route("api/users/[action]")]
[Authorize(Policy = PolicyNames.CanManageUsers)]
public class UserController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Liefert alle Benutzer.
    /// </summary>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpGet]
    public async Task<IReadOnlyList<UserDto>> GetAllUsers(CancellationToken cancellationToken) =>
        await Mediator.Send(new GetAllUsersQuery(), cancellationToken);

    /// <summary>
    /// Liefert den Benutzer mit der angegebenen Id, oder 404, wenn es keinen gibt.
    /// </summary>
    /// <param name="id">Id des gesuchten Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        UserDto? user = await Mediator.Send(new GetUserQuery(id), cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Legt einen Benutzer an und liefert ihn mit Status 201 zurück.
    /// </summary>
    /// <param name="command">Die Daten des neuen Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        Guid id = await Mediator.Send(command, cancellationToken);
        UserDto? user = await Mediator.Send(new GetUserQuery(id), cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { id }, user);
    }

    /// <summary>
    /// Ändert den Benutzer mit der angegebenen Id, oder 404, wenn es keinen gibt.
    /// </summary>
    /// <param name="id">Id des zu ändernden Benutzers.</param>
    /// <param name="command">Die neuen Daten des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        bool updated = await Mediator.Send(command with { Id = id }, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Löscht den Benutzer mit der angegebenen Id, oder 404, wenn es keinen gibt.
    /// </summary>
    /// <param name="id">Id des zu löschenden Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await Mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}