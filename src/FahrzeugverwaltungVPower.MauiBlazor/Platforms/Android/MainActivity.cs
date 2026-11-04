using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace FahrzeugverwaltungVPower.MauiBlazor;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Edge-to-Edge: Der Inhalt reicht hinter Status- und Navigationsleiste. Ab Android 15 ist das ohnehin Pflicht,
        // so verhält sich die App auch auf älteren Versionen gleich.
        WindowCompat.SetDecorFitsSystemWindows(Window!, false);

#pragma warning disable CA1422 // Die Farben werden ab Android 15 ignoriert (immer durchsichtig), davor werden sie gebraucht.
        Window!.SetStatusBarColor(Android.Graphics.Color.Transparent);
        Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);
#pragma warning restore CA1422

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            Window.NavigationBarContrastEnforced = false;
        }

        // Die Kopfzeile ist immer farbig, deshalb helle Symbole in der Statusleiste. Die Navigationsleiste liegt
        // über dem Seitenhintergrund und richtet sich nach dem Hell/Dunkel-Modus des Geräts.
        bool deviceIsDark = (Resources!.Configuration!.UiMode & Android.Content.Res.UiMode.NightMask) == Android.Content.Res.UiMode.NightYes;
        WindowInsetsControllerCompat? controller = WindowCompat.GetInsetsController(Window, Window.DecorView);

        if (controller is not null)
        {
            controller.AppearanceLightStatusBars = false;
            controller.AppearanceLightNavigationBars = !deviceIsDark;
        }

        ViewCompat.SetOnApplyWindowInsetsListener(Window.DecorView, new InsetsListener(Resources.DisplayMetrics!.Density));
    }

    /// <summary>
    /// Meldet die Abstände der Systemleisten und der Tastatur an die Oberfläche (SafeAreaService).
    /// </summary>
    private sealed class InsetsListener(float density) : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat? OnApplyWindowInsets(Android.Views.View? v, WindowInsetsCompat? insets)
        {
            if (insets is null)
            {
                return insets;
            }

            AndroidX.Core.Graphics.Insets? bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars() | WindowInsetsCompat.Type.DisplayCutout());
            AndroidX.Core.Graphics.Insets? keyboard = insets.GetInsets(WindowInsetsCompat.Type.Ime());

            if (bars is null)
            {
                return insets;
            }

            // Die Tastatur schiebt den unteren Abstand nach oben, damit Eingabefelder nicht verdeckt werden.
            int bottom = Math.Max(bars.Bottom, keyboard?.Bottom ?? 0);

            IPlatformApplication.Current?.Services.GetService<SafeAreaService>()?
                .Update(bars.Top / density, bars.Right / density, bottom / density, bars.Left / density);

            return insets;
        }
    }
}
