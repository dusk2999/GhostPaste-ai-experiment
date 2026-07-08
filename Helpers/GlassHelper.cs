using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace GhostPaste.Helpers;

/// <summary>
/// Applies pure Gaussian blur / Acrylic backdrop to a WPF layered window.
/// </summary>
internal static class GlassHelper
{
    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(nint hwnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public int Attribute;
        public nint Data;
        public int SizeOfData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public int AccentState;
        public int AccentFlags;
        public uint GradientColor;
        public int AnimationId;
    }

    public static void Apply(Window window)
    {
        SetAcrylicEnabled(window, true);
    }

    public static void SetAcrylicEnabled(Window window, bool enabled)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();

        var accent = new AccentPolicy
        {
            AccentState = enabled ? 4 : 0, // ACCENT_ENABLE_ACRYLICBLURBEHIND / ACCENT_DISABLED
            AccentFlags = 0,
            GradientColor = enabled ? 0x20FFFFFFu : 0x00000000u, // AABBGGRR
            AnimationId = 0
        };

        int accentStructSize = Marshal.SizeOf(accent);
        nint accentPtr = Marshal.AllocHGlobal(accentStructSize);
        try
        {
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = 19, // WCA_ACCENT_POLICY
                SizeOfData = accentStructSize,
                Data = accentPtr
            };
            SetWindowCompositionAttribute(hwnd, ref data);
        }
        finally
        {
            Marshal.FreeHGlobal(accentPtr);
        }
    }
}
