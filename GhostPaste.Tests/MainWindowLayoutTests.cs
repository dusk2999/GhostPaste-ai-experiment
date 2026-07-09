namespace GhostPaste.Tests;

[TestClass]
public sealed class MainWindowLayoutTests
{
    [TestMethod]
    public void MainWindowContainsAiChatTabAndScreenshotControls()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));

        StringAssert.Contains(xaml, "Header=\"AI问答\"");
        StringAssert.Contains(xaml, "x:Name=\"AiPromptBox\"");
        StringAssert.Contains(xaml, "x:Name=\"AiSettingsExpander\"");
        StringAssert.Contains(xaml, "x:Name=\"AiEndpointBox\"");
        StringAssert.Contains(xaml, "x:Name=\"AiKeyBox\"");
        StringAssert.Contains(xaml, "x:Name=\"SaveAiSettingsButton\"");
        StringAssert.Contains(xaml, "Click=\"SaveAiSettingsButton_Click\"");
        StringAssert.Contains(xaml, "x:Name=\"FullScreenCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"TargetWindowCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"PrintWindowCaptureButton\"");
        StringAssert.Contains(xaml, "x:Name=\"AiSendButton\"");
    }

    [TestMethod]
    public void MainWindowUsesTransparentResourceBrushesForInteractiveControls()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));

        StringAssert.Contains(xaml, "x:Key=\"ChromeSurfaceBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"PanelSurfaceBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"AnswerSurfaceBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"ControlSurfaceBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"ControlHoverBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"PrimaryButtonBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"PrimaryButtonTextBrush\"");
        StringAssert.Contains(xaml, "x:Key=\"PrimaryButtonBorderBrush\"");
        StringAssert.Contains(xaml, "Background=\"{DynamicResource ControlSurfaceBrush}\"");
        StringAssert.Contains(xaml, "Background=\"{DynamicResource PrimaryButtonBrush}\"");
        StringAssert.Contains(xaml, "Foreground=\"{DynamicResource PrimaryButtonTextBrush}\"");
        StringAssert.Contains(xaml, "BorderBrush=\"{DynamicResource PrimaryButtonBorderBrush}\"");
        Assert.IsFalse(xaml.Contains("{StaticResource PanelSurfaceBrush}", StringComparison.Ordinal));
        Assert.IsFalse(xaml.Contains("{StaticResource ControlSurfaceBrush}", StringComparison.Ordinal));
        Assert.IsFalse(xaml.Contains("{StaticResource PrimaryButtonBrush}", StringComparison.Ordinal));
        StringAssert.Contains(xaml, "Style=\"{StaticResource TransparentTabItemStyle}\"");
    }

    [TestMethod]
    public void MainWindowUsesMarkdownViewerForAiOutput()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));
        string code = File.ReadAllText(FindProjectFile("MainWindow.xaml.cs"));

        StringAssert.Contains(xaml, "<RichTextBox");
        StringAssert.Contains(xaml, "x:Name=\"AiMarkdownViewer\"");
        StringAssert.Contains(xaml, "IsDocumentEnabled=\"True\"");
        Assert.IsFalse(xaml.Contains("x:Name=\"AiAnswerBox\"", StringComparison.Ordinal));

        StringAssert.Contains(code, "MarkdownFlowDocumentRenderer");
        StringAssert.Contains(code, "SetAiAnswerMarkdown");
        StringAssert.Contains(code, "AiMarkdownViewer.Document");
    }

    [TestMethod]
    public void MainWindowContainsAttachmentPreviewAndCropControls()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));
        string code = File.ReadAllText(FindProjectFile("MainWindow.xaml.cs"));

        StringAssert.Contains(xaml, "x:Name=\"AttachmentPreviewPanel\"");
        StringAssert.Contains(xaml, "x:Name=\"AttachmentPreviewImage\"");
        StringAssert.Contains(xaml, "x:Name=\"CropAttachmentButton\"");
        StringAssert.Contains(xaml, "Click=\"CropAttachmentButton_Click\"");
        StringAssert.Contains(xaml, "x:Name=\"ClearAttachmentButton\"");
        StringAssert.Contains(code, "UpdateAttachmentPreview");
        StringAssert.Contains(code, "ImageCropWindow");
    }

    [TestMethod]
    public void MainWindowContainsRecordBoardControls()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));
        string code = File.ReadAllText(FindProjectFile("MainWindow.xaml.cs"));

        StringAssert.Contains(xaml, "Header=\"记录板\"");
        StringAssert.Contains(xaml, "x:Name=\"BoardInputBox\"");
        StringAssert.Contains(xaml, "x:Name=\"AddBoardRecordButton\"");
        StringAssert.Contains(xaml, "x:Name=\"PasteClipboardImageButton\"");
        StringAssert.Contains(xaml, "x:Name=\"BoardRecordsList\"");
        StringAssert.Contains(xaml, "Click=\"CopyBoardTextButton_Click\"");
        StringAssert.Contains(xaml, "Click=\"CopyBoardImageButton_Click\"");
        StringAssert.Contains(xaml, "Click=\"UseBoardRecordForAiButton_Click\"");
        StringAssert.Contains(code, "RecordBoard");
        StringAssert.Contains(code, "BuiltInExerciseRecords.Seed");
        StringAssert.Contains(code, "PasteClipboardImageButton_Click");
        StringAssert.Contains(code, "TryCreateAttachmentFromClipboardImage");
        StringAssert.Contains(code, "RefreshBoardRecords");
    }

    [TestMethod]
    public void MainWindowContainsTransparencySlider()
    {
        string xaml = File.ReadAllText(FindProjectFile("MainWindow.xaml"));

        StringAssert.Contains(xaml, "x:Name=\"WindowChromeBorder\"");
        StringAssert.Contains(xaml, "x:Name=\"UiOpacitySlider\"");
        StringAssert.Contains(xaml, "ValueChanged=\"UiOpacitySlider_ValueChanged\"");
        StringAssert.Contains(xaml, "完全透明");
    }

    private static string FindProjectFile(string fileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(directory.FullName, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        Assert.Fail($"Could not find {fileName}.");
        throw new InvalidOperationException();
    }
}
