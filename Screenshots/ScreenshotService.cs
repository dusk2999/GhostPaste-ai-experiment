using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using GhostPaste.Native;

namespace GhostPaste.Screenshots;

public sealed class ScreenshotService
{
    public ScreenshotResult CaptureFullScreen()
    {
        var bounds = new Rectangle(
            (int)SystemParameters.VirtualScreenLeft,
            (int)SystemParameters.VirtualScreenTop,
            (int)SystemParameters.VirtualScreenWidth,
            (int)SystemParameters.VirtualScreenHeight);

        return CaptureScreenRectangle("全屏截图", bounds);
    }

    public ScreenshotResult CaptureWindowFromScreen(nint hwnd)
    {
        if (!TryGetWindowBounds(hwnd, out var bounds))
        {
            return ScreenshotResult.Failure("目标窗口截图", "目标窗口不可用。");
        }

        return CaptureScreenRectangle("目标窗口截图", bounds);
    }

    public ScreenshotResult CaptureWindowWithPrintWindow(nint hwnd)
    {
        if (!TryGetWindowBounds(hwnd, out var bounds))
        {
            return ScreenshotResult.Failure("PrintWindow截图", "目标窗口不可用。");
        }

        try
        {
            using var bitmap = new Bitmap(bounds.Width, bounds.Height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            nint hdc = graphics.GetHdc();
            try
            {
                bool ok = User32.PrintWindow(hwnd, hdc, 2);
                if (!ok)
                {
                    return ScreenshotResult.Failure("PrintWindow截图", "目标窗口拒绝 PrintWindow 捕获。");
                }
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }

            return BitmapToResult("PrintWindow截图", bitmap);
        }
        catch (Exception ex)
        {
            return ScreenshotResult.Failure("PrintWindow截图", $"截图失败：{ex.Message}");
        }
    }

    private static ScreenshotResult CaptureScreenRectangle(string strategyName, Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return ScreenshotResult.Failure(strategyName, "截图区域无效。");
        }

        try
        {
            using var bitmap = new Bitmap(bounds.Width, bounds.Height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(bounds.Left, bounds.Top, 0, 0, bounds.Size);
            return BitmapToResult(strategyName, bitmap);
        }
        catch (Exception ex)
        {
            return ScreenshotResult.Failure(strategyName, $"截图失败：{ex.Message}");
        }
    }

    private static ScreenshotResult BitmapToResult(string strategyName, Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return ScreenshotResult.Success(strategyName, stream.ToArray(), bitmap.Width, bitmap.Height);
    }

    private static bool TryGetWindowBounds(nint hwnd, out Rectangle bounds)
    {
        bounds = Rectangle.Empty;
        if (hwnd == nint.Zero || !User32.GetWindowRect(hwnd, out var rect))
        {
            return false;
        }

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;
        if (width <= 0 || height <= 0)
        {
            return false;
        }

        bounds = new Rectangle(rect.Left, rect.Top, width, height);
        return true;
    }
}
