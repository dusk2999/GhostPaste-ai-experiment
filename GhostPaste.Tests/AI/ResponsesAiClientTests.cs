using System.Net;
using System.Net.Http;
using GhostPaste.AI;

namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class ResponsesAiClientTests
{
    [TestMethod]
    public async Task SendAsyncUsesLatestConfiguredEndpointAndKey()
    {
        var handler = new CaptureHandler();
        using var httpClient = new HttpClient(handler);
        Assert.IsTrue(AiSettings.TryCreate(
            "http://example.test/v1",
            "first-key",
            out var firstSettings,
            out string firstError), firstError);
        Assert.IsTrue(AiSettings.TryCreate(
            "http://example.test/alternate",
            "second-key",
            out var secondSettings,
            out string secondError), secondError);
        AiSettings currentSettings = firstSettings;
        var client = new ResponsesAiClient(httpClient, () => currentSettings);

        currentSettings = secondSettings;
        string answer = await client.SendAsync("hello", [], CancellationToken.None);

        Assert.AreEqual("pong", answer);
        Assert.AreEqual("http://example.test/alternate/responses", handler.LastRequestUri?.ToString());
        Assert.AreEqual("Bearer", handler.LastAuthorizationScheme);
        Assert.AreEqual("second-key", handler.LastAuthorizationParameter);
    }

    [TestMethod]
    public async Task SendAsyncPromptsForSettingsWhenEndpointOrKeyIsMissing()
    {
        using var httpClient = new HttpClient(new CaptureHandler());
        var client = new ResponsesAiClient(httpClient, () => AiSettings.Empty);

        string answer = await client.SendAsync("hello", [], CancellationToken.None);

        Assert.AreEqual("请先在 AI 设置中填写调用地址和密钥。", answer);
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        public string? LastAuthorizationScheme { get; private set; }

        public string? LastAuthorizationParameter { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            LastAuthorizationScheme = request.Headers.Authorization?.Scheme;
            LastAuthorizationParameter = request.Headers.Authorization?.Parameter;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"output\":[{\"content\":[{\"type\":\"output_text\",\"text\":\"pong\"}]}]}")
            };
            return Task.FromResult(response);
        }
    }
}
