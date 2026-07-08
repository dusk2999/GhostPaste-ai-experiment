namespace GhostPaste.Screenshots;

public sealed record ScreenshotResult(
    string StrategyName,
    byte[] PngBytes,
    int Width,
    int Height,
    string Message)
{
    public bool HasImage => PngBytes.Length > 0;

    public static ScreenshotResult Success(string strategyName, byte[] pngBytes, int width, int height)
    {
        return new ScreenshotResult(strategyName, pngBytes, width, height, "截图已添加。");
    }

    public static ScreenshotResult Failure(string strategyName, string message)
    {
        return new ScreenshotResult(strategyName, [], 0, 0, message);
    }

    public string ToDataUrl()
    {
        return HasImage
            ? $"data:image/png;base64,{Convert.ToBase64String(PngBytes)}"
            : "";
    }
}
