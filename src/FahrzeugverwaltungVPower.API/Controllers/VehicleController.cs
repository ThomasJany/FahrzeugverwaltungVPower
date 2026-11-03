using FahrzeugverwaltungVPower.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

[ApiController]
[Route("api/[action]")]
public class VehicleController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    public Vehicle GetAllVehicles()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{vin}")]
    public IActionResult GetVehicleByVin(string vin)
    {
        //Mediator.Send(new GetVehicleQuery(vin));
        return Ok();
    }

    [HttpPost]
    public IActionResult CreateVehicle([FromBody] Vehicle vehicle)
    {
        // Mediator.Send(new CreateVehicleCommand(vehicle));

        // Commands, Mediator und Validator

        return Ok(vehicle);
    }

    [HttpPut]
    public IActionResult UpdateVehicle([FromBody] Vehicle vehicle)
    {
        // Mediator.Send(new UpdateVehicleCommand(vehicle));

        // Commands, Mediator und Validator

        return Ok(vehicle);
    }

    [HttpDelete("{vin}")]
    public IActionResult DeleteVehicleByVin(string vin)
    {
        // Mediator.Send(new DeleteVehicleCommand(vin));

        // Commands, Mediator und Validator

        return Ok();
    }


}
