using FahrzeugverwaltungVPower.Blazor.Components;
using FahrzeugverwaltungVPower.Blazor.Services;
using FahrzeugverwaltungVPower.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Dienste der Oberfläche (Anmeldung, Zugriff auf die API). Wo die Anmeldung gespeichert wird, bestimmt der Host.
builder.Services.AddFahrzeugverwaltungUi(options =>
    options.ApiBaseUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException("Api:BaseUrl ist nicht konfiguriert."));
builder.Services.AddScoped<ITokenStore, SessionStorageTokenStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

// Alle Seiten liegen in der Razor-Klassenbibliothek (FahrzeugverwaltungVPower.UI) und müssen hier bekannt gemacht werden.
//
// AllowAnonymous gilt nur für den HTTP-Aufruf der Seitenhülle: Das Token liegt im sessionStorage des Browsers und
// wird bei Seitenaufrufen nicht mitgeschickt, die ASP.NET-Autorisierung könnte hier also nie "angemeldet" sehen.
// Geschützt wird trotzdem: Die Seiten tragen [Authorize] und werden im Blazor-Kreislauf von AuthorizeRouteView
// geprüft, und die API prüft jeden Aufruf selbst.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(FahrzeugverwaltungVPower.UI.Routes).Assembly)
    .AllowAnonymous();

app.Run();
