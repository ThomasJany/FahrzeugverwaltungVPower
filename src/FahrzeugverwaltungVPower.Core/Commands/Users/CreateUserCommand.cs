using System.Text;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Users;

/// <summary>
/// Befehl zum Anlegen eines Benutzers. Das Ergebnis ist die Id des neuen Benutzers.
/// </summary>
public sealed record CreateUserCommand : UserCommandBase, IRequest<Guid>
{
    /// <summary>Startpasswort, muss die Passwortregeln erfüllen.</summary>
    public required string Password { get; init; }

    // Verhindert, dass das Passwort in ToString() und damit in Logs oder Fehlermeldungen landet.
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append(", Password = ***");
        return true;
    }
}