using FahrzeugverwaltungVPower.UI.Models;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Erzeugt Testbenutzer für Vorführungen. Nur für Präsentationszwecke gedacht, siehe README.
/// </summary>
public static class TestUserGenerator
{
    /// <summary>Das Passwort aller Testbenutzer. Es erfüllt die Passwortregeln.</summary>
    public const string Password = "TestUser#12345";

    private static readonly string[] FirstNames =
        ["Anna", "Ben", "Clara", "David", "Elif", "Felix", "Greta", "Hannes", "Ida", "Jonas", "Klara", "Lukas", "Mia", "Noah"];

    private static readonly string[] LastNames =
        ["Berger", "Fischer", "Hoffmann", "Keller", "Lang", "Meyer", "Neumann", "Peters", "Richter", "Schmidt", "Vogel", "Winter"];

    /// <summary>
    /// Erzeugt die Eingaben für einen neuen Testbenutzer mit zufälligem Namen und zufälliger Rollenkombination.
    /// Der Benutzer ist nie Admin, und seine E-Mail-Adresse gibt es noch nicht.
    /// </summary>
    /// <param name="existingEmails">Die E-Mail-Adressen der vorhandenen Benutzer.</param>
    /// <param name="random">Zufallsquelle, nur für Tests austauschbar.</param>
    public static UserForm Create(IEnumerable<string> existingEmails, Random? random = null)
    {
        random ??= Random.Shared;

        HashSet<string> taken = new(existingEmails, StringComparer.OrdinalIgnoreCase);

        string firstName = FirstNames[random.Next(FirstNames.Length)];
        string lastName = LastNames[random.Next(LastNames.Length)];
        string baseName = $"{firstName}.{lastName}".ToLowerInvariant();

        string email = $"{baseName}@example.test";

        for (int number = 2; taken.Contains(email); number++)
        {
            email = $"{baseName}{number}@example.test";
        }

        return new UserForm
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = Password,
            PasswordConfirmation = Password,
            Roles = RandomRoles(random)
        };
    }

    // Eine zufällige, nicht leere Auswahl aus Creator, Editor und Viewer.
    private static HashSet<string> RandomRoles(Random random)
    {
        string[] candidates = [UiRoles.Creator, UiRoles.Editor, UiRoles.Viewer];
        HashSet<string> roles = [];

        while (roles.Count == 0)
        {
            foreach (string role in candidates)
            {
                if (random.Next(2) == 1)
                {
                    roles.Add(role);
                }
            }
        }

        return roles;
    }
}
