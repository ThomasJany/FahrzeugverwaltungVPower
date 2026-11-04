using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Sinks.MSSqlServer;

namespace FahrzeugverwaltungVPower.API;

/// <summary>
/// Einrichtung von Serilog: Konsole und Datenbanktabelle <c>Logs</c>.
/// </summary>
public static class LoggingSetup
{
    /// <summary>
    /// Erzeugt das Log für den Start der Anwendung. Es schreibt zunächst nur in die Konsole, denn die Datenbank
    /// (und damit die Tabelle <c>Logs</c>) gibt es erst nach den Migrationen.
    /// </summary>
    public static ReloadableLogger CreateBootstrapLogger() => Configure(new LoggerConfiguration()).CreateBootstrapLogger();

    /// <summary>
    /// Schaltet nach den Migrationen zusätzlich die Datenbank als Ziel ein. Die Tabelle <c>Logs</c> wird beim ersten
    /// Mal angelegt. Scheitert das, läuft die Anwendung mit der Konsole weiter und meldet es als Warnung.
    /// </summary>
    /// <param name="logger">Das Log vom Start, dessen Konfiguration ausgetauscht wird.</param>
    /// <param name="connectionString">Connection String der Datenbank, in der auch die Anwendung arbeitet.</param>
    public static void EnableDatabase(ReloadableLogger logger, string connectionString)
    {
        try
        {
            ColumnOptions columnOptions = new();

            // Statt der Eigenschaften als XML steht das ganze Ereignis als JSON in der Tabelle.
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);

            logger.Reload(configuration => Configure(configuration).WriteTo.MSSqlServer(
                connectionString,
                new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true },
                columnOptions: columnOptions));

            Log.Information("Logging in die Datenbank ist aktiv (Tabelle Logs).");
        }
        catch (Exception exception)
        {
            Log.Warning("Logging in die Datenbank konnte nicht eingerichtet werden, es wird nur in die Konsole geschrieben. Grund: {Reason}",
                exception.Message);
        }
    }

    private static LoggerConfiguration Configure(LoggerConfiguration configuration) => configuration
        .MinimumLevel.Information()
        // Das Framework meldet nur Warnungen und Fehler. Die Anwendung selbst schreibt ab Information.
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        // Unerwartete Exceptions schreibt die Pipeline (LoggingBehavior) mit Benutzer und Aktion ins Log.
        // Ohne diese Zeile stünde jede Exception ein zweites Mal vom Framework im Log.
        .MinimumLevel.Override("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogEventLevel.Fatal)
        .Enrich.FromLogContext()
        .WriteTo.Console();
}
