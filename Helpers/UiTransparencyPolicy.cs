namespace GhostPaste.Helpers;

public sealed record UiTransparencyStyle(
    bool AcrylicEnabled,
    byte ChromeAlpha,
    byte PanelAlpha,
    byte AnswerAlpha,
    byte ControlAlpha,
    byte ControlHoverAlpha,
    byte PrimaryButtonAlpha,
    byte PrimaryButtonHoverAlpha,
    byte PrimaryButtonPressedAlpha);

public static class UiTransparencyPolicy
{
    public static UiTransparencyStyle FromSliderValue(double value)
    {
        double ratio = Math.Clamp(value / 100.0, 0.0, 1.0);
        return new UiTransparencyStyle(
            AcrylicEnabled: ratio > 0,
            ChromeAlpha: Math.Max((byte)1, Scale(0x40, ratio)),
            PanelAlpha: Scale(0x60, ratio),
            AnswerAlpha: Scale(0x50, ratio),
            ControlAlpha: Scale(0x50, ratio),
            ControlHoverAlpha: Scale(0x70, ratio),
            PrimaryButtonAlpha: Scale(0xCC, ratio),
            PrimaryButtonHoverAlpha: Scale(0xDD, ratio),
            PrimaryButtonPressedAlpha: Scale(0xAA, ratio));
    }

    private static byte Scale(byte value, double ratio)
    {
        return (byte)Math.Round(value * ratio);
    }
}
