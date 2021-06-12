using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace EmojiShortcutApp
{
    public class WindowHookEvent: EventArgs
    {
        public IntPtr Handle;
    }

    public class WindowHook : IDisposable
    {
        private IntPtr CurrentWindow;
        private IntPtr ActiveWindow;
        private Timer Timer;

        public event EventHandler<WindowHookEvent> Changed;

        public WindowHook(IntPtr currentWindow)
        {
            Debug.WriteLine(currentWindow);

            ActiveWindow = IntPtr.Zero;
            CurrentWindow = currentWindow;

            Timer = new Timer();
            Timer.Interval = 100;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var handle = GetForegroundWindow();
            if (!handle.Equals(ActiveWindow) && !handle.Equals(CurrentWindow))
            {
                Debug.WriteLine("Window Changed: {0}", handle);
                ActiveWindow = handle;
                Changed?.Invoke(this, new WindowHookEvent() { Handle = ActiveWindow });
            }
        }

        public void Dispose()
        {
            Timer.Stop();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

    }
}
