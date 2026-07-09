using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace GhostPaste.Records;

public sealed record BuiltInExerciseRecord(int Number, string Text);

public static partial class BuiltInExerciseRecords
{
    private const string ResourceName = "GhostPaste.Records.BuiltInComputerNetworkExercises.md";

    public static IReadOnlyList<BuiltInExerciseRecord> Load()
    {
        string markdown = LoadMarkdown();
        var matches = ExerciseHeadingRegex().Matches(markdown);
        if (matches.Count == 0)
        {
            return [];
        }

        var records = new List<BuiltInExerciseRecord>(matches.Count);
        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : markdown.Length;
            string text = markdown[match.Index..nextIndex].Trim();
            int number = int.Parse(match.Groups["number"].Value);
            records.Add(new BuiltInExerciseRecord(number, text));
        }

        return records;
    }

    public static void Seed(RecordBoard board)
    {
        foreach (var exercise in Load().Reverse())
        {
            board.AddRecord(exercise.Text, null);
        }
    }

    private static string LoadMarkdown()
    {
        var assembly = typeof(BuiltInExerciseRecords).Assembly;
        using Stream? stream = assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"Missing embedded resource: {ResourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd().Replace("\r\n", "\n", StringComparison.Ordinal);
    }

    [GeneratedRegex(@"^## 第 (?<number>\d+) 题\s*$", RegexOptions.Multiline)]
    private static partial Regex ExerciseHeadingRegex();
}
