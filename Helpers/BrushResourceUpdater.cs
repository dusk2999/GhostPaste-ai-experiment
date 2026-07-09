using System.Windows;
using System.Windows.Media;

namespace GhostPaste.Helpers;

public static class BrushResourceUpdater
{
    public static void SetBrushColor(ResourceDictionary resources, string resourceKey, Color color)
    {
        if (resources[resourceKey] is SolidColorBrush brush && !brush.IsFrozen)
        {
            brush.Color = color;
            return;
        }

        resources[resourceKey] = new SolidColorBrush(color);
    }
}
