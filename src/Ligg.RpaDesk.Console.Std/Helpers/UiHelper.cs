using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.WinCnsl.DataModels;
using Ligg.RpaDesk.WinCnsl.DataModels.Enums;
using Ligg.RpaDesk.Parser.Helpers;

namespace Ligg.RpaDesk.WinCnsl.Helpers
{
    public static class UiHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static void CheckUiItems(string UiFilePath, List<UiItem> uiItems)
        {
            var exInfo = "\n >> " + _typeFullName + ".CheckUiItems Error: ";
            var uiItemTypeNames = EnumHelper.GetNames<UiItemType>();
            try
            {
                foreach (var item in uiItems)
                {
                    CommonHelper.CheckName(item.Name);

                    if (uiItems.FindAll(x => x.Name == item.Name).Count > 1)
                    {
                        throw new ArgumentException("UiItem can't have duplicated name!" + ", item.Name=" + item.Name);
                    }

                    var isLegalControlTypeName = uiItemTypeNames.Contains(item.TypeName);
                    if (!isLegalControlTypeName)
                        throw new ArgumentException(exInfo + "UiItem's Type is not valid ! TypeName= " + item.TypeName + ", item.Name=" + item.Name + ", TypeName should be in " + uiItemTypeNames.Unwrap(", "));

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + _typeFullName + ".CheckUiItems Error: UiFilePath=" + UiFilePath + "; " + ex.Message);
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
                throw new ArgumentException("\n>> " + _typeFullName + ".SetUiItemType Error: " + ex.Message);
            }
        }

        internal static void SetUiItemsIds(List<UiItem> uiItems)
        {
            var i = 0;
            foreach (var item in uiItems)
            {
                item.Id = i;
                i++;

            }
        }

        //*icon
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

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        private static void SetWindowIcon(System.Drawing.Icon icon)
        {
            IntPtr mwHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            IntPtr result01 = SendMessage(mwHandle, (int)WinMessages.SETICON, 0, icon.Handle);
            IntPtr result02 = SendMessage(mwHandle, (int)WinMessages.SETICON, 1, icon.Handle);
        }
        private enum WinMessages : uint
        {
            /// <summary>
            /// An application sends the WM_SETICON message to associate a new large or small icon with a window. 
            /// The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption. 
            /// </summary>
            SETICON = 0x0080,
        }



    }

}