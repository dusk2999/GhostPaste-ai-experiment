namespace GhostPaste;

internal static partial class LocalSecrets
{
    public static string ApiKey
    {
        get
        {
            string apiKey = "";
            ConfigureApiKey(ref apiKey);
            return apiKey;
        }
    }

    static partial void ConfigureApiKey(ref string apiKey);
}
