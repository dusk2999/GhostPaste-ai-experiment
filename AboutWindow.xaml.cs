using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GhostPaste.Helpers;

namespace GhostPaste;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        GitHubLink.Text = AppInfo.GitHubUrl;
        CreditLine.Text = AppInfo.CreditLine;
        SourceInitialized += (_, _) => GlassHelper.Apply(this);
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OpenGitHub_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = AppInfo.GitHubUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            MessageBox.Show(AppInfo.GitHubUrl, "GitHub", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
