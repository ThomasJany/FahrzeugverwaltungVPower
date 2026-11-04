using FahrzeugverwaltungVPower.UI.Models;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Zugriff auf die Benutzerverwaltung der API.
/// </summary>
/// <param name="api">Der Client für die Aufrufe an die API.</param>
public class UserService(ApiClient api)
{
    /// <summary>
    /// Liest alle Benutzer.
    /// </summary>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult<List<UserModel>>> GetAllAsync(CancellationToken cancellationToken = default) =>
        api.SendAsync<List<UserModel>>(HttpMethod.Get, "api/users/GetAllUsers", cancellationToken: cancellationToken);

    /// <summary>
    /// Liest einen Benutzer. Der Statuscode 404 bedeutet, dass es ihn nicht (mehr) gibt.
    /// </summary>
    /// <param name="id">Id des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult<UserModel>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        api.SendAsync<UserModel>(HttpMethod.Get, $"api/users/GetUserById/{id}", cancellationToken: cancellationToken);

    /// <summary>
    /// Legt einen Benutzer an.
    /// </summary>
    /// <param name="form">Die Eingaben aus dem Formular.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> CreateAsync(UserForm form, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Post, "api/users/CreateUser", new
        {
            email = form.Email.Trim(),
            firstName = form.FirstName.Trim(),
            lastName = form.LastName.Trim(),
            roles = form.Roles.Order().ToArray(),
            password = form.Password
        }, cancellationToken: cancellationToken);

    /// <summary>
    /// Ändert einen Benutzer. Ein leeres Passwort lässt das bisherige Passwort unverändert.
    /// </summary>
    /// <param name="id">Id des Benutzers.</param>
    /// <param name="form">Die Eingaben aus dem Formular.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> UpdateAsync(Guid id, UserForm form, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Put, $"api/users/UpdateUser/{id}", new
        {
            email = form.Email.Trim(),
            firstName = form.FirstName.Trim(),
            lastName = form.LastName.Trim(),
            roles = form.Roles.Order().ToArray(),
            newPassword = string.IsNullOrEmpty(form.Password) ? null : form.Password
        }, cancellationToken: cancellationToken);

    /// <summary>
    /// Löscht einen Benutzer.
    /// </summary>
    /// <param name="id">Id des Benutzers.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Delete, $"api/users/DeleteUser/{id}", cancellationToken: cancellationToken);
}
