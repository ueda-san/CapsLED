using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CapsLED
{
    internal static class NativeMethods
    {
        internal struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public Rectangle rcCaret;
        }

        [Flags]
        internal enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_CONTINUOUS = 0x80000000
        }

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool GetGUIThreadInfo(uint dwthreadid, ref GUITHREADINFO lpguithreadinfo);

        [DllImport("kernel32.dll")]
        internal static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [DllImport("imm32.dll")]
        internal static extern IntPtr ImmGetDefaultIMEWnd(IntPtr hWnd);

        //
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean DefineDosDevice(UInt32 flags, String deviceName, String targetPath);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(String fileName, UInt32 desiredAccess, UInt32 shareMode, IntPtr securityAttributes, UInt32 creationDisposition, UInt32 flagsAndAttributes, IntPtr templateFile);

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBOARD_INDICATOR_PARAMETERS
        {
            public UInt16 unitID;
            public UInt16 LEDflags;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean DeviceIoControl(IntPtr device, UInt32 ioControlCode, ref KEYBOARD_INDICATOR_PARAMETERS KIPin, UInt32 inBufferSize, ref KEYBOARD_INDICATOR_PARAMETERS KIPout, UInt32 outBufferSize, ref UInt32 bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean DeviceIoControl(IntPtr device, UInt32 ioControlCode, IntPtr KIPin, UInt32 inBufferSize, ref KEYBOARD_INDICATOR_PARAMETERS KIPout, UInt32 outBufferSize, ref UInt32 bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean DeviceIoControl(IntPtr device, UInt32 ioControlCode, ref KEYBOARD_INDICATOR_PARAMETERS KIPin, UInt32 inBufferSize, IntPtr KIPout, UInt32 outBufferSize, ref UInt32 bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern Boolean CloseHandle(IntPtr handle);

    }
}
