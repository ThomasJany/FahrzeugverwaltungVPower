using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API.Controllers;

public abstract class ApiControllerBase(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;
}
