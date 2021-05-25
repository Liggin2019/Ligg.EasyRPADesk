using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Handlers;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;
using Ligg.EasyWinApp.WinForm.Helpers;
using Ligg.EasyWinApp.WinForm.Resources;

namespace Ligg.EasyWinApp.WinForm.Forms
{
    //** will change IdName to IdValueText
    public partial class BaseForm : GroundForm
    {
        protected int RunningMessageSectionHeight = 49;
        protected int RunningProgressSectionHeight = 30;
        protected int RunningStatusSectionHeight = 21;
        protected int RunningStatusSectionBackgroundTaskRegionWidth = 0;

        private List<IntIdName> _backgroundTaskTypeList = new List<IntIdName>();
        private List<Annex> _backgroundTaskTypeAnnexes = new List<Annex>();
        protected string BackgroundTaskImageTextButtonText = WinformRes.TaskList;
        protected string BackgroundTaskInfoLabelText = WinformRes.TaskList;

        protected BaseForm()
        {
            InitializeComponent();
        }

        //#method
        private void BaseForm_Load(object sender, EventArgs e)
        {
            InitBaseComponent();
        }

        protected void RunningStatusSectionBackTaskRegionImageTextButton_Click(object sender, EventArgs e)
        {
            var baseCtrl = RunningStatusSectionBackTaskRegion;
            var p = baseCtrl.PointToClient(new Point(0, 0));
            var p1 = GroundPanel.PointToClient(new Point(0, 0));
            var pos = new Point(p1.X - p.X, p1.Y - p.Y);
            pos.X = pos.X - ManagedThreadPoolContainer.Width + RunningStatusSectionBackgroundTaskRegionWidth - 5;
            pos.Y = pos.Y - ManagedThreadPoolContainer.Height - 5;
            ManagedThreadPoolContainer.Location = pos;
            ManagedThreadPoolContainer.Visible = true;
            ManagedThreadPoolContainer.BringToFront();
        }

        //#proc
        protected void InitBaseComponent()
        {
            RunningMessageSection.BackColor = StyleSheet.GroundColor;
            RunningMessageSection.StyleType = Ligg.EasyWinApp.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            RunningMessageSection.RoundStyle = RoundStyle.None;
            RunningMessageSection.Radius = 0;
            RunningMessageSection.BorderWidthOnLeft = 0;
            RunningMessageSection.BorderWidthOnTop = 1;
            RunningMessageSection.BorderWidthOnRight = 0;
            RunningMessageSection.BorderWidthOnBottom = 0;
            RunningMessageSection.BorderColor = StyleSheet.ControlBorderColor;
            RunningMessageSection.Padding = new Padding(2);

            RunningProgressSection.BackColor = StyleSheet.GroundColor;

            RunningStatusSection.BackColor = StyleSheet.RunningStatusSectionBackColor;
            RunningStatusSection.StyleType = Ligg.EasyWinApp.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            RunningStatusSection.RoundStyle = RoundStyle.None;
            RunningStatusSection.Radius = 0;
            RunningStatusSection.BorderWidthOnLeft = 0;
            RunningStatusSection.BorderWidthOnTop = 1;
            RunningStatusSection.BorderWidthOnRight = 0;
            RunningStatusSection.BorderWidthOnBottom = 0;
            RunningStatusSection.BorderColor = StyleSheet.ControlBorderColor;

            RunningStatusSectionMsgRegionLabelMsg.Text = WinformRes.Ready;
            RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            RunningStatusSectionMsgRegionLabelMsg3.Text = "";


            RunningStatusSectionBackTaskRegion.Visible = false;
            RunningStatusSectionBackTaskRegionImageTextButton.Text = BackgroundTaskImageTextButtonText + @" 0/0";
        }

        protected void ResizeBaseComponent()
        {
            RunningMessageSection.Height = RunningMessageSectionHeight;

            RunningProgressSection.Height = RunningProgressSectionHeight;

            RunningStatusSection.Height = RunningStatusSectionHeight;
            RunningStatusSectionBackTaskRegion.Width = RunningStatusSectionBackgroundTaskRegionWidth;
            RunningStatusSectionMsgRegion.Width = RunningMessageSection.Width - RunningStatusSectionBackTaskRegion.Width - 5;
            RunningStatusSectionMsgRegionMsgZone.Width = RunningStatusSectionMsgRegion.Width;

        }

        protected void ResetBaseTextByCulture()
        {
            RunningStatusSectionMsgRegionLabelMsg.Text = WinformRes.Ready;
        }

        //#RuningdMessage
        //1.command, 2.suceeded, 3.failed
        protected void WriteRunningdMessage(int type, string message)
        {
            if (type == 1) WriteRunningMessage("# " + message, StyleSheet.ColorCommand, true);
            else if (type == 2) WriteRunningMessage("# " + message, StyleSheet.ColorSucceeded, true);
            else if (type == 3) WriteRunningMessage("# " + message, StyleSheet.ColorError, true);
            else WriteRunningMessage(message, StyleSheet.ColorDefault, true);
        }

        protected void WriteRunningMessage(string message, Color color, bool isNewLine)
        {
            RunningMessageSectionRichTextBox.Focus();
            RunningMessageSectionRichTextBox.AppendText("");
            RunningMessageSectionRichTextBox.SelectionColor = color;
            RunningMessageSectionRichTextBox.AppendText(message + (isNewLine ? "\r\n" : null));
        }

        //#RunningStatus
        protected void InitRunningStatusMessageComponent()
        {
            RunningStatusSectionMsgRegionLabelMsg.Text = WinformRes.Ready;
            RunningStatusSectionMsgRegionLabelMsg1.Text = "";
            RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            RunningStatusSectionMsgRegionLabelMsg3.Text = "";
            RunningStatusSectionMsgRegion.Refresh();
        }

        protected void RefreshRunningStatusMessage( string txt1)
        {
            var txt = WinformRes.Dispensing;
            var txt3 = ", " + WinformRes.PleaseWait + "...";
            RunningStatusSectionMsgRegionLabelMsg.Text = txt;
            RunningStatusSectionMsgRegionLabelMsg1.Text = txt1;
            //RunningStatusSectionMsgRegionLabelMsg2.Text = txt2;
            RunningStatusSectionMsgRegionLabelMsg3.Text = txt3;
            RunningStatusSectionMsgRegion.Refresh();
        }



    }
}
