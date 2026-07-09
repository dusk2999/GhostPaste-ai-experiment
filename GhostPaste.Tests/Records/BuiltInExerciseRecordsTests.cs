using GhostPaste.Records;

namespace GhostPaste.Tests.Records;

[TestClass]
public sealed class BuiltInExerciseRecordsTests
{
    [TestMethod]
    public void LoadReturnsThirteenComputerNetworkExercises()
    {
        var exercises = BuiltInExerciseRecords.Load();

        Assert.HasCount(13, exercises);
        CollectionAssert.AreEqual(Enumerable.Range(2, 13).ToArray(), exercises.Select(item => item.Number).ToArray());
        Assert.IsTrue(exercises.All(item => item.Text.Contains("**题目：**", StringComparison.Ordinal)));
        Assert.IsTrue(exercises.All(item => item.Text.Contains("**答案：**", StringComparison.Ordinal)));
        StringAssert.Contains(exercises[0].Text, "192.168.1.0/24");
        StringAssert.Contains(exercises[^1].Text, "下一跳");
    }

    [TestMethod]
    public void SeedAddsBuiltInExercisesInQuestionOrder()
    {
        var board = new RecordBoard();

        BuiltInExerciseRecords.Seed(board);

        Assert.HasCount(13, board.Records);
        StringAssert.StartsWith(board.Records[0].Text, "## 第 2 题");
        StringAssert.StartsWith(board.Records[^1].Text, "## 第 14 题");
    }
}
