using FahrzeugverwaltungVPower.UI.Services;
using Microsoft.Extensions.Logging;

namespace FahrzeugverwaltungVPower.MauiBlazor;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		// Dienste der Oberfläche aus der Razor-Klassenbibliothek. Wo die Anmeldung gespeichert wird, bestimmt der Host:
		// hier der sichere Speicher des Geräts.
		builder.Services.AddFahrzeugverwaltungUi(options => options.ApiBaseUrl = ApiEndpoints.BaseUrl);
		builder.Services.AddSingleton<ITokenStore, SecureStorageTokenStore>();

		// Abstände der Systemleisten für Edge-to-Edge (unter Android von der MainActivity befüllt).
		builder.Services.AddSingleton<SafeAreaService>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
