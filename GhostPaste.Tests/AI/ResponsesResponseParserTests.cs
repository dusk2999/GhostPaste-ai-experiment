using GhostPaste.AI;

namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class ResponsesResponseParserTests
{
    [TestMethod]
    public void ExtractsOutputTextWhenPresent()
    {
        string json = """{"output_text":"你好，我看到了截图。"}""";

        Assert.AreEqual("你好，我看到了截图。", ResponsesResponseParser.ExtractText(json));
    }

    [TestMethod]
    public void FallsBackToOutputContentText()
    {
        string json = """
        {
          "output": [
            {
              "type": "message",
              "content": [
                { "type": "output_text", "text": "第一段" },
                { "type": "output_text", "text": "第二段" }
              ]
            }
          ]
        }
        """;

        Assert.AreEqual("第一段\r\n第二段", ResponsesResponseParser.ExtractText(json));
    }

    [TestMethod]
    public void ReturnsReadableFallbackForEmptyResponses()
    {
        Assert.AreEqual("AI 没有返回可显示的文本。", ResponsesResponseParser.ExtractText("""{"output":[]}"""));
    }
}
