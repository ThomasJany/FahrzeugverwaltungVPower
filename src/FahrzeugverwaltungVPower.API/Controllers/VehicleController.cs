using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

/// <summary>
/// REST-Endpunkte zur Verwaltung von Fahrzeugen.
/// </summary>
/// <param name="mediator">Sendet Befehle und Abfragen an die Handler.</param>
[ApiController]
[Route("api/vehicles/[action]")]
public class VehicleController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Liefert alle Fahrzeuge.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = PolicyNames.CanView)]
    public async Task<ICollection<Vehicle>> GetAllVehicles() =>
        await Mediator.Send(new GetAllVehiclesQuery());

    /// <summary>
    /// Liefert das Fahrzeug mit der angegebenen Id, oder 404, wenn es keines gibt.
    /// </summary>
    /// <param name="id">Id des gesuchten Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = PolicyNames.CanView)]
    public async Task<ActionResult<Vehicle>> GetVehicleById(Guid id, CancellationToken cancellationToken)
    {
        Vehicle? vehicle = await Mediator.Send(new GetVehicleQuery(id), cancellationToken);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    /// <summary>
    /// Legt ein Fahrzeug an und gibt dessen Id zurück.
    /// </summary>
    /// <param name="command">Die Daten des neuen Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpPost]
    [Authorize(Policy = PolicyNames.CanCreate)]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        Guid id = await Mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    /// <summary>
    /// Ändert das Fahrzeug mit der angegebenen Id, oder 404, wenn es keines gibt. Die VIN kann dabei geändert werden.
    /// </summary>
    /// <param name="id">Id des zu ändernden Fahrzeugs.</param>
    /// <param name="command">Die neuen Daten des Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = PolicyNames.CanEdit)]
    public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleCommand command, CancellationToken cancellationToken)
    {
        bool updated = await Mediator.Send(command with { Id = id }, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    /// <summary>
    /// Löscht das Fahrzeug mit der angegebenen Id, oder 404, wenn es keines gibt.
    /// </summary>
    /// <param name="id">Id des zu löschenden Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PolicyNames.CanDelete)]
    public async Task<IActionResult> DeleteVehicleById(Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await Mediator.Send(new DeleteVehicleCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
