using System.Collections.ObjectModel;
using GhostPaste.AI;

namespace GhostPaste.Records;

public sealed class RecordBoard
{
    public ObservableCollection<BoardRecord> Records { get; } = [];

    public BoardRecord AddRecord(string? text, ImageAttachment? image)
    {
        string normalizedText = text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(normalizedText) && image is null)
        {
            throw new ArgumentException("Record must contain text or image.", nameof(text));
        }

        var record = new BoardRecord(Guid.NewGuid(), DateTimeOffset.Now, normalizedText, image);
        Records.Insert(0, record);
        return record;
    }

    public bool Remove(Guid id)
    {
        var record = Records.FirstOrDefault(item => item.Id == id);
        if (record is null)
        {
            return false;
        }

        Records.Remove(record);
        return true;
    }

    public void Clear()
    {
        Records.Clear();
    }
}
