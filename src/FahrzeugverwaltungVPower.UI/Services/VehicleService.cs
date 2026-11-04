using FahrzeugverwaltungVPower.UI.Models;

namespace FahrzeugverwaltungVPower.UI.Services;

/// <summary>
/// Zugriff auf die Fahrzeugverwaltung der API.
/// </summary>
/// <param name="api">Der Client für die Aufrufe an die API.</param>
public class VehicleService(ApiClient api)
{
    /// <summary>
    /// Liest alle Fahrzeuge.
    /// </summary>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult<List<VehicleModel>>> GetAllAsync(CancellationToken cancellationToken = default) =>
        api.SendAsync<List<VehicleModel>>(HttpMethod.Get, "api/vehicles/GetAllVehicles", cancellationToken: cancellationToken);

    /// <summary>
    /// Liest ein Fahrzeug. Der Statuscode 404 bedeutet, dass es das Fahrzeug nicht (mehr) gibt.
    /// </summary>
    /// <param name="id">Id des Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult<VehicleModel>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        api.SendAsync<VehicleModel>(HttpMethod.Get, $"api/vehicles/GetVehicleById/{id}", cancellationToken: cancellationToken);

    /// <summary>
    /// Legt ein Fahrzeug an.
    /// </summary>
    /// <param name="form">Die Eingaben aus dem Formular.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> CreateAsync(VehicleForm form, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Post, "api/vehicles/CreateVehicle", ToBody(form), cancellationToken: cancellationToken);

    /// <summary>
    /// Ändert ein Fahrzeug.
    /// </summary>
    /// <param name="id">Id des Fahrzeugs.</param>
    /// <param name="form">Die Eingaben aus dem Formular.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> UpdateAsync(Guid id, VehicleForm form, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Put, $"api/vehicles/UpdateVehicle/{id}", ToBody(form), cancellationToken: cancellationToken);

    /// <summary>
    /// Löscht ein Fahrzeug.
    /// </summary>
    /// <param name="id">Id des Fahrzeugs.</param>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    public Task<ApiResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        api.SendAsync(HttpMethod.Delete, $"api/vehicles/DeleteVehicleById/{id}", cancellationToken: cancellationToken);

    private static object ToBody(VehicleForm form) => new
    {
        licensePlate = form.LicensePlate.Trim(),
        firstRegistrationDate = DateOnly.FromDateTime(form.FirstRegistrationDate ?? default),
        holder = form.Holder.Trim(),
        holderFirstName = string.IsNullOrWhiteSpace(form.HolderFirstName) ? null : form.HolderFirstName.Trim(),
        manufacturer = form.Manufacturer.Trim(),
        typeDesignation = form.TypeDesignation.Trim(),
        model = form.Model.Trim(),
        vin = form.Vin.Trim(),
        inspectionValidUntil = DateOnly.FromDateTime(form.InspectionValidUntil ?? default),
        hsn = form.Hsn.Trim(),
        tsn = form.Tsn.Trim()
    };
}
