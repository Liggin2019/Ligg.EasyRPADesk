using System;
using System.Drawing;
using System.Windows.Forms;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.WinForm.Controls;
using Ligg.RpaDesk.WinForm.Resources;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Forms
{
    public partial class GroundForm : BaseForm
    {
        protected int RunningMessageSectionHeight = 0;
        protected bool ShowRunningProgressSection =false;
        protected int RunningProgressSectionHeight = 30;
        protected bool ShowRunningStatusSection = false;
        protected int RunningStatusSectionHeight = 25;
        protected bool ShowThreadInfo=false;
        protected int RunningStatusSectionThreadInfoRegionWidth =60;
        protected int RunningStatusSectionMsgRegionInfoZoneWidth = 88;
        protected string ThreadListImageTextButtonText = TextRes.TaskList;

        protected GroundForm()
        {
            InitializeComponent();
        }

        //#method
        private void GroundForm_Load(object sender, EventArgs e)
        {
            InitGroundComponent();
        }

        protected void RunningStatusSectionBackTaskRegionImageTextButton_Click(object sender, EventArgs e)
        {
            //var baseCtrl = RunningStatusSectionBackTaskRegion;
            //var p = baseCtrl.PointToClient(new Point(0, 0));
            //var p1 = GroundPanel.PointToClient(new Point(0, 0));
            //var pos = new Point(p1.X - p.X, p1.Y - p.Y);
            //pos.X = pos.X - ManagedThreadPoolContainer.Width + RunningStatusSectionBackgroundTaskRegionWidth - 5;
            //pos.Y = pos.Y - ManagedThreadPoolContainer.Height - 5;
            //ManagedThreadPoolContainer.Location = pos;
            //ManagedThreadPoolContainer.Visible = true;
            //ManagedThreadPoolContainer.BringToFront();
        }

        //#proc
        protected void InitGroundComponent()
        {
            RunningMessageSection.BackColor = StyleSheet.GroundColor;
            RunningMessageSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
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
            RunningStatusSection.StyleType = ContainerPanel.ContainerPanelStyle.Borders;
            RunningStatusSection.RoundStyle = RoundStyle.None;
            RunningStatusSection.Radius = 0;
            RunningStatusSection.BorderWidthOnLeft = 0;
            RunningStatusSection.BorderWidthOnTop = 1;
            RunningStatusSection.BorderWidthOnRight = 0;
            RunningStatusSection.BorderWidthOnBottom = 0;
            RunningStatusSection.BorderColor = StyleSheet.ControlBorderColor;

            RunningStatusSectionMsgRegionLabelMsg.Text = TextRes.Ready;
            RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            RunningStatusSectionMsgRegionLabelMsg3.Text = "";
        }

        protected void ResizeGroundComponent()
        {
            RunningMessageSection.Height = RunningMessageSectionHeight;
            RunningProgressSection.Height = ShowRunningProgressSection ? RunningProgressSectionHeight:0;

            RunningStatusSection.Height = ShowRunningStatusSection ? RunningStatusSectionHeight : 0;
            RunningStatusSectionMsgRegion.Width = RunningMessageSection.Width-1;
            RunningStatusSectionMsgRegionMsgZone.Width = RunningStatusSectionMsgRegion.Width - RunningStatusSectionMsgRegionInfoZoneWidth;


        }

        protected void ResetGroundTextByCulture()
        {
            RunningStatusSectionMsgRegionLabelMsg.Text = TextRes.Ready;
        }

        //#RuningdMessage
        //1.command, 2.suceeded, 3.failed
        protected void UpdateRunningMessage(int type, string message)
        {
            if (type == 1) UpdateRunningMessage("# " + message, StyleSheet.ColorCommand, true);
            else if (type == 2) UpdateRunningMessage("# " + message, StyleSheet.ColorSucceeded, true);
            else if (type == 3) UpdateRunningMessage("# " + message, StyleSheet.ColorError, true);
            else UpdateRunningMessage(message, StyleSheet.ColorDefault, true);
        }

        protected void UpdateRunningMessage(string message, Color color, bool isNewLine)
        {
            RunningMessageSectionRichTextBox.Focus();
            RunningMessageSectionRichTextBox.AppendText("");
            RunningMessageSectionRichTextBox.SelectionColor = color;
            RunningMessageSectionRichTextBox.AppendText(message + (isNewLine ? "\r\n" : null));
        }

        //#RunningStatus
        protected void InitRunningStatusMessageComponent()
        {
            RunningStatusSectionMsgRegionLabelMsg.Text = TextRes.Ready;
            RunningStatusSectionMsgRegionLabelMsg1.Text = "";
            RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            RunningStatusSectionMsgRegionLabelMsg3.Text = "";
            RunningStatusSectionMsgRegion.Refresh();
        }

        protected void RefreshRunningStatusMessage( string txt1)
        {
            var txt = TextRes.Dispensing;
            var txt3 = ", " + TextRes.PleaseWait + "...";
            RunningStatusSectionMsgRegionLabelMsg.Text = txt;
            RunningStatusSectionMsgRegionLabelMsg1.Text = txt1;
            RunningStatusSectionMsgRegionLabelMsg3.Text = txt3;
            RunningStatusSectionMsgRegion.Refresh();
        }

        protected void RefreshRunningStatusMessage(string txt1, string txt2)
        {
            var txt = TextRes.Dispensing;
            var txt3 = ", " + TextRes.PleaseWait + "...";
            RunningStatusSectionMsgRegionLabelMsg.Text = txt;
            RunningStatusSectionMsgRegionLabelMsg1.Text = txt1;
            RunningStatusSectionMsgRegionLabelMsg2.Text = ">"+txt2;
            RunningStatusSectionMsgRegionLabelMsg3.Text = txt3;
            RunningStatusSectionMsgRegion.Refresh();
        }



    }
}
