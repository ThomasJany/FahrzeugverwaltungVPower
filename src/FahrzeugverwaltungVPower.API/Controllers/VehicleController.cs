using FahrzeugverwaltungVPower.Core.Commands.Vehicles;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Queries.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

/// <summary>
/// REST-Endpunkte zur Verwaltung von Fahrzeugen.
/// </summary>
/// <param name="mediator">Sendet Befehle und Abfragen an die Handler.</param>
[ApiController]
[Route("api/[action]")]
public class VehicleController(IMediator mediator) : ApiControllerBase(mediator)
{
    /// <summary>
    /// Liefert alle Fahrzeuge.
    /// </summary>
    [HttpGet]
    public async Task<ICollection<Vehicle>> GetAllVehicles() =>
        await Mediator.Send(new GetAllVehiclesQuery());

    /// <summary>
    /// Liefert das Fahrzeug mit der angegebenen VIN, oder 404, wenn es keines gibt.
    /// </summary>
    /// <param name="vin">VIN des gesuchten Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpGet("{vin}")]
    public async Task<ActionResult<Vehicle>> GetVehicleByVin(string vin, CancellationToken cancellationToken)
    {
        Vehicle? vehicle = await Mediator.Send(new GetVehicleQuery(vin), cancellationToken);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    /// <summary>
    /// Legt ein Fahrzeug an und gibt dessen Id zurück.
    /// </summary>
    /// <param name="command">Die Daten des neuen Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleCommand command, CancellationToken cancellationToken)
    {
        Guid id = await Mediator.Send(command, cancellationToken);
        return Ok(id);
    }

    /// <summary>
    /// Aktualisiert ein Fahrzeug.
    /// </summary>
    /// <param name="vehicle">Das zu aktualisierende Fahrzeug.</param>
    [HttpPut]
    public IActionResult UpdateVehicle([FromBody] Vehicle vehicle)
    {
        // Mediator.Send(new UpdateVehicleCommand(vehicle));

        // Commands, Mediator und Validator

        return Ok(vehicle);
    }

    /// <summary>
    /// Löscht das Fahrzeug mit der angegebenen VIN.
    /// </summary>
    /// <param name="vin">VIN des zu löschenden Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    [HttpDelete("{vin}")]
    public async Task<IActionResult> DeleteVehicleByVin(string vin, CancellationToken cancellationToken)
    {
        bool deleted = await Mediator.Send(new DeleteVehicleCommand(vin), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
