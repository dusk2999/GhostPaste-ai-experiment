# AI Chat Screenshot Experiment Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an experimental AI chat tab with screenshot attachments to GhostPaste while keeping secrets and the original `main` branch safe.

**Architecture:** Use small services for configuration, Responses request building/parsing, screenshots, and UI orchestration. Keep the original paste-simulation flow in the same window under a `粘贴发送` tab, and add AI controls under an `AI问答` tab.

**Tech Stack:** .NET 8 WPF, MSTest, `System.Net.Http.Json`, `System.Drawing.Common` on Windows, Win32 interop.

## Global Constraints

- Experimental branch: `experiment/ai-chat-screenshot`.
- Base URL: `http://49.51.186.85/v1`.
- Endpoint: `/responses`.
- Default model: `gpt-5.5-fast`.
- Default reasoning effort: `xhigh`.
- Local key file: `LocalSecrets.g.cs`, ignored by Git and not committed.
- Public GitHub releases must not include an executable containing the local key.

---

### Task 1: Secret Guardrails

**Files:**
- Modify: `.gitignore`
- Create: `LocalSecrets.cs`
- Create: `LocalSecrets.example.txt`
- Local only: `LocalSecrets.g.cs`

**Interfaces:**
- Produces: `LocalSecrets.ApiKey`, available only in local ignored builds.

- [ ] Add `LocalSecrets.g.cs` to `.gitignore`.
- [ ] Add `LocalSecrets.cs` with an empty public-safe default.
- [ ] Add `LocalSecrets.example.txt` with the partial override pattern.
- [ ] Create local ignored `LocalSecrets.g.cs` with the provided key.
- [ ] Verify `git check-ignore -v LocalSecrets.g.cs` reports the ignore rule.

### Task 2: Responses API Layer

**Files:**
- Create: `AI/AiSettings.cs`
- Create: `AI/AiMessageAttachment.cs`
- Create: `AI/ResponsesRequestBuilder.cs`
- Create: `AI/ResponsesResponseParser.cs`
- Create: `AI/ResponsesAiClient.cs`
- Test: `GhostPaste.Tests/AI/ResponsesRequestBuilderTests.cs`
- Test: `GhostPaste.Tests/AI/ResponsesResponseParserTests.cs`

**Interfaces:**
- Produces: `AiSettings.Default`, `ResponsesRequestBuilder.Build(...)`, `ResponsesResponseParser.ExtractText(...)`, `ResponsesAiClient.SendAsync(...)`.

- [ ] Write failing tests for defaults, text-only JSON, image JSON, and parser fallbacks.
- [ ] Implement the settings, request builder, parser, and HTTP client.
- [ ] Run `dotnet test GhostPaste.Tests\GhostPaste.Tests.csproj`.

### Task 3: Screenshot Services

**Files:**
- Create: `Screenshots/ScreenshotStrategy.cs`
- Create: `Screenshots/ScreenshotResult.cs`
- Create: `Screenshots/ScreenshotService.cs`
- Modify: `Native/User32.cs`
- Test: `GhostPaste.Tests/Screenshots/ScreenshotResultTests.cs`

**Interfaces:**
- Produces: `ScreenshotService.CaptureFullScreen()`, `CaptureWindowFromScreen(nint)`, `CaptureWindowWithPrintWindow(nint)`.

- [ ] Write failing tests for PNG data URL conversion and empty capture result behavior.
- [ ] Implement capture strategies with Win32 bounds and `PrintWindow`.
- [ ] Run `dotnet test GhostPaste.Tests\GhostPaste.Tests.csproj`.

### Task 4: AI UI Integration

**Files:**
- Modify: `MainWindow.xaml`
- Modify: `MainWindow.xaml.cs`

**Interfaces:**
- Consumes: `ResponsesAiClient`, `ScreenshotService`, `ScreenshotResult`.

- [ ] Move existing paste UI into a `TabControl` tab named `粘贴发送`.
- [ ] Add `AI问答` tab with prompt box, answer box, screenshot strategy buttons, attachment status, and send button.
- [ ] Wire capture buttons to update attachment state and prompt context.
- [ ] Wire AI send button to call `ResponsesAiClient.SendAsync` and display answers/errors.
- [ ] Run `dotnet build GhostPaste.csproj`.

### Task 5: Verification and Local Packaging

**Files:**
- No source files unless verification reveals a defect.

**Interfaces:**
- Produces: local `publish_out\GhostPaste.exe`.

- [ ] Run `dotnet test GhostPaste.Tests\GhostPaste.Tests.csproj`.
- [ ] Run `dotnet build GhostPaste.csproj`.
- [ ] Run `dotnet publish GhostPaste.csproj -c Release -r win-x64 -p:PublishSingleFile=true --self-contained true -o publish_out`.
- [ ] Verify `LocalSecrets.g.cs` remains ignored and untracked.
- [ ] Push `experiment/ai-chat-screenshot` branch only, without creating a public release containing the key.
