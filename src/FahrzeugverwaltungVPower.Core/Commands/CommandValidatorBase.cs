using FluentValidation;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands;

public abstract class CommandValidatorBase<T>(IMediator mediator) : AbstractValidator<T>
{
    protected IMediator Mediator { get; } = mediator;
}
