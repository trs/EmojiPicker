using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace EmojiShortcutApp
{
    public class CaretHook
    {
        [DllImport("user32.dll")]
        static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        public struct GUITHREADINFO
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

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        public static bool GetCaretPosition(ref Point caretPoint)
        {
            GUITHREADINFO gti = default(GUITHREADINFO);
            gti.cbSize = Marshal.SizeOf(typeof(GUITHREADINFO));

            if (!GetGUIThreadInfo(0, ref gti))
            {
                //Debug.WriteLine("Failed to get GUI Thread Info");
                return false;
            }

            //Debug.WriteLine(gti.rcCaret); // The position of the caret in the parent, 
                                            // if the active control is a text box or similar

            var point = gti.rcCaret.Location;
            if (!ClientToScreen(gti.hwndCaret, ref point))
            {
                //Debug.WriteLine("Failed to get relative coordinates");
                return false;
            }

            caretPoint = point;

            //Debug.WriteLine(point); // The position of the caret in screen-coördinates

            return true;
        }
    }
}
