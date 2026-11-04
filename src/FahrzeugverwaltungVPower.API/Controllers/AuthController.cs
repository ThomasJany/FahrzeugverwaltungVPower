using FahrzeugverwaltungVPower.Core.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

/// <summary>
/// Endpunkte zur Anmeldung.
/// </summary>
/// <param name="mediator">Sendet Befehle und Abfragen an die Handler.</param>
[ApiController]
[Route("api/auth/[action]")]
public class AuthController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Meldet einen Benutzer an und liefert ein Zugriffstoken, oder 401 bei falschen Zugangsdaten.
    /// </summary>
    /// <param name="command">E-Mail und Passwort des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        LoginResult? result = await Mediator.Send(command, cancellationToken);

        if (result is null)
        {
            // Absichtlich immer dieselbe Meldung, egal ob der Benutzer fehlt, gesperrt ist oder das Passwort falsch war.
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Anmeldung fehlgeschlagen",
                Detail = "E-Mail oder Passwort ist falsch."
            });
        }

        return Ok(result);
    }
}