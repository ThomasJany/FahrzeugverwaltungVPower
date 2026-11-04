using Microsoft.Extensions.Logging;

namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Ein Logger, der alle Einträge in einer Liste sammelt, damit Tests prüfen können, was protokolliert wurde.
/// </summary>
/// <typeparam name="T">Die Kategorie des Loggers.</typeparam>
public class ListLogger<T> : ILogger<T>
{
    /// <summary>Die bisher geschriebenen Einträge.</summary>
    public List<LogEntry> Entries { get; } = [];

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));

    /// <summary>
    /// Ein Eintrag im Log.
    /// </summary>
    /// <param name="Level">Die Stufe.</param>
    /// <param name="Message">Die fertig formatierte Meldung.</param>
    /// <param name="Exception">Die Exception, falls eine mitgegeben wurde.</param>
    public record LogEntry(LogLevel Level, string Message, Exception? Exception);
}
