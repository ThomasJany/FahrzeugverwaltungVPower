using System.Text;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Auth;

/// <summary>
/// Befehl zum Anmelden mit E-Mail und Passwort. Das Ergebnis ist <see langword="null"/>, wenn die Anmeldung fehlschlägt.
/// </summary>
/// <param name="Email">E-Mail-Adresse des Benutzers.</param>
/// <param name="Password">Passwort des Benutzers.</param>
public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult?>
{
    // Records geben in ToString() alle Eigenschaften aus.
    // Mit dieser Methode wird das Passwort nicht in Logs oder Fehlermeldungen ausgegeben.
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"Email = {Email}");
        return true;
    }
}