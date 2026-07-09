using GhostPaste.AI;
using GhostPaste.Screenshots;

namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class ImageAttachmentTests
{
    [TestMethod]
    public void FromScreenshotKeepsBytesAndCreatesResponsesAttachment()
    {
        byte[] pngBytes = [1, 2, 3, 4];
        var screenshot = ScreenshotResult.Success("全屏截图", pngBytes, 12, 8);

        var image = ImageAttachment.FromScreenshot(screenshot);
        var aiAttachment = image.ToAiMessageAttachment();

        Assert.AreEqual("screenshot.png", image.FileName);
        Assert.AreEqual("image/png", image.MediaType);
        CollectionAssert.AreEqual(pngBytes, image.PngBytes);
        Assert.AreEqual(12, image.Width);
        Assert.AreEqual(8, image.Height);
        Assert.AreEqual("全屏截图", image.SourceName);
        Assert.AreEqual("screenshot.png", aiAttachment.FileName);
        Assert.AreEqual("image/png", aiAttachment.MediaType);
        Assert.AreEqual("data:image/png;base64,AQIDBA==", aiAttachment.DataUrl);
    }

    [TestMethod]
    public void WithPngBytesReturnsUpdatedImageMetadata()
    {
        var image = new ImageAttachment("screenshot.png", "image/png", [1, 2, 3], 100, 80, "全屏截图");

        var cropped = image.WithPngBytes([9, 8], 30, 20, "裁切截图");

        Assert.AreEqual("screenshot.png", cropped.FileName);
        Assert.AreEqual("image/png", cropped.MediaType);
        CollectionAssert.AreEqual(new byte[] { 9, 8 }, cropped.PngBytes);
        Assert.AreEqual(30, cropped.Width);
        Assert.AreEqual(20, cropped.Height);
        Assert.AreEqual("裁切截图", cropped.SourceName);
    }
}
