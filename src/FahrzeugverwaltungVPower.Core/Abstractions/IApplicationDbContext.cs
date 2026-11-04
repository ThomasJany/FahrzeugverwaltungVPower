using FahrzeugverwaltungVPower.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FahrzeugverwaltungVPower.Core.Abstractions;

/// <summary>
/// Abstraktion des Datenbankkontexts für Handler und Validatoren, damit Core nicht von Infrastructure abhängt.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Die gespeicherten Fahrzeuge.
    /// </summary>
    DbSet<Vehicle> Vehicles { get; }
    /// <summary>
    /// Speichert alle Änderungen und gibt die Anzahl der betroffenen Zeilen zurück.
    /// </summary>
    /// <param name="cancellationToken">Token zum Abbrechen des Vorgangs.</param>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
