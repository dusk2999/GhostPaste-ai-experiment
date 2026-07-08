namespace GhostPaste.Tests;

[TestClass]
public sealed class MainWindowLayoutTests
{
    [TestMethod]
    public void MainWindowContainsAiChatTabAndScreenshotControls()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));

        StringAssert.Contains(xaml, "Header=\"AI问答\"");
        StringAssert.Contains(xaml, "x:Name=\"AiPromptBox\"");
        StringAssert.Contains(xaml, "x:Name=\"FullScreenCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"TargetWindowCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"PrintWindowCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"AiSendButton\"");
    }

    [TestMethod]
    public void MainWindowContainsTransparencySlider()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));

        StringAssert.Contains(xaml, "x:Name=\"WindowChromeBorder\"");
        StringAssert.Contains(xaml, "x:Name=\"UiOpacitySlider\"");
        StringAssert.Contains(xaml, "ValueChanged=\"UiOpacitySlider_ValueChanged\"");
        StringAssert.Contains(xaml, "完全透明");
    }

    private static string FindProjectFile(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        Assert.Fail($"Could not find {fileName}.");
        throw new InvalidOperationException();
    }
}
