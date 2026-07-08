using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GhostPaste.AI;
using GhostPaste.Helpers;
using GhostPaste.Native;
using GhostPaste.Screenshots;

namespace GhostPaste;

public partial class MainWindow : Window
{
    private nint _targetWindow;
    private nint _selfHandle;
    private CancellationTokenSource? _cts;
    private CancellationTokenSource? _aiCts;
    private bool _isSourceReady;
    private readonly DispatcherTimer _tracker;
    private readonly ResponsesAiClient _aiClient;
    private readonly MarkdownFlowDocumentRenderer _markdownRenderer = new();
    private readonly ScreenshotService _screenshotService = new();
    private AiMessageAttachment? _currentAttachment;

    public MainWindow()
    {
        InitializeComponent();
        _aiClient = new ResponsesAiClient(new HttpClient(), AiSettings.Default);
        SourceInitialized += OnSourceInitialized;

        // Poll foreground window every 300ms to track the last non-self window
        _tracker = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _tracker.Tick += Tracker_Tick;
        _tracker.Start();
        ApplyUiTransparency(100);
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        _selfHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        _isSourceReady = true;
        ApplyUiTransparency(UiOpacitySlider.Value);
    }

    private void Tracker_Tick(object? sender, EventArgs e)
    {
        var fg = User32.GetForegroundWindow();
        if (fg != nint.Zero && fg != _selfHandle)
        {
            _targetWindow = fg;
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AboutButton_Click(object sender, RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow
        {
            Owner = this
        };
        aboutWindow.ShowDialog();
    }

    private void UiOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ApplyUiTransparency(e.NewValue);
    }

    private void ApplyUiTransparency(double value)
    {
        if (!IsInitialized)
        {
            return;
        }

        var style = UiTransparencyPolicy.FromSliderValue(value);
        if (_isSourceReady)
        {
            GlassHelper.SetAcrylicEnabled(this, style.AcrylicEnabled);
        }

        SetBrushAlpha("ChromeSurfaceBrush", style.ChromeAlpha, 0xFF, 0xFF, 0xFF);
        SetBrushAlpha("PanelSurfaceBrush", style.PanelAlpha, 0xFF, 0xFF, 0xFF);
        SetBrushAlpha("AnswerSurfaceBrush", style.AnswerAlpha, 0xFF, 0xFF, 0xFF);
        SetBrushAlpha("ControlSurfaceBrush", style.ControlAlpha, 0xFF, 0xFF, 0xFF);
        SetBrushAlpha("ControlHoverBrush", style.ControlHoverAlpha, 0xFF, 0xFF, 0xFF);
        SetBrushAlpha("PrimaryButtonBrush", style.PrimaryButtonAlpha, 0x25, 0x63, 0xEB);
        SetBrushAlpha("PrimaryButtonHoverBrush", style.PrimaryButtonHoverAlpha, 0x3B, 0x82, 0xF6);
        SetBrushAlpha("PrimaryButtonPressedBrush", style.PrimaryButtonPressedAlpha, 0x1D, 0x4E, 0xD8);
        double primaryRatio = style.PrimaryButtonAlpha / (double)0xCC;
        byte textChannel = (byte)Math.Round(0x18 + ((0xFF - 0x18) * primaryRatio));
        SetBrushAlpha("PrimaryButtonTextBrush", 0xFF, textChannel, textChannel, textChannel);
        SetBrushAlpha("PrimaryButtonBorderBrush", 0x80, 0x25, 0x63, 0xEB);
    }

    private void SetBrushAlpha(string resourceKey, byte alpha, byte red, byte green, byte blue)
    {
        var color = Color.FromArgb(alpha, red, green, blue);
        if (Resources[resourceKey] is SolidColorBrush brush)
        {
            brush.Color = color;
            return;
        }

        Resources[resourceKey] = new SolidColorBrush(color);
    }

    private void SetAiAnswerMarkdown(string markdown)
    {
        AiMarkdownViewer.Document = _markdownRenderer.Render(markdown);
    }

    private async void FullScreenCaptureButton_Click(object sender, RoutedEventArgs e)
    {
        await CaptureWithHiddenWindowAsync(() => _screenshotService.CaptureFullScreen());
    }

    private async void TargetWindowCaptureButton_Click(object sender, RoutedEventArgs e)
    {
        await CaptureWithHiddenWindowAsync(() => _screenshotService.CaptureWindowFromScreen(_targetWindow));
    }

    private void PrintWindowCaptureButton_Click(object sender, RoutedEventArgs e)
    {
        SetScreenshotAttachment(_screenshotService.CaptureWindowWithPrintWindow(_targetWindow));
    }

    private async Task CaptureWithHiddenWindowAsync(Func<ScreenshotResult> capture)
    {
        try
        {
            Topmost = false;
            Hide();
            await Task.Delay(160);
            SetScreenshotAttachment(capture());
        }
        finally
        {
            Show();
            Topmost = true;
        }
    }

    private void SetScreenshotAttachment(ScreenshotResult result)
    {
        if (!result.HasImage)
        {
            _currentAttachment = null;
            AttachmentStatusText.Text = result.Message;
            return;
        }

        _currentAttachment = new AiMessageAttachment("screenshot.png", "image/png", result.ToDataUrl());
        AttachmentStatusText.Text = $"{result.StrategyName}已添加：{result.Width} x {result.Height}";

        string marker = $"[已附加截图：{result.StrategyName} {result.Width} x {result.Height}]";
        if (!AiPromptBox.Text.Contains(marker, StringComparison.Ordinal))
        {
            AiPromptBox.AppendText((AiPromptBox.Text.Length > 0 ? Environment.NewLine : "") + marker);
        }
    }

    private async void AiSendButton_Click(object sender, RoutedEventArgs e)
    {
        string prompt = AiPromptBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(prompt) && _currentAttachment is null)
        {
            SetAiAnswerMarkdown("请输入问题，或先添加一张截图。");
            return;
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            prompt = "请分析这张截图。";
        }

        AiSendButton.IsEnabled = false;
        AiSendButton.Content = "AI 回复中...";
        SetAiAnswerMarkdown("正在请求 AI...");
        _aiCts = new CancellationTokenSource();

        try
        {
            var attachments = _currentAttachment is null
                ? Array.Empty<AiMessageAttachment>()
                : [_currentAttachment];
            SetAiAnswerMarkdown(await _aiClient.SendAsync(prompt, attachments, _aiCts.Token));
        }
        catch (OperationCanceledException)
        {
            SetAiAnswerMarkdown("AI 请求已取消。");
        }
        catch (Exception ex)
        {
            SetAiAnswerMarkdown($"AI 请求失败：{ex.Message}");
        }
        finally
        {
            _aiCts = null;
            AiSendButton.IsEnabled = true;
            AiSendButton.Content = "发送给 AI";
        }
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var text = InputBox.Text;
        if (string.IsNullOrEmpty(text)) return;
        if (_targetWindow == nint.Zero)
        {
            MessageBox.Show("未检测到目标窗口，请先点击目标输入框再切回本窗口。",
                "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int delayMs = (int)SpeedSlider.Value;
        SendButton.IsEnabled = false;
        SendButton.Content = "发送中...";

        _cts = new CancellationTokenSource();

        try
        {
            // Hide ourselves so we don't interfere with focus
            Topmost = false;
            Hide();
            await Task.Delay(100);

            // Switch to target window
            FocusTargetWindow();
            await Task.Delay(300);

            // Disable IME on the target to prevent it from eating Unicode input
            DisableTargetIME();
            await Task.Delay(50);

            // Send text
            await InputSimulator.SendTextAsync(text, delayMs, _cts.Token);

            // Restore IME
            RestoreTargetIME();
        }
        catch (OperationCanceledException)
        {
            RestoreTargetIME();
        }
        finally
        {
            _cts = null;
            Show();
            Topmost = true;
            SendButton.IsEnabled = true;
            SendButton.Content = "发 送";
        }
    }

    private nint _focusedControl;

    private void DisableTargetIME()
    {
        // Get the actual focused control within the target window
        uint targetThreadId = User32.GetWindowThreadProcessId(_targetWindow, out _);
        uint currentThreadId = User32.GetCurrentThreadId();

        User32.AttachThreadInput(currentThreadId, targetThreadId, true);
        _focusedControl = User32.GetFocus();
        User32.AttachThreadInput(currentThreadId, targetThreadId, false);

        if (_focusedControl == nint.Zero)
            _focusedControl = _targetWindow;

        // Get and disable IME
        var imc = Imm32.ImmGetContext(_focusedControl);
        if (imc != nint.Zero)
        {
            Imm32.ImmSetOpenStatus(imc, false);
            Imm32.ImmReleaseContext(_focusedControl, imc);
        }
    }

    private void RestoreTargetIME()
    {
        if (_focusedControl == nint.Zero) return;

        var imc = Imm32.ImmGetContext(_focusedControl);
        if (imc != nint.Zero)
        {
            Imm32.ImmSetOpenStatus(imc, true);
            Imm32.ImmReleaseContext(_focusedControl, imc);
        }
    }

    private void FocusTargetWindow()
    {
        uint targetThreadId = User32.GetWindowThreadProcessId(_targetWindow, out _);
        uint currentThreadId = User32.GetCurrentThreadId();

        User32.AttachThreadInput(currentThreadId, targetThreadId, true);
        User32.SetForegroundWindow(_targetWindow);
        User32.BringWindowToTop(_targetWindow);
        User32.AttachThreadInput(currentThreadId, targetThreadId, false);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape && _cts != null)
        {
            _cts.Cancel();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape && _aiCts != null)
        {
            _aiCts.Cancel();
            e.Handled = true;
        }
        base.OnKeyDown(e);
    }
}
