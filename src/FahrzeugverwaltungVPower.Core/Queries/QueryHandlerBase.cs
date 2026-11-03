using MediatR;

namespace FahrzeugverwaltungVPower.Core.Queries;

public abstract class QueryHandlerBase<TQuery, TResult>(IMediator mediator) : IRequestHandler<TQuery, TResult> where TQuery : IRequest<TResult>
{
    protected IMediator Mediator { get; } = mediator;

    public abstract Task<TResult> Handle(TQuery request, CancellationToken cancellationToken);
}
