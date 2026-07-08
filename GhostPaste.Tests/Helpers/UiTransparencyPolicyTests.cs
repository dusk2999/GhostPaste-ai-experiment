using GhostPaste.Helpers;

namespace GhostPaste.Tests.Helpers;

[TestClass]
public sealed class UiTransparencyPolicyTests
{
    [TestMethod]
    public void FullyTransparentValueDisablesAcrylicAndKeepsHitTestSurface()
    {
        var style = UiTransparencyPolicy.FromSliderValue(0);

        Assert.IsFalse(style.AcrylicEnabled);
        Assert.AreEqual(1, style.ChromeAlpha);
        Assert.AreEqual(0, style.PanelAlpha);
        Assert.AreEqual(0, style.AnswerAlpha);
        Assert.AreEqual(0, style.ControlAlpha);
        Assert.AreEqual(0, style.ControlHoverAlpha);
        Assert.AreEqual(0, style.PrimaryButtonAlpha);
        Assert.AreEqual(0, style.PrimaryButtonHoverAlpha);
        Assert.AreEqual(0, style.PrimaryButtonPressedAlpha);
    }

    [TestMethod]
    public void ClearValueEnablesAcrylicAndOriginalBackgrounds()
    {
        var style = UiTransparencyPolicy.FromSliderValue(100);

        Assert.IsTrue(style.AcrylicEnabled);
        Assert.AreEqual(0x40, style.ChromeAlpha);
        Assert.AreEqual(0x60, style.PanelAlpha);
        Assert.AreEqual(0x50, style.AnswerAlpha);
        Assert.AreEqual(0x50, style.ControlAlpha);
        Assert.AreEqual(0x70, style.ControlHoverAlpha);
        Assert.AreEqual(0xCC, style.PrimaryButtonAlpha);
        Assert.AreEqual(0xDD, style.PrimaryButtonHoverAlpha);
        Assert.AreEqual(0xAA, style.PrimaryButtonPressedAlpha);
    }

    [TestMethod]
    public void SliderValueIsClamped()
    {
        Assert.AreEqual(1, UiTransparencyPolicy.FromSliderValue(-20).ChromeAlpha);
        Assert.AreEqual(0x40, UiTransparencyPolicy.FromSliderValue(150).ChromeAlpha);
        Assert.AreEqual(0, UiTransparencyPolicy.FromSliderValue(-20).ControlAlpha);
        Assert.AreEqual(0xCC, UiTransparencyPolicy.FromSliderValue(150).PrimaryButtonAlpha);
    }
}
