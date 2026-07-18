# GhostPaste AI Experiment

GhostPaste AI Experiment is an experimental Windows floating utility built from GhostPaste. It keeps the original simulated keyboard paste workflow, then adds AI chat, screenshot-assisted prompts, Markdown rendering, image cropping, and a record board for text/image notes.

This repository is the standalone experimental project:

https://github.com/dusk2999/GhostPaste-ai-experiment

## Status

This is an experimental branch project. The original GhostPaste paste-simulation workflow is preserved, while the AI and screenshot features are being tested in this separate project.

## Features

- Simulated keyboard paste: sends text by emulating real keystrokes instead of relying on Ctrl+V.
- Always-on-top floating window: designed to stay visible above the target app.
- Transparent UI: opacity slider can make the window background nearly invisible while keeping controls usable.
- Adjustable typing speed: supports fast sending or slower human-like input.
- Target window tracking: remembers the most recently focused non-GhostPaste window as the send target.
- AI chat: sends prompts to an OpenAI-compatible Responses API endpoint.
- Runtime AI settings: configure API base URL and key in the app UI. They are saved locally and are not hardcoded in source.
- Screenshot prompts: supports full-screen capture, target-window capture, and PrintWindow capture.
- Image preview and crop: preview the attached screenshot and crop it before sending to AI.
- Markdown output: AI answers are rendered as Markdown inside the app.
- Record board: add, view, reuse, and copy text/image records.
- Built-in study records: includes 13 computer-network exercise records for quick review.
- Single-file packaging: publishes as one self-contained Windows exe.

## AI Configuration

Open the `AI问答` tab and expand `AI设置`.

Fill in:

- `地址`: the API base URL, for example `http://example.com/v1`
- `密钥`: your API key

The app calls the `/responses` endpoint under that base URL. The default model is `gpt-5.5-fast`, and the default reasoning effort is `xhigh`.

Settings are saved outside the repository:

```text
%AppData%\GhostPaste\ai-settings.json
```

No API key or default service address is stored in the tracked source code.

## Screenshot Strategies

The AI tab provides multiple screenshot options:

- `全屏截图`: captures the whole desktop after briefly hiding the GhostPaste window.
- `目标窗口截图`: captures the last tracked target window from the screen.
- `PrintWindow截图`: asks Windows to render the target window through PrintWindow, which can work better for some covered or inactive windows.

Some protected content may still appear blank or unavailable. In that case, the app reports the capture failure instead of pretending the image was captured.

## Usage

1. Run `GhostPaste.exe`.
2. For paste simulation, click the target input field in another app, return to GhostPaste, enter text, adjust speed, and click `发送`.
3. For AI chat, open `AI问答`, configure the API address and key, enter a question, optionally attach a screenshot, and click `发送给 AI`.
4. Use `裁切` to crop the current screenshot before sending.
5. Use `记录板` to store useful text and images, copy them later, or fill them back into the AI prompt.
6. Press `Esc` while sending text or waiting for AI to cancel the current operation.

## Build

The project is a .NET 8 WPF app for Windows.

Run from the project root:

```powershell
dotnet restore
dotnet test .\GhostPaste.Tests\GhostPaste.Tests.csproj
dotnet publish .\GhostPaste.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\publish_out
```

The generated exe will be:

```text
publish_out\GhostPaste.exe
```

## Repository Notes

- The original project repository is `dusk2999/GhostPaste`.
- This standalone experiment repository is `dusk2999/GhostPaste-ai-experiment`.
- The app does not include a baked-in API key or hardcoded AI service address.
- Local build outputs are ignored by Git.
