namespace FahrzeugverwaltungVPower.MauiBlazor;

/// <summary>
/// Die Abstände, die Statusleiste, Navigationsleiste, Aussparungen der Kamera und die Bildschirmtastatur vom Inhalt
/// belegen, in CSS-Pixeln (geräteunabhängige Einheiten).
/// </summary>
/// <remarks>
/// Android liefert die Werte (<c>MainActivity</c>), die Oberfläche setzt sie über <c>SafeAreaBridge</c> als
/// CSS-Variablen. Unter iOS und Windows bleiben sie 0, dort genügt <c>env(safe-area-inset-*)</c> im CSS.
/// </remarks>
public class SafeAreaService
{
    /// <summary>Abstand oben.</summary>
    public double Top { get; private set; }

    /// <summary>Abstand rechts.</summary>
    public double Right { get; private set; }

    /// <summary>Abstand unten.</summary>
    public double Bottom { get; private set; }

    /// <summary>Abstand links.</summary>
    public double Left { get; private set; }

    /// <summary>Wird ausgelöst, wenn sich ein Abstand geändert hat, zum Beispiel beim Drehen oder beim Einblenden der Tastatur.</summary>
    public event Action? Changed;

    /// <summary>
    /// Übernimmt neue Abstände und meldet eine Änderung.
    /// </summary>
    /// <param name="top">Abstand oben.</param>
    /// <param name="right">Abstand rechts.</param>
    /// <param name="bottom">Abstand unten.</param>
    /// <param name="left">Abstand links.</param>
    public void Update(double top, double right, double bottom, double left)
    {
        if (top == Top && right == Right && bottom == Bottom && left == Left)
        {
            return;
        }

        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;

        Changed?.Invoke();
    }
}
