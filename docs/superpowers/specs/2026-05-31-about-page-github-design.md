# About Page and GitHub Publishing Design

## Goal

Publish GhostPaste to a public GitHub repository and add a small About page to the WPF app that includes the repository link and the English credit line `Made by dusk`.

## Current Context

GhostPaste is a .NET 8 WPF floating window app. The main UI is implemented in `MainWindow.xaml` and `MainWindow.xaml.cs`, with a custom transparent title bar and no existing navigation or settings window. The directory was not a Git repository before this work.

## Recommended Approach

Add a compact `?` button to the right side of the existing custom title bar, next to the close button. Clicking it opens a dedicated `AboutWindow` dialog styled to match the light acrylic look of the main window. The dialog will show the app name, the clickable GitHub URL, and `Made by dusk`.

This keeps the main floating tool focused on typing text while making the About information easy to find. A plain message box would be faster but would not provide a polished clickable link. Embedding About content in the main window would consume scarce space in a small utility UI.

## Components

- `AppInfo.cs`: central constants for the GitHub URL and credit line.
- `AboutWindow.xaml`: WPF dialog UI for the About page.
- `AboutWindow.xaml.cs`: opens the GitHub link through the OS shell.
- `MainWindow.xaml`: adds the title-bar About button.
- `MainWindow.xaml.cs`: opens the About dialog.
- `GhostPaste.Tests`: lightweight tests for the stable About metadata.

## GitHub Publishing

Initialize the repository on branch `main`, ignore generated build output, commit the source and feature work, create a public repository under the authenticated GitHub account, and push `main`.

## Testing

Use a small test project to verify the GitHub URL and credit line stay stable. Use `dotnet build` to verify the WPF app compiles after XAML and code-behind changes.
