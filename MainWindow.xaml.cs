using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GhostPaste.Helpers;
using GhostPaste.Native;

namespace GhostPaste;

public partial class MainWindow : Window
{
    private nint _targetWindow;
    private nint _selfHandle;
    private CancellationTokenSource? _cts;
    private readonly DispatcherTimer _tracker;

    public MainWindow()
    {
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;

        // Poll foreground window every 300ms to track the last non-self window
        _tracker = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
        _tracker.Tick += Tracker_Tick;
        _tracker.Start();
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        _selfHandle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        GlassHelper.Apply(this);
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
        base.OnKeyDown(e);
    }
}
