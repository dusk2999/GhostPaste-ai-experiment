namespace GhostPaste.AI;

public sealed record AiSettings(Uri? BaseUri, string ApiKey, string Model, string ReasoningEffort)
{
    public const string DefaultModel = "gpt-5.5-fast";

    public const string DefaultReasoningEffort = "xhigh";

    public static AiSettings Empty { get; } = new(null, "", DefaultModel, DefaultReasoningEffort);

    public static AiSettings Default => Empty;

    public bool IsConfigured => BaseUri is not null && !string.IsNullOrWhiteSpace(ApiKey);

    public Uri? ResponsesUri => BaseUri is null ? null : new Uri(BaseUri, "responses");

    public static bool TryCreate(
        string baseUriText,
        string apiKey,
        out AiSettings settings,
        out string error)
    {
        settings = Empty;
        error = "";

        if (string.IsNullOrWhiteSpace(baseUriText))
        {
            error = "请填写 AI 调用地址。";
            return false;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            error = "请填写 AI 密钥。";
            return false;
        }

        string normalizedBaseUriText = NormalizeBaseUriText(baseUriText);
        if (!Uri.TryCreate(normalizedBaseUriText, UriKind.Absolute, out Uri? baseUri)
            || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
        {
            error = "AI 调用地址必须是 http 或 https 开头的完整地址。";
            return false;
        }

        settings = new AiSettings(
            baseUri,
            apiKey.Trim(),
            DefaultModel,
            DefaultReasoningEffort);
        return true;
    }

    private static string NormalizeBaseUriText(string baseUriText)
    {
        string normalized = baseUriText.Trim();
        string withoutTrailingSlash = normalized.TrimEnd('/');
        if (withoutTrailingSlash.EndsWith("/responses", StringComparison.OrdinalIgnoreCase))
        {
            normalized = withoutTrailingSlash[..^"/responses".Length];
        }

        if (!normalized.EndsWith("/", StringComparison.Ordinal))
        {
            normalized += "/";
        }

        return normalized;
    }
}
