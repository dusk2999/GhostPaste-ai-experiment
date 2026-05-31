# About Page and GitHub Publishing Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a polished About dialog with a GitHub link and `Made by dusk`, then publish GhostPaste to a public GitHub repository.

**Architecture:** Keep stable About metadata in a small `AppInfo` class so it can be tested without driving WPF UI automation. Add a dedicated WPF `AboutWindow` for the visible page and wire it from the existing custom title bar.

**Tech Stack:** .NET 8 WPF, MSTest, Git, GitHub CLI.

---

### Task 1: Repository Hygiene and Initial Import

**Files:**
- Create: `.gitignore`
- Track existing project source and docs.

- [ ] **Step 1: Add generated-output ignores**

Create `.gitignore` with:

```gitignore
bin/
obj/
publish/
publish_out/

*.user
*.suo
*.rsuser
.vs/

TestResults/
*.nupkg
```

- [ ] **Step 2: Commit the initial source import**

Run:

```powershell
git add .gitignore App.xaml App.xaml.cs AssemblyInfo.cs GhostPaste.csproj MainWindow.xaml MainWindow.xaml.cs Native Helpers docs README.md
git commit -m "chore: import GhostPaste source"
```

Expected: a root commit on `main` containing source and documentation, without build output.

### Task 2: Test the About Metadata

**Files:**
- Create: `GhostPaste.Tests/GhostPaste.Tests.csproj`
- Create: `GhostPaste.Tests/AppInfoTests.cs`

- [ ] **Step 1: Create the test project**

Run:

```powershell
dotnet new mstest -n GhostPaste.Tests
dotnet add GhostPaste.Tests/GhostPaste.Tests.csproj reference GhostPaste.csproj
```

- [ ] **Step 2: Replace the default test with failing metadata tests**

Use this content in `GhostPaste.Tests/AppInfoTests.cs`:

```csharp
using GhostPaste;

namespace GhostPaste.Tests;

[TestClass]
public sealed class AppInfoTests
{
    [TestMethod]
    public void GitHubUrlPointsToPublicRepository()
    {
        Assert.AreEqual("https://github.com/dusk2999/GhostPaste", AppInfo.GitHubUrl);
    }

    [TestMethod]
    public void CreditLineUsesEnglishDuskCredit()
    {
        Assert.AreEqual("Made by dusk", AppInfo.CreditLine);
    }
}
```

- [ ] **Step 3: Run tests to verify RED**

Run:

```powershell
dotnet test GhostPaste.Tests/GhostPaste.Tests.csproj
```

Expected: compile failure because `AppInfo` does not exist yet.

### Task 3: Add About Metadata

**Files:**
- Create: `AppInfo.cs`

- [ ] **Step 1: Add minimal implementation**

Create `AppInfo.cs`:

```csharp
namespace GhostPaste;

public static class AppInfo
{
    public static string GitHubUrl => "https://github.com/dusk2999/GhostPaste";
    public static string CreditLine => "Made by dusk";
}
```

- [ ] **Step 2: Run tests to verify GREEN**

Run:

```powershell
dotnet test GhostPaste.Tests/GhostPaste.Tests.csproj
```

Expected: all tests pass.

### Task 4: Add About Dialog UI

**Files:**
- Create: `AboutWindow.xaml`
- Create: `AboutWindow.xaml.cs`
- Modify: `MainWindow.xaml`
- Modify: `MainWindow.xaml.cs`

- [ ] **Step 1: Create `AboutWindow.xaml`**

Create a small WPF window using the existing light glass visual language. It must include `GhostPaste`, the GitHub link, `Made by dusk`, and a close button.

- [ ] **Step 2: Create `AboutWindow.xaml.cs`**

Create the code-behind with:

```csharp
using System.Diagnostics;
using System.Windows;
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

    private void OpenGitHub_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppInfo.GitHubUrl,
            UseShellExecute = true
        });
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
```

- [ ] **Step 3: Wire the main title bar**

Add a `?` button beside the close button in `MainWindow.xaml`, using the existing close-button style for consistent sizing.

- [ ] **Step 4: Open the dialog from main window**

Add `AboutButton_Click` in `MainWindow.xaml.cs`:

```csharp
private void AboutButton_Click(object sender, RoutedEventArgs e)
{
    var aboutWindow = new AboutWindow
    {
        Owner = this
    };
    aboutWindow.ShowDialog();
}
```

- [ ] **Step 5: Verify build**

Run:

```powershell
dotnet build GhostPaste.csproj
```

Expected: build succeeds with no errors.

### Task 5: Publish to GitHub

**Files:**
- Git metadata only.

- [ ] **Step 1: Commit feature work**

Run:

```powershell
git add AppInfo.cs AboutWindow.xaml AboutWindow.xaml.cs MainWindow.xaml MainWindow.xaml.cs GhostPaste.Tests docs/superpowers
git commit -m "feat: add about page"
```

- [ ] **Step 2: Create public repository and push**

Run:

```powershell
gh repo create dusk2999/GhostPaste --public --source . --remote origin --push
```

Expected: GitHub repository exists at `https://github.com/dusk2999/GhostPaste`, and branch `main` is pushed.
