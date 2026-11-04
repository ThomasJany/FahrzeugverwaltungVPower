namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Ein einzelner Fehler einer API-Antwort.
/// </summary>
/// <param name="Field">Name des betroffenen Feldes laut API, leer bei einem allgemeinen Fehler.</param>
/// <param name="Message">Die Fehlermeldung.</param>
public record ApiError(string Field, string Message);

/// <summary>
/// Das Ergebnis eines API-Aufrufs ohne Rückgabewert: Erfolg oder eine Liste von Fehlern.
/// </summary>
public class ApiResult
{
    /// <summary>Gibt an, ob der Aufruf erfolgreich war.</summary>
    public bool Success { get; init; }

    /// <summary>HTTP-Statuscode der Antwort, 0 wenn die API nicht erreichbar war.</summary>
    public int StatusCode { get; init; }

    /// <summary>Die Fehler der Antwort, bei Erfolg leer.</summary>
    public IReadOnlyList<ApiError> Errors { get; init; } = [];

    /// <summary>
    /// Erzeugt ein erfolgreiches Ergebnis.
    /// </summary>
    /// <param name="statusCode">HTTP-Statuscode der Antwort.</param>
    public static ApiResult Ok(int statusCode) => new() { Success = true, StatusCode = statusCode };

    /// <summary>
    /// Erzeugt ein fehlgeschlagenes Ergebnis.
    /// </summary>
    /// <param name="statusCode">HTTP-Statuscode der Antwort, 0 wenn die API nicht erreichbar war.</param>
    /// <param name="errors">Die Fehler der Antwort.</param>
    public static ApiResult Failure(int statusCode, IReadOnlyList<ApiError> errors) =>
        new() { Success = false, StatusCode = statusCode, Errors = errors };

    /// <summary>
    /// Fasst alle Fehlermeldungen zu einem Text zusammen.
    /// </summary>
    public string ErrorText => string.Join(" ", Errors.Select(e => e.Message));
}

/// <summary>
/// Das Ergebnis eines API-Aufrufs mit Rückgabewert.
/// </summary>
/// <typeparam name="T">Typ des Rückgabewerts.</typeparam>
public sealed class ApiResult<T> : ApiResult
{
    /// <summary>Der Rückgabewert, nur bei Erfolg gesetzt.</summary>
    public T? Value { get; init; }

    /// <summary>
    /// Erzeugt ein erfolgreiches Ergebnis mit Wert.
    /// </summary>
    /// <param name="value">Der Rückgabewert.</param>
    /// <param name="statusCode">HTTP-Statuscode der Antwort.</param>
    public static ApiResult<T> WithValue(T? value, int statusCode) =>
        new() { Success = true, StatusCode = statusCode, Value = value };

    /// <summary>
    /// Überträgt ein fehlgeschlagenes Ergebnis ohne Wert auf den Ergebnistyp mit Wert.
    /// </summary>
    /// <param name="failure">Das fehlgeschlagene Ergebnis.</param>
    public static ApiResult<T> FromFailure(ApiResult failure) =>
        new() { Success = false, StatusCode = failure.StatusCode, Errors = failure.Errors };
}
