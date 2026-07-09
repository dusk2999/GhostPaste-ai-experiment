using System.Windows;
using System.Windows.Media;
using GhostPaste.Helpers;

namespace GhostPaste.Tests.Helpers;

[TestClass]
public sealed class BrushResourceUpdaterTests
{
    [TestMethod]
    public void SetBrushColorReplacesFrozenBrush()
    {
        var frozenBrush = new SolidColorBrush(Colors.White);
        frozenBrush.Freeze();
        var resources = new ResourceDictionary
        {
            ["PanelSurfaceBrush"] = frozenBrush
        };

        BrushResourceUpdater.SetBrushColor(
            resources,
            "PanelSurfaceBrush",
            Color.FromArgb(0x40, 0x10, 0x20, 0x30));

        var replacement = (SolidColorBrush)resources["PanelSurfaceBrush"];
        Assert.AreNotSame(frozenBrush, replacement);
        Assert.AreEqual(Color.FromArgb(0x40, 0x10, 0x20, 0x30), replacement.Color);
    }

    [TestMethod]
    public void SetBrushColorMutatesWritableBrush()
    {
        var brush = new SolidColorBrush(Colors.White);
        var resources = new ResourceDictionary
        {
            ["ControlSurfaceBrush"] = brush
        };

        BrushResourceUpdater.SetBrushColor(
            resources,
            "ControlSurfaceBrush",
            Color.FromArgb(0x20, 0xAA, 0xBB, 0xCC));

        Assert.AreSame(brush, resources["ControlSurfaceBrush"]);
        Assert.AreEqual(Color.FromArgb(0x20, 0xAA, 0xBB, 0xCC), brush.Color);
    }
}
