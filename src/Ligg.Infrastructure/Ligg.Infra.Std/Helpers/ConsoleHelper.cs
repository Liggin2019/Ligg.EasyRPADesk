using System;
using System.Runtime.InteropServices;


namespace Ligg.Infrastructure.Helpers
{
    public static class ConsoleHelper
    {
        //*disabled
        public static void DisbleClosebtn(string consoleTitle)
        {
            IntPtr windowHandle = FindWindow(null, consoleTitle);
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        private extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        //*close
        public static void CloseConsole()
        {
            Environment.Exit(0);
        }

        //*close QuickEditMode
        public static void CloseQuickEditMode()
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


    }
}
