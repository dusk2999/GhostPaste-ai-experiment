using System.Runtime.InteropServices;

namespace GhostPaste.Native;

/// <summary>
/// Simulates keyboard input via SendInput Unicode events.
/// </summary>
internal static class InputSimulator
{
    /// <summary>
    /// Sends text character by character with the specified delay between each.
    /// If delayMs is 0, sends all characters in a single batch call.
    /// </summary>
    public static async Task SendTextAsync(string text, int delayMs, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(text)) return;

        if (delayMs <= 0)
        {
            // Batch mode: send everything in one SendInput call
            SendBatch(text);
            return;
        }

        // Character-by-character with delay
        for (int i = 0; i < text.Length; i++)
        {
            ct.ThrowIfCancellationRequested();

            char c = text[i];

            if (c == '\r') continue; // skip \r, handle \n as Enter

            if (c == '\n')
            {
                SendVirtualKey(0x0D); // VK_RETURN
            }
            else if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
            {
                // Surrogate pair (emoji etc.)
                SendUnicode(c);
                i++;
                SendUnicode(text[i]);
            }
            else
            {
                SendUnicode(c);
            }

            await Task.Delay(delayMs, ct);
        }
    }

    private static void SendBatch(string text)
    {
        var inputs = new List<INPUT>();

        foreach (char c in text)
        {
            if (c == '\r') continue;

            if (c == '\n')
            {
                // VK_RETURN key down + up
                inputs.Add(MakeVkInput(0x0D, false));
                inputs.Add(MakeVkInput(0x0D, true));
            }
            else
            {
                inputs.Add(MakeUnicodeInput(c, false));
                inputs.Add(MakeUnicodeInput(c, true));
            }
        }

        if (inputs.Count > 0)
        {
            var arr = inputs.ToArray();
            User32.SendInput((uint)arr.Length, arr, Marshal.SizeOf<INPUT>());
        }
    }

    private static void SendUnicode(char c)
    {
        var inputs = new INPUT[]
        {
            MakeUnicodeInput(c, false),
            MakeUnicodeInput(c, true),
        };
        User32.SendInput(2, inputs, Marshal.SizeOf<INPUT>());
    }

    private static void SendVirtualKey(ushort vk)
    {
        var inputs = new INPUT[]
        {
            MakeVkInput(vk, false),
            MakeVkInput(vk, true),
        };
        User32.SendInput(2, inputs, Marshal.SizeOf<INPUT>());
    }

    private static INPUT MakeUnicodeInput(char c, bool keyUp) => new()
    {
        Type = INPUT.KEYBOARD,
        U = new INPUTUNION
        {
            Ki = new KEYBDINPUT
            {
                Vk = 0,
                Scan = c,
                Flags = KEYBDINPUT.KEYEVENTF_UNICODE | (keyUp ? KEYBDINPUT.KEYEVENTF_KEYUP : 0),
            }
        }
    };

    private static INPUT MakeVkInput(ushort vk, bool keyUp) => new()
    {
        Type = INPUT.KEYBOARD,
        U = new INPUTUNION
        {
            Ki = new KEYBDINPUT
            {
                Vk = vk,
                Scan = 0,
                Flags = keyUp ? KEYBDINPUT.KEYEVENTF_KEYUP : 0,
            }
        }
    };
}
