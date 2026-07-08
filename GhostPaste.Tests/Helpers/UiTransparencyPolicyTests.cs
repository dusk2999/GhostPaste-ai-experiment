using GhostPaste.Helpers;

namespace GhostPaste.Tests.Helpers;

[TestClass]
public sealed class UiTransparencyPolicyTests
{
    [TestMethod]
    public void FullyTransparentValueDisablesAcrylicAndBackgrounds()
    {
        var style = UiTransparencyPolicy.FromSliderValue(0);

        Assert.IsFalse(style.AcrylicEnabled);
        Assert.AreEqual(0, style.ChromeAlpha);
        Assert.AreEqual(0, style.PanelAlpha);
        Assert.AreEqual(0, style.AnswerAlpha);
    }

    [TestMethod]
    public void ClearValueEnablesAcrylicAndOriginalBackgrounds()
    {
        var style = UiTransparencyPolicy.FromSliderValue(100);

        Assert.IsTrue(style.AcrylicEnabled);
        Assert.AreEqual(0x40, style.ChromeAlpha);
        Assert.AreEqual(0x60, style.PanelAlpha);
        Assert.AreEqual(0x50, style.AnswerAlpha);
    }

    [TestMethod]
    public void SliderValueIsClamped()
    {
        Assert.AreEqual(0, UiTransparencyPolicy.FromSliderValue(-20).ChromeAlpha);
        Assert.AreEqual(0x40, UiTransparencyPolicy.FromSliderValue(150).ChromeAlpha);
    }
}
