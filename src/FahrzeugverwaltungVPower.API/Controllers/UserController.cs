using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Core.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

[ApiController]
[Route("api/[action]")]
[Authorize(Policy = PolicyNames.CanManageUsers)]
public class UserController(IMediator mediator) : ApiControllerBase(mediator)
{
    [HttpGet]
    public ApplicationUser GetAllUsers()
    {
        return new ApplicationUser
        {
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@mustermann.de"
        };
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] ApplicationUser user)
    {
        return Ok(user);
    }

    [HttpPut]
    public IActionResult UpdateUser([FromBody] ApplicationUser user)
    {
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUserById(Guid id)
    {
        return Ok();
    }
}
