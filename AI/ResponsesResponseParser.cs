using System.Text.Json;

namespace GhostPaste.AI;

public static class ResponsesResponseParser
{
    private const string EmptyResponseText = "AI 没有返回可显示的文本。";

    public static string ExtractText(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        if (TryGetNonEmptyString(root, "output_text", out string outputText))
        {
            return outputText;
        }

        var parts = new List<string>();
        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var contentItem in content.EnumerateArray())
                {
                    if (TryGetNonEmptyString(contentItem, "text", out string text))
                    {
                        parts.Add(text);
                    }
                }
            }
        }

        return parts.Count > 0
            ? string.Join(Environment.NewLine, parts)
            : EmptyResponseText;
    }

    private static bool TryGetNonEmptyString(JsonElement element, string propertyName, out string value)
    {
        value = "";
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        value = property.GetString() ?? "";
        return !string.IsNullOrWhiteSpace(value);
    }
}
