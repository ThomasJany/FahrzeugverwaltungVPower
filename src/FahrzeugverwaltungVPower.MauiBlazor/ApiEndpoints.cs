namespace FahrzeugverwaltungVPower.MauiBlazor;

/// <summary>
/// Die Adresse der API für die App. Sie hängt davon ab, wo die App läuft, weil "localhost" auf einem
/// Emulator oder Gerät nicht der Rechner mit der API ist.
/// </summary>
/// <remarks>
/// Für die Entwicklung ist die API per HTTP erreichbar (Port 5112, Profil "http" der API). HTTPS mit dem
/// Entwicklerzertifikat scheitert auf Emulatoren und Geräten, weil sie diesem Zertifikat nicht vertrauen.
/// Wie die API von einem echten Gerät aus erreichbar wird, steht im README.
/// </remarks>
public static class ApiEndpoints
{
    private const int Port = 5112;

    /// <summary>
    /// Adresse der API für ein echtes Android- oder iOS-Gerät, zum Beispiel <c>http://192.168.0.20:5112/</c>
    /// (IP-Adresse des Rechners im selben WLAN). Leer lassen, wenn die App nur im Emulator oder unter Windows läuft.
    /// </summary>
    public const string PhysicalDeviceUrl = "http://192.168.178.48:5112/";

    /// <summary>
    /// Die Basisadresse der API für die Plattform, auf der die App gerade läuft.
    /// </summary>
    public static string BaseUrl
    {
        get
        {
            bool mobile = DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS;

            if (mobile && DeviceInfo.DeviceType == DeviceType.Physical && PhysicalDeviceUrl.Length > 0)
            {
                return PhysicalDeviceUrl;
            }

            // Der Android-Emulator erreicht den Rechner, auf dem er läuft, über 10.0.2.2.
            // Windows, Mac und der iOS-Simulator teilen sich das Netzwerk mit dem Rechner.
            return DeviceInfo.Platform == DevicePlatform.Android
                ? $"http://10.0.2.2:{Port}/"
                : $"http://localhost:{Port}/";
        }
    }
}
