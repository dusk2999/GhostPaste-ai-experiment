namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class MarkdownFlowDocumentRendererSourceTests
{
    [TestMethod]
    public void RendererUsesMarkdigAndHandlesCommonMarkdownBlocks()
    {
        string source = File.ReadAllText(FindProjectFile("AI", "MarkdownFlowDocumentRenderer.cs"));

        StringAssert.Contains(source, "using Markdig");
        StringAssert.Contains(source, "Markdown.Parse");
        StringAssert.Contains(source, "HeadingBlock");
        StringAssert.Contains(source, "ListBlock");
        StringAssert.Contains(source, "FencedCodeBlock");
        StringAssert.Contains(source, "EmphasisInline");
        StringAssert.Contains(source, "CodeInline");
        StringAssert.Contains(source, "LinkInline");
    }

    private static string FindProjectFile(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(new[] { directory.FullName }.Concat(pathParts).ToArray());
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        Assert.Fail($"Could not find {Path.Combine(pathParts)}.");
        throw new InvalidOperationException();
    }
}
