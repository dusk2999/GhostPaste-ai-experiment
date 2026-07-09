using System.Drawing;
using System.Drawing.Imaging;
using GhostPaste.Screenshots;

namespace GhostPaste.Tests.Screenshots;

[TestClass]
public sealed class ImageCropperTests
{
    [TestMethod]
    public void CropPngReturnsSelectedPixels()
    {
        byte[] source = CreateSamplePng();

        byte[] cropped = ImageCropper.CropPng(source, new PixelCropRect(1, 1, 2, 2));

        using var stream = new MemoryStream(cropped);
        using var bitmap = new Bitmap(stream);

        Assert.AreEqual(2, bitmap.Width);
        Assert.AreEqual(2, bitmap.Height);
        Assert.AreEqual(Color.Yellow.ToArgb(), bitmap.GetPixel(0, 0).ToArgb());
        Assert.AreEqual(Color.Cyan.ToArgb(), bitmap.GetPixel(1, 0).ToArgb());
        Assert.AreEqual(Color.Black.ToArgb(), bitmap.GetPixel(0, 1).ToArgb());
        Assert.AreEqual(Color.White.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
    }

    [TestMethod]
    public void CropPngClampsSelectionToImageBounds()
    {
        byte[] source = CreateSamplePng();

        byte[] cropped = ImageCropper.CropPng(source, new PixelCropRect(2, 1, 8, 8));

        using var stream = new MemoryStream(cropped);
        using var bitmap = new Bitmap(stream);

        Assert.AreEqual(2, bitmap.Width);
        Assert.AreEqual(2, bitmap.Height);
        Assert.AreEqual(Color.Cyan.ToArgb(), bitmap.GetPixel(0, 0).ToArgb());
        Assert.AreEqual(Color.Magenta.ToArgb(), bitmap.GetPixel(1, 0).ToArgb());
        Assert.AreEqual(Color.White.ToArgb(), bitmap.GetPixel(0, 1).ToArgb());
        Assert.AreEqual(Color.Gray.ToArgb(), bitmap.GetPixel(1, 1).ToArgb());
    }

    private static byte[] CreateSamplePng()
    {
        using var bitmap = new Bitmap(4, 3);
        Color[] colors =
        [
            Color.Red, Color.Green, Color.Blue, Color.Orange,
            Color.Purple, Color.Yellow, Color.Cyan, Color.Magenta,
            Color.Brown, Color.Black, Color.White, Color.Gray
        ];

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                bitmap.SetPixel(x, y, colors[(y * bitmap.Width) + x]);
            }
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
