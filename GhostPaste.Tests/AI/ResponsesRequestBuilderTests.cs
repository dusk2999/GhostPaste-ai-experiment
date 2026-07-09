using System.Text.Json;
using GhostPaste.AI;

namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class ResponsesRequestBuilderTests
{
    [TestMethod]
    public void EmptySettingsKeepModelDefaultsWithoutEndpointOrKey()
    {
        var settings = AiSettings.Empty;

        Assert.IsNull(settings.BaseUri);
        Assert.IsNull(settings.ResponsesUri);
        Assert.AreEqual("", settings.ApiKey);
        Assert.AreEqual("gpt-5.5-fast", settings.Model);
        Assert.AreEqual("xhigh", settings.ReasoningEffort);
        Assert.IsFalse(settings.IsConfigured);
    }

    [TestMethod]
    public void TryCreateNormalizesConfiguredResponsesEndpoint()
    {
        bool ok = AiSettings.TryCreate(
            "http://example.test/v1",
            "test-key",
            out var settings,
            out string error);

        Assert.IsTrue(ok, error);
        Assert.AreEqual("http://example.test/v1/", settings.BaseUri?.ToString());
        Assert.AreEqual("http://example.test/v1/responses", settings.ResponsesUri?.ToString());
        Assert.AreEqual("test-key", settings.ApiKey);
        Assert.IsTrue(settings.IsConfigured);
    }

    [TestMethod]
    public void TryCreateRejectsMissingEndpoint()
    {
        bool ok = AiSettings.TryCreate(
            "",
            "test-key",
            out var settings,
            out string error);

        Assert.IsFalse(ok);
        Assert.AreSame(AiSettings.Empty, settings);
        Assert.AreEqual("请填写 AI 调用地址。", error);
    }

    [TestMethod]
    public void BuildsTextOnlyResponsesRequest()
    {
        string json = ResponsesRequestBuilder.BuildJson(
            "gpt-5.5-fast",
            "解释这段文字",
            []);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        Assert.AreEqual("gpt-5.5-fast", root.GetProperty("model").GetString());
        Assert.AreEqual("xhigh", root.GetProperty("reasoning").GetProperty("effort").GetString());
        var content = root.GetProperty("input")[0].GetProperty("content");
        Assert.AreEqual("input_text", content[0].GetProperty("type").GetString());
        Assert.AreEqual("解释这段文字", content[0].GetProperty("text").GetString());
        Assert.AreEqual(1, content.GetArrayLength());
    }

    [TestMethod]
    public void BuildsTextAndImageResponsesRequest()
    {
        var attachment = new AiMessageAttachment(
            "screen.png",
            "image/png",
            "data:image/png;base64,AAAA");

        string json = ResponsesRequestBuilder.BuildJson(
            "gpt-5.5-fast",
            "这张截图里有什么？",
            [attachment]);

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("input")[0].GetProperty("content");
        Assert.AreEqual(2, content.GetArrayLength());
        Assert.AreEqual("input_image", content[1].GetProperty("type").GetString());
        Assert.AreEqual("data:image/png;base64,AAAA", content[1].GetProperty("image_url").GetString());
    }
}
