namespace GhostPaste;

public static class AppInfo
{
    public static string GitHubUrl => "https://github.com/dusk2999/GhostPaste";
    public static string CreditLine => "Made by dusk";
    public static string Description => "GhostPaste 会把输入框中的内容模拟真实键盘输入，发送到你上次点击过的目标窗口，适合无法直接粘贴的场景。";
    public static IReadOnlyList<string> UsageInstructions { get; } =
    [
        "1. 先点击要接收文本的目标输入框。",
        "2. 回到 GhostPaste 输入内容，并按需要调整速度。",
        "3. 点击发送；发送中可按 Esc 取消。"
    ];
}
