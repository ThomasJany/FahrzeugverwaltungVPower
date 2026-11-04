using System.Text;
using MediatR;

namespace FahrzeugverwaltungVPower.Core.Commands.Users;

/// <summary>
/// Befehl zum Ändern eines Benutzers. Das Ergebnis ist <see langword="false"/>, wenn es den Benutzer nicht gibt.
/// </summary>
public sealed record UpdateUserCommand : UserCommandBase, IRequest<bool>
{
    /// <summary>Id des zu ändernden Benutzers, wird aus der Route übernommen.</summary>
    public Guid Id { get; init; }

    /// <summary>Neues Passwort, <see langword="null"/> lässt das Passwort unverändert.</summary>
    public string? NewPassword { get; init; }

    // Verhindert, dass das Passwort in ToString() und damit in Logs oder Fehlermeldungen landet.
    protected override bool PrintMembers(StringBuilder builder)
    {
        base.PrintMembers(builder);
        builder.Append($", Id = {Id}, NewPassword = {(NewPassword is null ? "(unverändert)" : "***")}");
        return true;
    }
}