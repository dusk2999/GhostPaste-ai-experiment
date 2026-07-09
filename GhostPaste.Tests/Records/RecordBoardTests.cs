using GhostPaste.AI;
using GhostPaste.Records;
using System.Drawing;
using System.Drawing.Imaging;

namespace GhostPaste.Tests.Records;

[TestClass]
public sealed class RecordBoardTests
{
    [TestMethod]
    public void AddRecordStoresTextAndImageNewestFirst()
    {
        var board = new RecordBoard();
        var image = new ImageAttachment("screenshot.png", "image/png", CreatePng(), 1, 1, "目标窗口截图");

        var first = board.AddRecord("第一条", null);
        var second = board.AddRecord("带图", image);

        Assert.HasCount(2, board.Records);
        Assert.AreEqual(second.Id, board.Records[0].Id);
        Assert.AreEqual(first.Id, board.Records[1].Id);
        Assert.AreEqual("带图", board.Records[0].Text);
        Assert.AreSame(image, board.Records[0].Image);
        Assert.IsTrue(board.Records[0].HasImage);
        Assert.IsTrue(board.Records[0].HasText);
    }

    [TestMethod]
    public void AddRecordRejectsEmptyTextWithoutImage()
    {
        var board = new RecordBoard();

        try
        {
            board.AddRecord("  ", null);
            Assert.Fail("Expected ArgumentException.");
        }
        catch (ArgumentException)
        {
        }

        Assert.IsEmpty(board.Records);
    }

    [TestMethod]
    public void RemoveDeletesRecordById()
    {
        var board = new RecordBoard();
        var record = board.AddRecord("要删除", null);

        Assert.IsTrue(board.Remove(record.Id));

        Assert.IsEmpty(board.Records);
        Assert.IsFalse(board.Remove(record.Id));
    }

    private static byte[] CreatePng()
    {
        using var bitmap = new Bitmap(1, 1);
        bitmap.SetPixel(0, 0, Color.Red);
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
