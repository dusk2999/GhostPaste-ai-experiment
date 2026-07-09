using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace GhostPaste.AI;

public sealed class ResponsesAiClient
{
    private readonly HttpClient _httpClient;
    private readonly Func<AiSettings> _settingsProvider;

    public ResponsesAiClient(HttpClient httpClient, Func<AiSettings> settingsProvider)
    {
        _httpClient = httpClient;
        _settingsProvider = settingsProvider;
    }

    public ResponsesAiClient(HttpClient httpClient, AiSettings settings)
        : this(httpClient, () => settings)
    {
    }

    public async Task<string> SendAsync(
        string prompt,
        IReadOnlyList<AiMessageAttachment> attachments,
        CancellationToken cancellationToken)
    {
        AiSettings settings = _settingsProvider();
        Uri? responsesUri = settings.ResponsesUri;
        if (!settings.IsConfigured || responsesUri is null)
        {
            return "请先在 AI 设置中填写调用地址和密钥。";
        }

        string requestJson = ResponsesRequestBuilder.BuildJson(
            settings.Model,
            prompt,
            attachments,
            settings.ReasoningEffort);
        using var request = new HttpRequestMessage(HttpMethod.Post, responsesUri)
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return $"AI 请求失败：{(int)response.StatusCode} {response.ReasonPhrase}\r\n{responseJson}";
        }

        try
        {
            return ResponsesResponseParser.ExtractText(responseJson);
        }
        catch (JsonException)
        {
            return $"AI 返回了无法解析的内容：\r\n{responseJson}";
        }
    }
}
