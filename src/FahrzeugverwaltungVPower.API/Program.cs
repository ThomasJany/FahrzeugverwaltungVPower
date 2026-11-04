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
        Description = "Token aus POST /api/auth/login einfügen."
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

    // Hinweis:
    // Neue Migrationen werden bei jedem Start der API-Anwendung automatisch angewendet.
    // Das ist eine bewusste Entscheidung, da dies "nur" ein Beispielprojekt ist.
    // In einer Produktionsumgebung würde ich Migrationen manuell anwenden, um die Kontrolle über den Prozess zu behalten.

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