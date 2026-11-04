using System.Reflection;
using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Commands.Auth;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FahrzeugverwaltungVPower.Core.Behaviors;

/// <summary>
/// MediatR-Pipeline als "Interceptor": Sie umschließt jede Anfrage (Anlegen, Lesen, Ändern, Löschen, Anmelden) und
/// schreibt ins Log, wer was getan hat, und jede unerwartete Exception.
/// </summary>
/// <typeparam name="TRequest">Typ der Anfrage.</typeparam>
/// <typeparam name="TResponse">Typ der Antwort.</typeparam>
/// <param name="logger">Das Log.</param>
/// <param name="currentUser">Der Benutzer, der die Anfrage stellt.</param>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string Action = ActionName(typeof(TRequest).Name);
    private static readonly PropertyInfo? IdProperty = typeof(TRequest).GetProperty("Id");

    /// <summary>
    /// Führt die Anfrage aus und protokolliert das Ergebnis.
    /// </summary>
    /// <param name="request">Die Anfrage.</param>
    /// <param name="next">Der nächste Schritt der Pipeline.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            TResponse response = await next(cancellationToken);

            LogResult(request, response);

            return response;
        }
        catch (ValidationException exception)
        {
            LogRejected(request, exception);

            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "{Action} mit unerwartetem Fehler abgebrochen. Benutzer: {UserId}, Id: {EntityId}",
                Action, currentUser.UserId, EntityId(request, default));

            throw;
        }
    }

    private void LogResult(TRequest request, TResponse? response)
    {
        if (request is LoginCommand login)
        {
            if (response is LoginResult)
            {
                logger.LogInformation("Anmeldung erfolgreich. E-Mail: {Email}", login.Email);
            }
            else
            {
                logger.LogWarning("Anmeldung fehlgeschlagen. E-Mail: {Email}", login.Email);
            }

            return;
        }

        if (response is false)
        {
            // Ändern oder Löschen ohne Wirkung: Den Datensatz gibt es nicht (mehr).
            logger.LogWarning("{Action} ohne Wirkung. Benutzer: {UserId}, Id: {EntityId}", Action, currentUser.UserId, EntityId(request, response));

            return;
        }

        logger.LogInformation("{Action} erfolgreich. Benutzer: {UserId}, Id: {EntityId}", Action, currentUser.UserId, EntityId(request, response));
    }

    private void LogRejected(TRequest request, ValidationException exception)
    {
        if (request is LoginCommand login)
        {
            logger.LogWarning("Anmeldung fehlgeschlagen. E-Mail: {Email}", login.Email);

            return;
        }

        // Nur Feldnamen und Fehlercodes, nicht die Meldungen: Sie können eingegebene Werte enthalten.
        string errors = string.Join(", ", exception.Errors.Select(e => $"{e.PropertyName}:{e.ErrorCode}").Distinct());

        logger.LogWarning("{Action} abgelehnt. Benutzer: {UserId}, Id: {EntityId}, Fehler: {Errors}",
            Action, currentUser.UserId, EntityId(request, default), errors);
    }

    // Die Id des betroffenen Datensatzes: bei "Anlegen" die Antwort, sonst die Id aus der Anfrage.
    private static Guid? EntityId(TRequest request, TResponse? response) =>
        response is Guid created ? created : IdProperty?.GetValue(request) as Guid?;

    // "CreateVehicleCommand" wird zu "CreateVehicle", "GetAllUsersQuery" zu "GetAllUsers".
    private static string ActionName(string requestName) =>
        requestName.EndsWith("Command", StringComparison.Ordinal) ? requestName[..^"Command".Length]
        : requestName.EndsWith("Query", StringComparison.Ordinal) ? requestName[..^"Query".Length]
        : requestName;
}
