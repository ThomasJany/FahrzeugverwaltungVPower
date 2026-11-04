using FahrzeugverwaltungVPower.Core.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FahrzeugverwaltungVPower.API;

/// <summary>
/// Wandelt eine <see cref="ValidationException"/> in eine 400-Antwort mit den Fehlern je Feld um.
/// Fehlt nur die angefragte Ressource, lautet die Antwort 404.
/// </summary>
/// <param name="problemDetailsService">Schreibt die Fehlerantwort.</param>
public class ValidationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    /// <summary>
    /// Behandelt <see cref="ValidationException"/>, bei jeder anderen Exception wird <see langword="false"/> zurückgegeben.
    /// </summary>
    /// <param name="httpContext">Der aktuelle HTTP-Kontext.</param>
    /// <param name="exception">Die aufgetretene Exception.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        bool onlyNotFound = validationException.Errors.Any()
            && validationException.Errors.All(e => e.ErrorCode == CustomErrorCodes.NotFound);
        int status = onlyNotFound ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest;

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ValidationProblemDetails(errors) { Status = status }
        });
    }
}