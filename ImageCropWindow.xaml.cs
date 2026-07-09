using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GhostPaste.AI;
using GhostPaste.Helpers;
using GhostPaste.Screenshots;

namespace GhostPaste;

public partial class ImageCropWindow : Window
{
    private readonly ImageAttachment _sourceImage;
    private Point? _selectionStart;
    private Rect? _selectionInSurface;

    public ImageCropWindow(ImageAttachment image)
    {
        InitializeComponent();
        _sourceImage = image;
        CropPreviewImage.Source = WpfImageFactory.FromPngBytes(image.PngBytes);
        SelectionCanvas.SizeChanged += (_, _) => DrawSelection();
    }

    public ImageAttachment? CroppedImage { get; private set; }

    private void CropSurface_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _selectionStart = e.GetPosition(CropSurface);
        _selectionInSurface = new Rect(_selectionStart.Value, new Size(0, 0));
        CropSurface.CaptureMouse();
        DrawSelection();
        e.Handled = true;
    }

    private void CropSurface_MouseMove(object sender, MouseEventArgs e)
    {
        if (_selectionStart is null || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var current = ClampToSurface(e.GetPosition(CropSurface));
        _selectionInSurface = new Rect(_selectionStart.Value, current);
        DrawSelection();
    }

    private void CropSurface_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_selectionStart is null)
        {
            return;
        }

        CropSurface.ReleaseMouseCapture();
        _selectionStart = null;
        _selectionInSurface = NormalizeRect(_selectionInSurface);
        DrawSelection();
    }

    private void UseWholeImageButton_Click(object sender, RoutedEventArgs e)
    {
        CroppedImage = _sourceImage;
        DialogResult = true;
    }

    private void CropButton_Click(object sender, RoutedEventArgs e)
    {
        var cropRect = ToPixelCropRect(_selectionInSurface);
        if (cropRect is null)
        {
            CroppedImage = _sourceImage;
            DialogResult = true;
            return;
        }

        byte[] croppedBytes = ImageCropper.CropPng(_sourceImage.PngBytes, cropRect.Value);
        var bounds = cropRect.Value.ClampTo(_sourceImage.Width, _sourceImage.Height);
        CroppedImage = _sourceImage.WithPngBytes(croppedBytes, bounds.Width, bounds.Height, "裁切截图");
        DialogResult = true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void DrawSelection()
    {
        var selection = NormalizeRect(_selectionInSurface);
        if (selection is null || selection.Value.Width < 4 || selection.Value.Height < 4)
        {
            SelectionRectangle.Visibility = Visibility.Collapsed;
            SelectionStatusText.Text = "未选择区域时会发送整张图片。";
            return;
        }

        Rect rect = selection.Value;
        SelectionRectangle.Visibility = Visibility.Visible;
        Canvas.SetLeft(SelectionRectangle, rect.Left);
        Canvas.SetTop(SelectionRectangle, rect.Top);
        SelectionRectangle.Width = rect.Width;
        SelectionRectangle.Height = rect.Height;

        var pixelRect = ToPixelCropRect(rect);
        SelectionStatusText.Text = pixelRect is null
            ? "选择区域未覆盖图片。"
            : $"选择区域：{pixelRect.Value.Width} x {pixelRect.Value.Height}";
    }

    private PixelCropRect? ToPixelCropRect(Rect? selection)
    {
        var normalized = NormalizeRect(selection);
        if (normalized is null)
        {
            return null;
        }

        Rect imageRect = GetRenderedImageRect();
        Rect selected = Rect.Intersect(normalized.Value, imageRect);
        if (selected.IsEmpty || selected.Width < 4 || selected.Height < 4)
        {
            return null;
        }

        double scaleX = _sourceImage.Width / imageRect.Width;
        double scaleY = _sourceImage.Height / imageRect.Height;
        int x = (int)Math.Round((selected.Left - imageRect.Left) * scaleX);
        int y = (int)Math.Round((selected.Top - imageRect.Top) * scaleY);
        int width = (int)Math.Round(selected.Width * scaleX);
        int height = (int)Math.Round(selected.Height * scaleY);
        return new PixelCropRect(x, y, Math.Max(1, width), Math.Max(1, height));
    }

    private Rect GetRenderedImageRect()
    {
        double hostWidth = Math.Max(1, CropSurface.ActualWidth);
        double hostHeight = Math.Max(1, CropSurface.ActualHeight);
        double imageRatio = _sourceImage.Width / (double)_sourceImage.Height;
        double hostRatio = hostWidth / hostHeight;

        double width;
        double height;
        if (imageRatio >= hostRatio)
        {
            width = hostWidth;
            height = hostWidth / imageRatio;
        }
        else
        {
            height = hostHeight;
            width = hostHeight * imageRatio;
        }

        return new Rect((hostWidth - width) / 2, (hostHeight - height) / 2, width, height);
    }

    private Point ClampToSurface(Point point)
    {
        return new Point(
            Math.Clamp(point.X, 0, CropSurface.ActualWidth),
            Math.Clamp(point.Y, 0, CropSurface.ActualHeight));
    }

    private static Rect? NormalizeRect(Rect? rect)
    {
        if (rect is null)
        {
            return null;
        }

        Rect value = rect.Value;
        if (double.IsNaN(value.Width) || double.IsNaN(value.Height))
        {
            return null;
        }

        var normalized = new Rect(
            Math.Min(value.Left, value.Right),
            Math.Min(value.Top, value.Bottom),
            Math.Abs(value.Width),
            Math.Abs(value.Height));
        return normalized.Width <= 0 || normalized.Height <= 0 ? null : normalized;
    }
}
