using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace FahrzeugverwaltungVPower.Core.Security;

/// <summary>
/// Erweiterungsmethoden für <see cref="IdentityResult"/>.
/// </summary>
public static class IdentityResultExtensions
{
    /// <summary>
    /// Wirft eine <see cref="ValidationException"/> mit allen Fehlern, wenn das Ergebnis nicht erfolgreich war.
    /// </summary>
    /// <param name="result">Das Ergebnis eines Identity-Aufrufs.</param>
    public static void ThrowIfFailed(this IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        throw new ValidationException(result.Errors.Select(error =>
            new ValidationFailure(GetPropertyName(error.Code), error.Description) { ErrorCode = error.Code }));
    }

    /// <summary>
    /// Ordnet einen Identity-Fehlercode dem betroffenen Feld zu, unbekannte Codes bleiben ohne Feld.
    /// </summary>
    /// <param name="code">Der Fehlercode von Identity.</param>
    private static string GetPropertyName(string code) => code switch
    {
        _ when code.StartsWith("Password", StringComparison.Ordinal) => "Password",
        "DuplicateEmail" or "InvalidEmail" or "DuplicateUserName" or "InvalidUserName" => "Email",
        _ => string.Empty
    };
}