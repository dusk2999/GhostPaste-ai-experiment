namespace GhostPaste.Tests;

[TestClass]
public sealed class AboutWindowLayoutTests
{
    [TestMethod]
    public void AboutContentUsesScrollableMiddleRegion()
    {
        string xaml = File.ReadAllText(FindProjectFile("AboutWindow.xaml"));

        StringAssert.Contains(xaml, "x:Name=\"AboutContentScrollViewer\"");
        StringAssert.Contains(xaml, "Grid.Row=\"1\"");
        StringAssert.Contains(xaml, "VerticalScrollBarVisibility=\"Auto\"");
        StringAssert.Contains(xaml, "Grid.Row=\"2\"");
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
