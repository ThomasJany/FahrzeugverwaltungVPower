using FahrzeugverwaltungVPower.API;
using FahrzeugverwaltungVPower.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Token aus POST /api/auth/Login einfügen."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Migrationen werden bei jedem Start der API automatisch angewendet.
    // Das ist nur für dieses Übungsprojekt so gewählt: Wer die API startet, bekommt ohne weiteren Schritt
    // eine fertige Datenbank, auch der Projektleiter auf einem neuen Rechner.
    //  
    // In Produktion ist das NICHT zulässig:
    //  - Mehrere API-Instanzen würden sich gegenseitig migrieren.
    //  - Das Datenbankkonto der API bräuchte Rechte zum Ändern des Schemas.
    //  - Eine fehlgeschlagene Migration würde den Start der Anwendung blockieren.
    //
    // Stattdessen: Migration Bundle oder SQL-Skript in der Deployment-Pipeline ausführen vor dem Start.
    // 
    // TODO: Automatisches Anwenden von Migrationen entfernen.
    await dbContext.Database.MigrateAsync();
    
    // Legt beim ersten Start den Administrator an.
    await scope.ServiceProvider.GetRequiredService<AdminSeeder>().SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();