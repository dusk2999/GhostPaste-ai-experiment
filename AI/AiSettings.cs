namespace GhostPaste.AI;

public sealed record AiSettings(Uri BaseUri, string ApiKey, string Model, string ReasoningEffort)
{
    public static AiSettings Default => new(
        new Uri("http://49.51.186.85/v1/"),
        LocalSecrets.ApiKey,
        "gpt-5.5-fast",
        "xhigh");

    public Uri ResponsesUri => new(BaseUri, "responses");
}
