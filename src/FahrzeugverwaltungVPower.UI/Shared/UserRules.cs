using System.ComponentModel.DataAnnotations;

namespace FahrzeugverwaltungVPower.UI.Shared;

/// <summary>
/// Die Prüfregeln für die Felder des Benutzerformulars. Jede Regel liefert eine Fehlermeldung oder
/// <see langword="null"/>, wenn die Eingabe in Ordnung ist.
/// </summary>
/// <remarks>
/// Die Regeln spiegeln die Prüfungen der API (Validatoren in Core, Passwortregeln in Identity), damit Fehler
/// schon bei der Eingabe auffallen. Die API prüft trotzdem jeden Aufruf selbst.
/// </remarks>
public static class UserRules
{
    /// <summary>Mindestlänge des Passworts, entspricht den Identity-Optionen der API.</summary>
    public const int PasswordMinLength = 12;

    /// <summary>Höchstlänge des Passworts.</summary>
    public const int PasswordMaxLength = 128;

    /// <summary>Erklärung der Passwortregeln für die Anzeige.</summary>
    public const string PasswordHint = "Mindestens 12 Zeichen mit Groß- und Kleinbuchstaben, einer Ziffer und einem Sonderzeichen.";

    /// <summary>
    /// Prüft einen Namen (Vor- oder Nachname).
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    /// <param name="label">Bezeichnung des Feldes für die Meldung.</param>
    public static string? Name(string? value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"{label} ist erforderlich.";
        }

        return value.Trim().Length > 100 ? $"{label} darf höchstens 100 Zeichen lang sein." : null;
    }

    /// <summary>
    /// Prüft die E-Mail-Adresse.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    public static string? Email(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Die E-Mail-Adresse ist erforderlich.";
        }

        string email = value.Trim();

        if (email.Length > 256)
        {
            return "Die E-Mail-Adresse darf höchstens 256 Zeichen lang sein.";
        }

        // EmailAddressAttribute prüft nur grob (ein @ in der Mitte), "name@host" oder Leerzeichen ließe es durch.
        // Deshalb zusätzlich: keine Leerzeichen und eine Domain mit Punkt und mindestens zwei Zeichen am Ende.
        if (email.Any(char.IsWhiteSpace) || !new EmailAddressAttribute().IsValid(email))
        {
            return InvalidEmail;
        }

        string domain = email[(email.LastIndexOf('@') + 1)..];
        int lastDot = domain.LastIndexOf('.');

        return lastDot < 1 || domain.Length - lastDot - 1 < 2 ? InvalidEmail : null;
    }

    private const string InvalidEmail = "Das ist keine gültige E-Mail-Adresse, zum Beispiel name@beispiel.de.";

    /// <summary>
    /// Prüft das Passwort.
    /// </summary>
    /// <param name="value">Die Eingabe.</param>
    /// <param name="required">Beim Anlegen ist das Passwort Pflicht, beim Bearbeiten darf es leer bleiben.</param>
    public static string? Password(string? value, bool required)
    {
        if (string.IsNullOrEmpty(value))
        {
            return required ? "Das Passwort ist erforderlich." : null;
        }

        if (value.Length < PasswordMinLength)
        {
            return $"Das Passwort muss mindestens {PasswordMinLength} Zeichen lang sein.";
        }

        if (value.Length > PasswordMaxLength)
        {
            return $"Das Passwort darf höchstens {PasswordMaxLength} Zeichen lang sein.";
        }

        if (!value.Any(char.IsUpper))
        {
            return "Das Passwort braucht mindestens einen Großbuchstaben.";
        }

        if (!value.Any(char.IsLower))
        {
            return "Das Passwort braucht mindestens einen Kleinbuchstaben.";
        }

        if (!value.Any(char.IsDigit))
        {
            return "Das Passwort braucht mindestens eine Ziffer.";
        }

        return value.All(char.IsLetterOrDigit) ? "Das Passwort braucht mindestens ein Sonderzeichen." : null;
    }

    /// <summary>
    /// Prüft, ob die Wiederholung zum Passwort passt.
    /// </summary>
    /// <param name="confirmation">Die Wiederholung.</param>
    /// <param name="password">Das Passwort.</param>
    public static string? PasswordConfirmation(string? confirmation, string? password) =>
        (confirmation ?? string.Empty) == (password ?? string.Empty) ? null : "Die Passwörter stimmen nicht überein.";
}
