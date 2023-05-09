using System.Windows.Forms;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.WinForm.Dialogs;
using Ligg.RpaDesk.WinForm.Resources;

namespace Ligg.RpaDesk.WinForm.Helpers
{
    public static class MessageHelper
    {
        public static void Popup(string msg)
        {
            MessageBox.Show(msg, TextRes.SystemMessage);
        }

        public static void Popup(string title, string msg)
        {
            title = title ?? TextRes.SystemMessage;
            MessageBox.Show(msg, title);
        }

        public static void Popup(string title, string msg, PopupMessageFormFormat format, int w)
        {
            title = title ?? TextRes.SystemMessage;
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
            MessageBox.Show(msg, TextRes.ApplicationRunningError, MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public static void PopupError(string tittle,string msg)
        {
            MessageBox.Show(msg, tittle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void PopupWarning(string tittle, string msg)
        {
            MessageBox.Show(msg, tittle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void PopupError(string err, string ErrMsg, string addInfo)
        {
            var dlg = new RunningErrorViewer();
            dlg.Text = TextRes.ApplicationRunningError;
            dlg.ErrorText = err;
            dlg.ErrorMsg = ErrMsg;
            dlg.AdditionalInfo = addInfo;
            dlg.ShowDialog();
        }

    }

}
