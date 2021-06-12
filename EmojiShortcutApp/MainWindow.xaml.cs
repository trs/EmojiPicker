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
using System.Runtime.InteropServices;
using Indieteur.GlobalHooks;
using System.Windows.Interop;

namespace EmojiShortcutApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private List<EmojiSearch> EmojiList;

        private bool searching = false;
        private string searchQuery = "";
        private IntPtr activeWindowHandle = IntPtr.Zero;

        private bool isInsertingEmoji = false;

        public MainWindow()
        {
            InitializeComponent();
            Visibility = System.Windows.Visibility.Hidden;

            EmojiList = Emoji.Load();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var keyHook = new GlobalKeyHook();
            keyHook.OnKeyDown += KeyHook_OnKeyDown;

            var mainWindowHandle = new WindowInteropHelper(this).Handle;
            var winHook = new WindowHook(mainWindowHandle);
            winHook.Changed += WinHook_Changed;
        }

        private void WinHook_Changed(object sender, WindowHookEvent e)
        {
            activeWindowHandle = e.Handle;
        }

        private void MoveToCaret()
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

        private void StartSearch()
        {
            searching = true;
            searchQuery = "";
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                searchQueryElement.Text = "";
                Visibility = System.Windows.Visibility.Visible;
            }));
        }

        private void StopSearch()
        {
            searching = false;
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                searchQueryElement.Text = "";
                Visibility = System.Windows.Visibility.Hidden;
            }));
        }

        private void UpdateSearchQuery()
        {
            var MatchingEmojiList = EmojiList
                .Where(e => e.keywords.Any(k => k.ToLower().StartsWith(searchQuery.ToLower())));

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                searchQueryElement.Text = searchQuery;

                //EmojiListBox.ItemsSource = MatchingEmojiList;
            }));
        }

        private void SubmitEmoji()
        {
            isInsertingEmoji = true;
            if (SetForegroundWindow(activeWindowHandle))
            {
                KeyboardInput.Send("🎉");
            }
            else
            {
                Debug.WriteLine("NO");
            }
            isInsertingEmoji = false;
        }


        private void KeyHook_OnKeyDown(object sender, GlobalKeyEventArgs e)
        {
            if (isInsertingEmoji)
            {
                return;
            }

            //Debug.WriteLine("KeyHook_OnKeyDown: {0} ({1})", e.CharResult, e.KeyCode);
            if (!searching)
            {
                if (e.CharResult == ":")
                {
                    e.Handled = true;
                    MoveToCaret();
                    StartSearch();
                }
            }
            else
            {
                if (e.KeyCode == VirtualKeycodes.Enter || e.CharResult == ":")
                {
                    e.Handled = true;
                    SubmitEmoji();
                    StopSearch();
                }
                else if (e.KeyCode == VirtualKeycodes.Backspace)
                {
                    e.Handled = true;

                    searchQuery = searchQuery.Remove(searchQuery.Length - 1);

                    UpdateSearchQuery();
                }
                else if (e.KeyCode == VirtualKeycodes.Esc)
                {
                    e.Handled = true;

                    searchQuery = "";

                    StopSearch();
                }
                else if (e.KeyCode == VirtualKeycodes.Space || (e.CharResult.Length > 0 && char.IsLetterOrDigit(e.CharResult, 0)))
                {
                    e.Handled = true;

                    searchQuery += e.CharResult;

                    UpdateSearchQuery();
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
