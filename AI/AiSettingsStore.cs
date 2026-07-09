using System.IO;
using System.Text.Json;

namespace GhostPaste.AI;

public sealed class AiSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configPath;

    public AiSettingsStore()
        : this(DefaultConfigPath)
    {
    }

    public AiSettingsStore(string configPath)
    {
        _configPath = configPath;
    }

    public static string DefaultConfigPath
    {
        get
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "GhostPaste", "ai-settings.json");
        }
    }

    public AiSettings Load()
    {
        if (!File.Exists(_configPath))
        {
            return AiSettings.Empty;
        }

        try
        {
            string json = File.ReadAllText(_configPath);
            var stored = JsonSerializer.Deserialize<StoredAiSettings>(json, JsonOptions);
            if (stored is null
                || !AiSettings.TryCreate(
                    stored.BaseUri ?? "",
                    stored.ApiKey ?? "",
                    out AiSettings settings,
                    out _))
            {
                return AiSettings.Empty;
            }

            return settings with
            {
                Model = string.IsNullOrWhiteSpace(stored.Model)
                    ? AiSettings.DefaultModel
                    : stored.Model.Trim(),
                ReasoningEffort = string.IsNullOrWhiteSpace(stored.ReasoningEffort)
                    ? AiSettings.DefaultReasoningEffort
                    : stored.ReasoningEffort.Trim()
            };
        }
        catch (IOException)
        {
            return AiSettings.Empty;
        }
        catch (JsonException)
        {
            return AiSettings.Empty;
        }
        catch (UnauthorizedAccessException)
        {
            return AiSettings.Empty;
        }
    }

    public void Save(AiSettings settings)
    {
        string? directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var stored = new StoredAiSettings(
            settings.BaseUri?.ToString() ?? "",
            settings.ApiKey,
            settings.Model,
            settings.ReasoningEffort);
        string json = JsonSerializer.Serialize(stored, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private sealed record StoredAiSettings(
        string? BaseUri,
        string? ApiKey,
        string? Model,
        string? ReasoningEffort);
}
