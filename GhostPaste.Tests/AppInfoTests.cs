using GhostPaste;

namespace GhostPaste.Tests;

[TestClass]
public sealed class AppInfoTests
{
    [TestMethod]
    public void GitHubUrlPointsToPublicRepository()
    {
        Assert.AreEqual("https://github.com/dusk2999/GhostPaste", AppInfo.GitHubUrl);
    }

    [TestMethod]
    public void CreditLineUsesEnglishDuskCredit()
    {
        Assert.AreEqual("Made by dusk", AppInfo.CreditLine);
    }

    [TestMethod]
    public void DescriptionExplainsWhatGhostPasteDoes()
    {
        StringAssert.Contains(AppInfo.Description, "模拟真实键盘输入");
        StringAssert.Contains(AppInfo.Description, "无法直接粘贴");
    }

    [TestMethod]
    public void UsageInstructionsStayShortAndActionable()
    {
        CollectionAssert.AreEqual(
            new[]
            {
                "1. 先点击要接收文本的目标输入框。",
                "2. 回到 GhostPaste 输入内容，并按需要调整速度。",
                "3. 点击发送；发送中可按 Esc 取消。"
            },
            AppInfo.UsageInstructions.ToArray());
    }
}
