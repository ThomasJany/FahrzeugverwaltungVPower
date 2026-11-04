using FahrzeugverwaltungVPower.UI.Models;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Erzeugt Testfahrzeuge für Vorführungen. Nur für Präsentationszwecke gedacht, siehe README.
/// </summary>
public static class TestVehicleGenerator
{
    private static readonly (string Manufacturer, string Model, string Type)[] Cars =
    [
        ("Volkswagen", "Golf", "Golf VIII"),
        ("Volkswagen", "Passat", "Passat B8"),
        ("BMW", "320d", "3er G20"),
        ("Mercedes-Benz", "C 200", "C-Klasse W206"),
        ("Audi", "A4", "A4 B9"),
        ("Opel", "Corsa", "Corsa F"),
        ("Skoda", "Octavia", "Octavia IV"),
        ("Ford", "Focus", "Focus IV"),
        ("Renault", "Clio", "Clio V"),
        ("Toyota", "Yaris", "Yaris XP21")
    ];

    private static readonly string[] FirstNames =
        ["Anna", "Ben", "Clara", "David", "Elif", "Felix", "Greta", "Hannes", "Ida", "Jonas", "Klara", "Lukas"];

    private static readonly string[] LastNames =
        ["Berger", "Fischer", "Hoffmann", "Keller", "Lang", "Meyer", "Neumann", "Peters", "Richter", "Schmidt"];

    private static readonly string[] Companies =
        ["Berger Logistik GmbH", "Keller Bau GmbH", "Stadtwerke Beispielstadt", "Lang & Söhne KG"];

    private static readonly string[] CityCodes = ["M", "B", "HH", "K", "F", "S", "N", "DD", "L", "HB"];

    // Ohne I, O und Q, wie bei einer echten VIN.
    private const string VinChars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";

    private const string Letters = "ABCDEFGHJKLMNPRSTUVWXYZ";

    /// <summary>
    /// Erzeugt die Eingaben für ein neues Testfahrzeug mit zufälligen Daten. Die VIN gibt es noch nicht, die HU
    /// ist gemischt abgelaufen, bald fällig oder länger gültig, damit alle Farben der Tabelle vorkommen.
    /// </summary>
    /// <param name="existingVins">Die VINs der vorhandenen Fahrzeuge.</param>
    /// <param name="today">Das heutige Datum.</param>
    /// <param name="random">Zufallsquelle, nur für Tests austauschbar.</param>
    public static VehicleForm Create(IEnumerable<string> existingVins, DateOnly today, Random? random = null)
    {
        random ??= Random.Shared;

        HashSet<string> taken = new(existingVins, StringComparer.OrdinalIgnoreCase);
        (string manufacturer, string model, string type) = Cars[random.Next(Cars.Length)];

        string vin;

        do
        {
            vin = new string(Enumerable.Range(0, 17).Select(_ => VinChars[random.Next(VinChars.Length)]).ToArray());
        }
        while (taken.Contains(vin));

        bool company = random.Next(5) == 0;
        DateOnly firstRegistration = today.AddDays(-random.Next(60, 365 * 15));

        return new VehicleForm
        {
            LicensePlate = $"{CityCodes[random.Next(CityCodes.Length)]} {Letters[random.Next(Letters.Length)]}{Letters[random.Next(Letters.Length)]} {random.Next(1, 10000)}",
            FirstRegistrationDate = firstRegistration.ToDateTime(TimeOnly.MinValue),
            Holder = company ? Companies[random.Next(Companies.Length)] : LastNames[random.Next(LastNames.Length)],
            HolderFirstName = company ? string.Empty : FirstNames[random.Next(FirstNames.Length)],
            Manufacturer = manufacturer,
            TypeDesignation = type,
            Model = model,
            Vin = vin,
            InspectionValidUntil = RandomInspection(today, random).ToDateTime(TimeOnly.MinValue),
            Hsn = random.Next(0, 10000).ToString("D4"),
            Tsn = new string(Enumerable.Range(0, 3).Select(_ => Letters[random.Next(Letters.Length)]).ToArray())
        };
    }

    // Jeweils ein Viertel abgelaufen und bald fällig, die Hälfte länger gültig.
    private static DateOnly RandomInspection(DateOnly today, Random random) => random.Next(4) switch
    {
        0 => today.AddDays(-random.Next(1, 400)),
        1 => today.AddDays(random.Next(1, 85)),
        _ => today.AddDays(random.Next(100, 730))
    };
}
