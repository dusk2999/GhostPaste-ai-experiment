using GhostPaste.Screenshots;

namespace GhostPaste.AI;

public sealed record ImageAttachment(
    string FileName,
    string MediaType,
    byte[] PngBytes,
    int Width,
    int Height,
    string SourceName)
{
    public static ImageAttachment FromScreenshot(ScreenshotResult screenshot)
    {
        if (!screenshot.HasImage)
        {
            throw new ArgumentException("Screenshot must contain image bytes.", nameof(screenshot));
        }

        return new ImageAttachment(
            "screenshot.png",
            "image/png",
            screenshot.PngBytes,
            screenshot.Width,
            screenshot.Height,
            screenshot.StrategyName);
    }

    public ImageAttachment WithPngBytes(byte[] pngBytes, int width, int height, string sourceName)
    {
        return this with
        {
            PngBytes = pngBytes,
            Width = width,
            Height = height,
            SourceName = sourceName
        };
    }

    public AiMessageAttachment ToAiMessageAttachment()
    {
        string dataUrl = $"data:{MediaType};base64,{Convert.ToBase64String(PngBytes)}";
        return new AiMessageAttachment(FileName, MediaType, dataUrl);
    }
}
