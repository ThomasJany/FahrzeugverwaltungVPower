using FahrzeugverwaltungVPower.Core.Abstractions;
using FahrzeugverwaltungVPower.Core.Entities;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FahrzeugverwaltungVPower.Test.Helpers;

/// <summary>
/// Stellt pro Test eine isolierte In-Memory-Datenbank bereit.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    public TestDatabase()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Db = new ApplicationDbContext(options);
        Context = CreateContext(Db);
    }

    /// <summary>
    /// Der echte Kontext, zum Vorbefüllen und zum Prüfen des Ergebnisses.
    /// </summary>
    public ApplicationDbContext Db { get; }

    /// <summary>
    /// Der gemockte Kontext, der an die Handler und Validatoren übergeben wird.
    /// </summary>
    public IApplicationDbContext Context { get; }

    /// <summary>
    /// Legt Fahrzeuge als Ausgangsdaten für einen Test an.
    /// </summary>
    public async Task AddAsync(params Vehicle[] vehicles)
    {
        Db.Vehicles.AddRange(vehicles);
        await Db.SaveChangesAsync();
    }

    /// <summary>
    /// Erzeugt einen Mock, dessen <see cref="IApplicationDbContext.Vehicles"/> und <see cref="IApplicationDbContext.SaveChangesAsync(CancellationToken)"/> an <paramref name="db"/> weiterleiten.
    /// </summary>
    public static IApplicationDbContext CreateContext(ApplicationDbContext db)
    {
        IApplicationDbContext context = Substitute.For<IApplicationDbContext>();

        context.Vehicles.Returns(db.Vehicles);
        context.SaveChangesAsync(Arg.Any<CancellationToken>())
               .Returns(call => db.SaveChangesAsync(call.Arg<CancellationToken>()));

        return context;
    }

    public void Dispose() => Db.Dispose();
}
