using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GhostPaste.Screenshots;

public readonly record struct PixelCropRect(int X, int Y, int Width, int Height)
{
    public Rectangle ClampTo(int imageWidth, int imageHeight)
    {
        int left = Math.Clamp(X, 0, imageWidth);
        int top = Math.Clamp(Y, 0, imageHeight);
        int right = Math.Clamp(X + Width, left, imageWidth);
        int bottom = Math.Clamp(Y + Height, top, imageHeight);
        return new Rectangle(left, top, right - left, bottom - top);
    }
}

public static class ImageCropper
{
    public static byte[] CropPng(byte[] pngBytes, PixelCropRect cropRect)
    {
        if (pngBytes.Length == 0)
        {
            throw new ArgumentException("PNG bytes cannot be empty.", nameof(pngBytes));
        }

        using var input = new MemoryStream(pngBytes);
        using var source = new Bitmap(input);
        Rectangle crop = cropRect.ClampTo(source.Width, source.Height);
        if (crop.Width <= 0 || crop.Height <= 0)
        {
            throw new ArgumentException("Crop selection must overlap the image.", nameof(cropRect));
        }

        using var cropped = source.Clone(crop, PixelFormat.Format32bppArgb);
        using var output = new MemoryStream();
        cropped.Save(output, ImageFormat.Png);
        return output.ToArray();
    }
}
