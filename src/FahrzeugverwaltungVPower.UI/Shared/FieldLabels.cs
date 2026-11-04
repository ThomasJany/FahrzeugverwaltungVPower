namespace FahrzeugverwaltungVPower.UI.Shared;

/// <summary>
/// Übersetzt Feldnamen aus Fehlermeldungen der API in die Beschriftungen der Oberfläche.
/// </summary>
public static class FieldLabels
{
    private static readonly Dictionary<string, string> Labels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Email"] = "E-Mail",
        ["Password"] = "Passwort"
    };

    /// <summary>
    /// Liefert die Beschriftung zu einem Feldnamen der API, oder einen leeren Text, wenn keine bekannt ist.
    /// </summary>
    /// <param name="field">Feldname laut API, zum Beispiel "VIN" oder "Roles[1]".</param>
    public static string For(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return string.Empty;
        }

        // Bei Listen steht die Position im Namen, zum Beispiel "Roles[1]".
        string name = field.Split('[')[0].TrimStart('$', '.');

        return Labels.TryGetValue(name, out string? label) ? label : string.Empty;
    }
}
