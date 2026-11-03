using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands;

public abstract class CommandHandlerBase<T>(IMediator mediator) : IRequestHandler<T> where T : IRequest
{
    protected IMediator Mediator { get; } = mediator;

    public abstract Task Handle(T request, CancellationToken cancellationToken);
}