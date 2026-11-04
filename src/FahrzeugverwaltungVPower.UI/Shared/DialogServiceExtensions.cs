using MudBlazor;

namespace FahrzeugverwaltungVPower.UI.Shared;

/// <summary>
/// Gemeinsame Dialoge der Oberfläche.
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Fragt in einem Modal nach, ob der Benutzer wirklich gelöscht werden soll. Gibt <see langword="true"/> zurück,
    /// wenn der Löschen-Knopf gedrückt wurde. Ein Klick neben den Dialog schließt ihn nicht.
    /// </summary>
    /// <param name="dialogs">Der Dialogdienst.</param>
    /// <param name="what">Beschreibung dessen, was gelöscht wird, zum Beispiel der Name des Benutzers.</param>
    public static async Task<bool> ConfirmDeleteAsync(this IDialogService dialogs, string what)
    {
        bool? result = await dialogs.ShowMessageBoxAsync(
            "Löschen bestätigen",
            $"{what} wirklich löschen? Das kann nicht rückgängig gemacht werden.",
            yesText: "Löschen",
            cancelText: "Abbrechen",
            options: new DialogOptions { BackdropClick = false });

        return result == true;
    }
}
