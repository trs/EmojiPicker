using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EmojiShortcutApp
{
    public class KeyboardHook
    {
        private LowLevelKeyboardProc _proc = hookProc;

        private static IntPtr hhook = IntPtr.Zero;

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        private static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                return (IntPtr)1;
            }
            else
                return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}
