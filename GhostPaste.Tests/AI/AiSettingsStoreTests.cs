using GhostPaste.AI;

namespace GhostPaste.Tests.AI;

[TestClass]
public sealed class AiSettingsStoreTests
{
    [TestMethod]
    public void MissingConfigLoadsEmptyDefaults()
    {
        string configPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "ai-settings.json");
        var store = new AiSettingsStore(configPath);

        AiSettings settings = store.Load();

        Assert.AreSame(AiSettings.Empty, settings);
    }

    [TestMethod]
    public void SaveAndLoadRoundTripsEndpointAndKey()
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string configPath = Path.Combine(directory, "ai-settings.json");
        var store = new AiSettingsStore(configPath);
        Assert.IsTrue(AiSettings.TryCreate(
            "http://example.test/v1",
            "local-key",
            out var settings,
            out string error), error);

        store.Save(settings);
        AiSettings loaded = store.Load();

        Assert.AreEqual("http://example.test/v1/", loaded.BaseUri?.ToString());
        Assert.AreEqual("local-key", loaded.ApiKey);
        Assert.AreEqual("gpt-5.5-fast", loaded.Model);
        Assert.AreEqual("xhigh", loaded.ReasoningEffort);
    }
}
