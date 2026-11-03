using FahrzeugverwaltungVPower.API;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Hinweis:
    // Neue Migrationen werden bei jedem Start der API-Anwendung automatisch angewendet.
    // Das ist eine bewusste Entscheidung, da dies "nur" ein Beispielprojekt ist.
    // In einer Produktionsumgebung würde ich Migrationen manuell anwenden, um die Kontrolle über den Prozess zu behalten.

    // TODO: Automatisches Anwenden von Migrationen entfernen.
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();