using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using Indieteur.GlobalHooks;

namespace EmojiShortcutApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        // https://www.webfx.com/tools/emoji-cheat-sheet/

        public MainWindow()
        {
            InitializeComponent();

            var keyHook = new GlobalKeyHook();
            keyHook.OnKeyDown += KeyHook_OnKeyDown;
            keyHook.OnKeyUp += KeyHook_OnKeyUp;

            var caretTimer = new Timer();
            caretTimer.Elapsed += CaretTimer_Elapsed;
            caretTimer.Interval = 100;
            caretTimer.Start();
        }
        
        private void CaretTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var caretPosition = new Point();
            var success = CaretHook.GetCaretPosition(ref caretPosition);

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                if (!success)
                {
                    double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                    double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                    double windowWidth = Width;
                    double windowHeight = Height;

                    caretPosition.X = (int)((screenWidth / 2) - (windowWidth / 2));
                    caretPosition.Y = (int)((screenHeight / 2) - (windowHeight / 2));
                }

                Left = caretPosition.X;
                Top = caretPosition.Y + 10;
            }));
        }

        private void KeyHook_OnKeyUp(object sender, GlobalKeyEventArgs e)
        {
            Debug.WriteLine("KeyHook_OnKeyUp: {0} ({1})", e.CharResult, e.KeyCode);
            e.Handled = false;
        }

        private void KeyHook_OnKeyDown(object sender, GlobalKeyEventArgs e)
        {
            Debug.WriteLine("KeyHook_OnKeyDown: {0} ({1})", e.CharResult, e.KeyCode);
            e.Handled = false;
        }
    }
}
