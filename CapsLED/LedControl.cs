using System;
using System.Runtime.InteropServices;

// https://stackoverflow.com/questions/2248358/way-to-turn-on-keyboards-caps-lock-light-without-actually-turning-on-caps-lock/76222691
// https://www.giorgi.dev/miscellaneous/faking-num-lock-caps-lock-and-scroll-lock-leds/
// https://stackoverflow.com/questions/5375268/how-to-set-leds-of-a-usb-keyboard-under-windows

namespace CapsLED
{
    internal class LedControl
    {
        UInt32 bytesReturned = 0;
        IntPtr device;
        NativeMethods.KEYBOARD_INDICATOR_PARAMETERS KIPbuf = new NativeMethods.KEYBOARD_INDICATOR_PARAMETERS { unitID = 0, LEDflags = 0 };

        class Flags
        {
            public static IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
            public const UInt32 IOCTL_KEYBOARD_SET_INDICATORS   = (0x0000000b << 16) | (0 << 14) | (0x0002 << 2) | 0; // from ntddkbd.h, ntddk.h
            public const UInt32 IOCTL_KEYBOARD_QUERY_INDICATORS = (0x0000000b << 16) | (0 << 14) | (0x0010 << 2) | 0; // from ntddkbd.h, ntddk.h

            public const UInt32 DDD_RAW_TARGET_PATH       = 0x00000001;
            public const UInt32 DDD_REMOVE_DEFINITION     = 0x00000002;
//          public const UInt32 DDD_EXACT_MATCH_ON_REMOVE = 0x00000004;
//          public const UInt32 DDD_NO_BROADCAST_SYSTEM   = 0x00000008;

//          public const UInt32 GENERIC_ALL     = 0x10000000;
//          public const UInt32 GENERIC_EXECUTE = 0x20000000;
            public const UInt32 GENERIC_WRITE   = 0x40000000;
//          public const UInt32 GENERIC_READ    = 0x80000000;

//          public const UInt32 CREATE_NEW        = 1;
//          public const UInt32 CREATE_ALWAYS     = 2;
            public const UInt32 OPEN_EXISTING     = 3;
//          public const UInt32 OPEN_ALWAYS       = 4;
//          public const UInt32 TRUNCATE_EXISTING = 5;

//          public const UInt16 KEYBOARD_SCROLL_LOCK_ON = 1;
//          public const UInt16 KEYBOARD_NUM_LOCK_ON    = 2;
            public const UInt16 KEYBOARD_CAPS_LOCK_ON   = 4;
//          public const UInt16 KEYBOARD_SHADOW         = 0x4000;
//          public const UInt16 KEYBOARD_LED_INJECTED   = 0x8000;
        };


        public int OpenDevice()
        {
            int ret = -1;
            if (NativeMethods.DefineDosDevice(Flags.DDD_RAW_TARGET_PATH, "myKBD", "\\Device\\KeyboardClass0"))
            {
                device = NativeMethods.CreateFile("\\\\.\\myKBD", Flags.GENERIC_WRITE, 0, IntPtr.Zero, Flags.OPEN_EXISTING, 0, IntPtr.Zero);
                if (device == Flags.INVALID_HANDLE_VALUE)
                {
                    ret = -2;
                } else {
                    if (!NativeMethods.DeviceIoControl(device, Flags.IOCTL_KEYBOARD_QUERY_INDICATORS, IntPtr.Zero, 0, ref KIPbuf, (UInt32)Marshal.SizeOf(KIPbuf), ref bytesReturned, IntPtr.Zero))
                    {
                        ret = -3;
                    } else {
                        return ((KIPbuf.LEDflags & Flags.KEYBOARD_CAPS_LOCK_ON) != 0) ? 1 : 0;
                    }
                    NativeMethods.CloseHandle(device);
                }
                NativeMethods.DefineDosDevice(Flags.DDD_REMOVE_DEFINITION, "myKBD", null);
            }
            return ret;
        }

        public int SetLED(bool flag)
        {
            if (!NativeMethods.DeviceIoControl(device, Flags.IOCTL_KEYBOARD_QUERY_INDICATORS, IntPtr.Zero, 0, ref KIPbuf, (UInt32)Marshal.SizeOf(KIPbuf), ref bytesReturned, IntPtr.Zero)) return -1;

            if (flag)
            {
                KIPbuf.LEDflags = (UInt16)(KIPbuf.LEDflags | Flags.KEYBOARD_CAPS_LOCK_ON);
            } else {
                KIPbuf.LEDflags = (UInt16)(KIPbuf.LEDflags & ~Flags.KEYBOARD_CAPS_LOCK_ON);
            }

            if (!NativeMethods.DeviceIoControl(device, Flags.IOCTL_KEYBOARD_SET_INDICATORS, ref KIPbuf, (UInt32)Marshal.SizeOf(KIPbuf), IntPtr.Zero, 0, ref bytesReturned, IntPtr.Zero)) return -2;
            return 0;
        }

        public void Close()
        {
            NativeMethods.CloseHandle(device);
            NativeMethods.DefineDosDevice(Flags.DDD_REMOVE_DEFINITION, "myKBD", null);
        }
    }
}
