# AI Chat and Screenshot Experiment Design

## Goal

Build an experimental GhostPaste branch that adds AI question answering and screenshot-assisted prompts while preserving the original `main` branch.

## Branching and Release Safety

All work happens on `experiment/ai-chat-screenshot` in the isolated worktree `D:\test\GhostPaste-ai-experiment`. The public repository must not receive API keys. AI connection details are configured at runtime through the app UI and saved only to the user's local application data folder.

## API Integration

The app sends requests to an OpenAI-compatible Responses endpoint:

- Base URL: entered by the user in the AI settings UI.
- Endpoint: `/responses`
- Default model: `gpt-5.5-fast`
- Default reasoning effort: `xhigh`
- Request format: Responses API-style `input` containing `input_text` and optional `input_image` data URLs.
- Response parsing: prefer `output_text`; fall back to `output[].content[].text` and compatible text fields.

If the service rejects image input or differs from official Responses schema, the app displays a readable error in the AI answer area.

## UI Design

Keep AI tools inside the existing topmost floating window to avoid being hidden by other software. Replace the single text area layout with a compact tabbed interface:

- `粘贴发送`: the existing text simulation workflow.
- `AI问答`: prompt box, screenshot strategy buttons, attachment preview/status, answer area, send button.

The existing about button and close button remain in the custom title bar.

## Screenshot Strategies

The experiment provides several capture strategies for user choice:

- Full screen capture using Windows desktop capture.
- Target window screen capture using the last tracked target window bounds.
- Target window `PrintWindow` capture as an alternate strategy when screen capture is blocked or visually incomplete.

The implementation does not attempt to bypass DRM or protected-content restrictions. If a screenshot is blank, restricted, or unavailable, the app reports that limitation instead of pretending capture succeeded.

## Local Configuration Handling

The source tree does not contain a default API endpoint or key. The app stores the user's endpoint and key in `%AppData%\GhostPaste\ai-settings.json`, which is outside the repository and outside the single-file executable.

## Testing

Unit tests cover:

- Runtime endpoint/key configuration and default model/reasoning values.
- Request JSON shape for text-only and text-plus-image prompts.
- Responses response parsing fallbacks.
- Screenshot result model and data URL conversion.
- Local settings persistence behavior.

WPF visual behavior is verified through `dotnet build` and focused layout tests where feasible.
