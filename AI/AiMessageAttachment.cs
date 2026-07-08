namespace GhostPaste.AI;

public sealed record AiMessageAttachment(
    string FileName,
    string MediaType,
    string DataUrl);
