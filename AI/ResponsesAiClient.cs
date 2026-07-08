using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace GhostPaste.AI;

public sealed class ResponsesAiClient
{
    private readonly HttpClient _httpClient;
    private readonly AiSettings _settings;

    public ResponsesAiClient(HttpClient httpClient, AiSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
    }

    public async Task<string> SendAsync(
        string prompt,
        IReadOnlyList<AiMessageAttachment> attachments,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return "缺少本地 AI 密钥：请创建 LocalSecrets.g.cs 后重新编译。";
        }

        string requestJson = ResponsesRequestBuilder.BuildJson(
            _settings.Model,
            prompt,
            attachments,
            _settings.ReasoningEffort);
        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.ResponsesUri)
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);

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
