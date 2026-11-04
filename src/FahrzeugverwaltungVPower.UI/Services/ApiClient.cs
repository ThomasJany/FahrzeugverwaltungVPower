using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Sendet Anfragen an die API: hängt das Zugriffstoken an und übersetzt Fehlerantworten in <see cref="ApiError"/>.
/// </summary>
/// <remarks>
/// Das Token wird hier an jede Anfrage gehängt und nicht in einem HttpClient-Handler. Ein solcher Handler läuft
/// in Blazor Server in einem eigenen Dienstbereich und käme nicht an die Anmeldung des Benutzers heran.
/// </remarks>
/// <param name="http">Der HttpClient mit der Adresse der API.</param>
/// <param name="session">Die Anmeldesitzung des Benutzers.</param>
public class ApiClient(HttpClient http, AuthSession session)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Sendet eine Anfrage und liest bei Erfolg den Rückgabewert.
    /// </summary>
    /// <typeparam name="T">Typ des Rückgabewerts.</typeparam>
    /// <param name="method">HTTP-Methode.</param>
    /// <param name="url">Adresse relativ zur Adresse der API.</param>
    /// <param name="body">Optionaler Inhalt, wird als JSON gesendet.</param>
    /// <param name="authenticated">Ob das Zugriffstoken mitgesendet wird, beim Login nicht.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method, string url, object? body = null, bool authenticated = true, CancellationToken cancellationToken = default)
    {
        (HttpResponseMessage? response, ApiResult? failure) = await ExecuteAsync(method, url, body, authenticated, cancellationToken);

        if (response is null)
        {
            return ApiResult<T>.FromFailure(failure!);
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return ApiResult<T>.FromFailure(await ReadFailureAsync(response, authenticated, cancellationToken));
            }

            T? value = response.StatusCode == HttpStatusCode.NoContent
                ? default
                : await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);

            return ApiResult<T>.WithValue(value, (int)response.StatusCode);
        }
    }

    /// <summary>
    /// Sendet eine Anfrage, deren Antwort keinen Inhalt hat.
    /// </summary>
    /// <param name="method">HTTP-Methode.</param>
    /// <param name="url">Adresse relativ zur Adresse der API.</param>
    /// <param name="body">Optionaler Inhalt, wird als JSON gesendet.</param>
    /// <param name="authenticated">Ob das Zugriffstoken mitgesendet wird.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public async Task<ApiResult> SendAsync(
        HttpMethod method, string url, object? body = null, bool authenticated = true, CancellationToken cancellationToken = default)
    {
        (HttpResponseMessage? response, ApiResult? failure) = await ExecuteAsync(method, url, body, authenticated, cancellationToken);

        if (response is null)
        {
            return failure!;
        }

        using (response)
        {
            return response.IsSuccessStatusCode
                ? ApiResult.Ok((int)response.StatusCode)
                : await ReadFailureAsync(response, authenticated, cancellationToken);
        }
    }

    private async Task<(HttpResponseMessage? Response, ApiResult? Failure)> ExecuteAsync(
        HttpMethod method, string url, object? body, bool authenticated, CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(method, url);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        if (authenticated)
        {
            StoredSession? current = await session.GetAsync();

            if (current is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", current.Token);
            }
        }

        try
        {
            return (await http.SendAsync(request, cancellationToken), null);
        }
        catch (HttpRequestException)
        {
            return (null, ApiResult.Failure(0, [new ApiError("", "Die API ist nicht erreichbar. Bitte prüfen, ob sie läuft und ob die Adresse stimmt.")]));
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return (null, ApiResult.Failure(0, [new ApiError("", "Die API hat nicht rechtzeitig geantwortet.")]));
        }
    }

    private async Task<ApiResult> ReadFailureAsync(HttpResponseMessage response, bool authenticated, CancellationToken cancellationToken)
    {
        int status = (int)response.StatusCode;

        // 401 bei einem angemeldeten Benutzer heißt: Das Token ist abgelaufen oder ungültig. Die Anmeldung wird
        // gelöscht, dadurch leitet die Oberfläche selbst zur Anmeldeseite um.
        if (status == (int)HttpStatusCode.Unauthorized && authenticated)
        {
            await session.SignOutAsync();

            return ApiResult.Failure(status, [new ApiError("", "Die Anmeldung ist abgelaufen. Bitte melde dich erneut an.")]);
        }

        List<ApiError> errors = await ParseErrorsAsync(response, cancellationToken);

        if (errors.Count == 0)
        {
            errors.Add(new ApiError("", DefaultMessage(status)));
        }

        return ApiResult.Failure(status, errors);
    }

    /// <summary>
    /// Liest die Fehler aus einer ProblemDetails-Antwort: Feldfehler aus "errors", sonst "detail" oder "title".
    /// </summary>
    /// <param name="response">Die fehlgeschlagene Antwort.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    private static async Task<List<ApiError>> ParseErrorsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        List<ApiError> errors = [];

        try
        {
            string content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(content))
            {
                return errors;
            }

            using JsonDocument document = JsonDocument.Parse(content);
            JsonElement root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return errors;
            }

            if (root.TryGetProperty("errors", out JsonElement fieldErrors) && fieldErrors.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty field in fieldErrors.EnumerateObject())
                {
                    foreach (JsonElement message in field.Value.EnumerateArray())
                    {
                        errors.Add(new ApiError(field.Name, message.GetString() ?? string.Empty));
                    }
                }
            }
            else if (root.TryGetProperty("detail", out JsonElement detail) && detail.ValueKind == JsonValueKind.String)
            {
                errors.Add(new ApiError("", detail.GetString() ?? string.Empty));
            }
            else if (root.TryGetProperty("title", out JsonElement title) && title.ValueKind == JsonValueKind.String)
            {
                errors.Add(new ApiError("", title.GetString() ?? string.Empty));
            }
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            errors.Clear();
        }

        return errors;
    }

    private static string DefaultMessage(int status) => status switch
    {
        (int)HttpStatusCode.Forbidden => "Dafür fehlt die Berechtigung.",
        (int)HttpStatusCode.NotFound => "Der Eintrag wurde nicht gefunden.",
        >= 500 => "Die API hat einen Fehler gemeldet. Bitte später erneut versuchen.",
        _ => $"Die Anfrage wurde abgelehnt (Status {status})."
    };
}
