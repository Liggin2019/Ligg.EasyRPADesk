using System;
using System.IO;
using System.Runtime.InteropServices;


namespace Ligg.Infrastructure.Base.Helpers
{
    public static class ConsoleHelper
    {
        public  static void DisbleClosebtn(string consoleTitle)
        {
            IntPtr windowHandle = FindWindow(null, consoleTitle);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }
        public static void CloseConsole()
        {
            Environment.Exit(0);
        }

        public static void HideWindow(string consoleTitle)
        {
            IntPtr a = FindWindow("ConsoleWindowClass", consoleTitle);
            if (a != IntPtr.Zero)
                ShowWindow(a, 0);//隐藏窗口
            else
                throw new Exception("can't hide console window");
        }
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        private extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        public static void DisbleQuickEditMode()
        {
            IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
            uint mode;
            GetConsoleMode(hStdin, out mode);
            mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
            mode &= ~ENABLE_INSERT_MODE;      //移除插入模式
            SetConsoleMode(hStdin, mode);
        }
        // 关闭控制台 快速编辑模式、插入模式
        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040;
        const uint ENABLE_INSERT_MODE = 0x0020;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int hConsoleHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);


        //*put into netStd project(ligg.infra, netStd2.0), then be called by console project(netCore 3.1), ok;
        //*put into netStd project(ligg.infra, netStd2.0), then be called by console project(netFx4.72), popup error:未能加载文件或程序集“System.Drawing.Common, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51”或它的某一个依赖项。
        //* directly put into .netcore console project(netCore 3.1) then be called internally, not ok
        //* directly put into .netFx console project(netFx4.72) then be called internally, ok
        public static void SetIcon(string iconFilePath)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!string.IsNullOrEmpty(iconFilePath)&File.Exists(iconFilePath))
                {
                    System.Drawing.Icon icon = new System.Drawing.Icon(iconFilePath);
                    SetWindowIcon(icon);
                }
            }
        }
        private enum WinMessages : uint
        {
            /// <summary>
            /// An application sends the WM_SETICON message to associate a new large or small icon with a window. 
            /// The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
            /// </summary>
            SETICON = 0x0080,
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private static void SetWindowIcon(System.Drawing.Icon icon)
        {
            IntPtr mwHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            IntPtr result01 = SendMessage(mwHandle, (int)WinMessages.SETICON, 0, icon.Handle);
            IntPtr result02 = SendMessage(mwHandle, (int)WinMessages.SETICON, 1, icon.Handle);
        }


        //*followings not ok
        //*put into netStd project(ligg.infra, netStd2.0), then be called by console project(netCore 3.1), not ok;
        //*put into netStd project(ligg.infra, netStd2.0), then be called by console project(netFx4.72), popup error:未能加载文件或程序集“System.Drawing.Common, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51”或它的某一个依赖项。
        //*directly put into .netcore console project(netCore 3.1) then be called internally, not ok
        //*directly put into .netFx console project(netFx4.72) then be called internally, not ok
        public static void SetIconNotOk(System.Drawing.Icon icon)
        {
            SetConsoleIcon(icon.Handle);
        }
        public static void SetIconNotOk(string iconFilePath)
        {

            var strm = File.Open(iconFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            //var icon = new System.Drawing.Icon(strm);
            var icon = new System.Drawing.Icon(iconFilePath);
            SetConsoleIcon(icon.Handle);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleIcon(IntPtr hIcon);



    }
}
