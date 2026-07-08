using System.Text.Encodings.Web;
using System.Text.Json;

namespace GhostPaste.AI;

public static class ResponsesRequestBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string BuildJson(
        string model,
        string prompt,
        IReadOnlyList<AiMessageAttachment> attachments,
        string reasoningEffort = "xhigh")
    {
        var content = new List<Dictionary<string, string>>
        {
            new()
            {
                ["type"] = "input_text",
                ["text"] = prompt
            }
        };

        foreach (var attachment in attachments)
        {
            content.Add(new Dictionary<string, string>
            {
                ["type"] = "input_image",
                ["image_url"] = attachment.DataUrl
            });
        }

        var request = new
        {
            model,
            reasoning = new
            {
                effort = reasoningEffort
            },
            input = new[]
            {
                new
                {
                    role = "user",
                    content
                }
            }
        };

        return JsonSerializer.Serialize(request, JsonOptions);
    }
}
