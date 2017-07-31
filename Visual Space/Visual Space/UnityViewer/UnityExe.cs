using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;

namespace VisualSpace.UnityViewer
{
    public partial class UnityExe : System.Windows.Forms.UserControl
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        //MoveWindow 함수를 호출한다.
        internal static extern bool MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern void SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hWnd, Int32 Offset);

        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hWnd, Int32 Offset, Int32 newLong);


        private const int GWL_STYLE = (-16);
        public static bool firstLoad = true, UnityExeOn = false;
        public static Process p;

        public string Path { get; private set; }

        public UnityExe()
        {
            InitializeComponent();
            this.Load += UnityExe_Load;
         }

 
        public UnityExe(string exePath)
        {
            Path = exePath;
            InitializeComponent();
        }


        public void CloseExe()
        {
            if (p != null)
                p.Kill();
        }

        private void UnityExe_SizeChanged(object sender, EventArgs e)
        {
            if (UnityExeOn)
                MoveWindow(p.MainWindowHandle, 0, 0, this.Width, this.Height, true);
        }

        private void UnityExe_LostFocus(object sender, EventArgs e)
        {

            GetFocus();
        }

        public void GetFocus()
        {
            if (UnityExeOn)
                SetForegroundWindow(p.MainWindowHandle);
        }

        public void UnityExe_Load(object sender, EventArgs e)
        {
            if (firstLoad)
            {
                p = Process.Start(Path);
                p.WaitForInputIdle();
                Thread.Sleep(1000);
                IntPtr child = SetParent(p.MainWindowHandle, this.panel.Handle);


                int style = GetWindowLong(this.Handle, GWL_STYLE);

                WindowStyle myStyle = (WindowStyle)style;

                myStyle = myStyle & ~WindowStyle.WS_CAPTION;
                myStyle = myStyle | WindowStyle.WS_BORDER | WindowStyle.WS_MINIMIZEBOX;

                SetWindowLong(p.MainWindowHandle, GWL_STYLE, (int)myStyle);

                MoveWindow(p.MainWindowHandle, 0, 0, panel.Width, panel.Height, true);
                firstLoad = false;
                UnityExeOn = true;

            }
        }

        public void ExeLoad()
        {
            if (firstLoad)
            {
                p = Process.Start(Path);
                p.WaitForInputIdle();
                Thread.Sleep(500);
                IntPtr child = SetParent(p.MainWindowHandle, this.panel.Handle);


                int style = GetWindowLong(this.Handle, GWL_STYLE);

                WindowStyle myStyle = (WindowStyle)style;

                myStyle = myStyle & ~WindowStyle.WS_CAPTION;
                myStyle = myStyle | WindowStyle.WS_BORDER | WindowStyle.WS_MAXIMIZEBOX;

                SetWindowLong(p.Handle, GWL_STYLE, (int)myStyle);

                MoveWindow(p.MainWindowHandle, 0, 0, this.Width, this.Height, true);
                firstLoad = false;
                UnityExeOn = true;
            }

        }

    }

    [Flags]
    public enum WindowStyle
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = -2147483648, //0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_CAPTION = 0x00C00000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,
        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU |
                    WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU),
        WS_CHILDWINDOW = (WS_CHILD)
    }
}
