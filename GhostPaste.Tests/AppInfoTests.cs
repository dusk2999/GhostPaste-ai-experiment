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
}
