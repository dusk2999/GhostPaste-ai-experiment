using GhostPaste.Screenshots;

namespace GhostPaste.Tests.Screenshots;

[TestClass]
public sealed class ScreenshotResultTests
{
    [TestMethod]
    public void SuccessResultConvertsPngBytesToDataUrl()
    {
        byte[] pngBytes = [0x89, 0x50, 0x4E, 0x47];
        var result = ScreenshotResult.Success("FullScreen", pngBytes, 2, 2);

        Assert.IsTrue(result.HasImage);
        Assert.AreEqual("FullScreen", result.StrategyName);
        Assert.AreEqual("data:image/png;base64,iVBORw==", result.ToDataUrl());
    }

    [TestMethod]
    public void FailureResultKeepsReadableReason()
    {
        var result = ScreenshotResult.Failure("TargetWindow", "目标窗口不可用。");

        Assert.IsFalse(result.HasImage);
        Assert.AreEqual("目标窗口不可用。", result.Message);
        Assert.AreEqual("", result.ToDataUrl());
    }
}
