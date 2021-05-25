using System;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.EasyWinApp.WinCnsl.DataModel;
using Ligg.EasyWinApp.WinCnsl.DataModel.Enums;
using Ligg.EasyWinApp.Parser.Helpers;

namespace Ligg.EasyWinApp.WinCnsl.Helpers
{
    public static class UiHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static void CheckUiItems(List<UiItem> uiItems)
        {

            var exInfo = "\n>> " + TypeName + ".CheckUiItems Error: ";
            foreach (var item in uiItems)
            {
                UiElementHelper.CheckName(item.Name);

                if (uiItems.FindAll(x => x.Name == item.Name).Count > 1)
                {
                    throw new ArgumentException(exInfo + "UiItem can't have duplicated name!" + ", item.Name=" + item.Name);
                }
            }

        }

        internal static void SetUiItemType(UiItem uiItem)
        {
            try
            {
                uiItem.Type = EnumHelper.GetIdByName<UiItemType>(uiItem.TypeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".SetUiItemType Error: " + ex.Message);
            }
        }

        internal static void SetUiItemsIds(List<UiItem> uiItems)
        {
            var i = 0;
            foreach(var item in uiItems)
            {
                item.Id = i;
                i++;

            }
        }

        public static void SetIcon(string iconFilePath)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!string.IsNullOrEmpty(iconFilePath) & File.Exists(iconFilePath))
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



    }

}