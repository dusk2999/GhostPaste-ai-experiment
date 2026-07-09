using System.Windows;
using System.Windows.Media;
using GhostPaste.AI;
using GhostPaste.Helpers;

namespace GhostPaste.Records;

public sealed class BoardRecord
{
    public BoardRecord(Guid id, DateTimeOffset createdAt, string text, ImageAttachment? image)
    {
        Id = id;
        CreatedAt = createdAt;
        Text = text;
        Image = image;
        PreviewImage = image is null ? null : WpfImageFactory.FromPngBytes(image.PngBytes);
    }

    public Guid Id { get; }

    public DateTimeOffset CreatedAt { get; }

    public string Text { get; }

    public ImageAttachment? Image { get; }

    public ImageSource? PreviewImage { get; }

    public bool HasText => !string.IsNullOrWhiteSpace(Text);

    public bool HasImage => Image is not null;

    public string TimeLabel => CreatedAt.ToLocalTime().ToString("HH:mm:ss");

    public string ImageSummary => Image is null ? "" : $"{Image.SourceName} · {Image.Width} x {Image.Height}";

    public Visibility TextVisibility => HasText ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ImageVisibility => HasImage ? Visibility.Visible : Visibility.Collapsed;
}
