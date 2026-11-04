using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Behaviors;

/// <summary>
/// MediatR-Pipeline, die vor jedem Handler alle passenden Validatoren ausführt und bei Fehlern eine <see cref="ValidationException"/> wirft.
/// </summary>
/// <typeparam name="TRequest">Typ der Anfrage.</typeparam>
/// <typeparam name="TResponse">Typ der Antwort.</typeparam>
/// <param name="validators">Alle für die Anfrage registrierten Validatoren.</param>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Validiert die Anfrage und ruft danach den nächsten Schritt der Pipeline auf.
    /// </summary>
    /// <param name="request">Die zu validierende Anfrage.</param>
    /// <param name="next">Der nächste Schritt der Pipeline.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators != null && validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            List<ValidationFailure> failures = [];

            foreach (IValidator<TRequest> validator in validators)
            {
                ValidationResult result = await validator.ValidateAsync(context, cancellationToken);
                failures.AddRange(result.Errors);
            }

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(cancellationToken);
    }
}
