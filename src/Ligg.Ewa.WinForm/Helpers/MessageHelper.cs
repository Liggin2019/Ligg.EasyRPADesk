﻿using System.Windows.Forms;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.EasyWinApp.WinForm.Dialogs;
using Ligg.EasyWinApp.WinForm.Resources;

namespace Ligg.EasyWinApp.WinForm.Helpers
{
    public static class MessageHelper
    {

        public static void Popup(string msg)
        {
            MessageBox.Show(msg, WinformRes.SystemMessage);
        }

        public static void Popup(string title, string msg)
        {
            title = title ?? WinformRes.SystemMessage;
            MessageBox.Show(msg, title);
        }

        public static void Popup(string title, string msg, PopupMessageFormFormat format, int w)
        {
            title = title ?? WinformRes.SystemMessage;
            if(format==PopupMessageFormFormat.RichTextViewer)
            {
                var dlg = new RichTextViewer();
                dlg.Title = title;
                dlg.Content = msg;
                dlg.FormWidth = w;
                dlg.ShowDialog();
            }
            else if(format==PopupMessageFormFormat.MessageViewer)
            {
                var dlg = new MessageViewer();
                dlg.Title = title;
                dlg.Content = msg;
                dlg.FormWidth = w;
                dlg.ShowDialog();
            }
            else
            {
                MessageBox.Show(msg, title);
            }
        }

        public static void PopupError(string msg)
        {
            MessageBox.Show(msg, WinformRes.ApplicationRunningError, MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public static void PopupError(string tittle,string msg)
        {
            MessageBox.Show(msg, tittle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void PopupError(string error, string exceptionMsg, string addInfo)
        {
            var dlg = new RunningErrorViewer();
            dlg.Text = WinformRes.ApplicationRunningError;
            dlg.AdditionalInfo = addInfo;
            dlg.ErrorText = error;
            dlg.ExceptionMsg = exceptionMsg;
            dlg.ShowDialog();
        }

    }

}
