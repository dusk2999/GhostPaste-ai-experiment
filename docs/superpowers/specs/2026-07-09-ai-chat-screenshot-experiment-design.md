# AI Chat and Screenshot Experiment Design

## Goal

Build an experimental GhostPaste branch that adds AI question answering and screenshot-assisted prompts while preserving the original `main` branch.

## Branching and Release Safety

All work happens on `experiment/ai-chat-screenshot` in the isolated worktree `D:\test\GhostPaste-ai-experiment`. The public repository must not receive the provided API key. Local builds may embed the key through an ignored generated file named `LocalSecrets.g.cs`; GitHub commits and public releases must not include that file or any executable built with that key.

## API Integration

The app sends requests to an OpenAI-compatible Responses endpoint:

- Base URL: `http://49.51.186.85/v1`
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

## Local Secret Handling

The source tree includes `LocalSecrets.cs` with an empty public-safe default and `LocalSecrets.example.txt` as a template. `.gitignore` ignores `LocalSecrets.g.cs`. The local worktree can contain `LocalSecrets.g.cs` with the provided key so a local exe works immediately, but that file is never committed.

## Testing

Unit tests cover:

- Endpoint/model/default configuration.
- Request JSON shape for text-only and text-plus-image prompts.
- Responses response parsing fallbacks.
- Screenshot result model and data URL conversion.
- Secret template and ignore behavior.

WPF visual behavior is verified through `dotnet build` and focused layout tests where feasible.
