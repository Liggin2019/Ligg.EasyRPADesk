using System.Windows.Forms;
using Ligg.RpaDesk.WinForm.Controls;
using Ligg.RpaDesk.WinForm.DataModels.Enums;

namespace Ligg.RpaDesk.WinForm.Forms
{
    partial class GroundForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RunningStatusSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.RunningStatusSectionMsgRegion = new System.Windows.Forms.Panel();
            this.RunningStatusSectionMsgRegionMsgZone = new System.Windows.Forms.Panel();
            this.RunningStatusSectionMsgRegionLabelMsg3 = new System.Windows.Forms.Label();
            this.RunningStatusSectionMsgRegionLabelMsg2 = new System.Windows.Forms.Label();
            this.RunningStatusSectionMsgRegionLabelMsg1 = new System.Windows.Forms.Label();
            this.RunningStatusSectionMsgRegionLabelMsg = new System.Windows.Forms.Label();
            this.RunningStatusSectionMsgRegionInfoZone = new System.Windows.Forms.Panel();
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1 = new System.Windows.Forms.Label();
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo = new System.Windows.Forms.Label();
            this.RunningProgressSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.RunningProgressSectionProgressBar = new System.Windows.Forms.ProgressBar();
            this.RunningMessageSectionRichTextBox = new System.Windows.Forms.RichTextBox();
            this.RunningMessageSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.BasePanel.SuspendLayout();
            this.RunningStatusSection.SuspendLayout();
            this.RunningStatusSectionMsgRegion.SuspendLayout();
            this.RunningStatusSectionMsgRegionMsgZone.SuspendLayout();
            this.RunningStatusSectionMsgRegionInfoZone.SuspendLayout();
            this.RunningProgressSection.SuspendLayout();
            this.RunningMessageSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // BasePanel
            // 
            this.BasePanel.BackColor = System.Drawing.Color.White;
            this.BasePanel.Controls.Add(this.RunningMessageSection);
            this.BasePanel.Controls.Add(this.RunningProgressSection);
            this.BasePanel.Controls.Add(this.RunningStatusSection);
            this.BasePanel.Location = new System.Drawing.Point(2, 24);
            this.BasePanel.Size = new System.Drawing.Size(796, 574);
            // 
            // RunningStatusSection
            // 
            this.RunningStatusSection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.RunningStatusSection.BorderWidthOnBottom = 0;
            this.RunningStatusSection.BorderWidthOnTop = 1;
            this.RunningStatusSection.Controls.Add(this.RunningStatusSectionMsgRegion);
            this.RunningStatusSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RunningStatusSection.Location = new System.Drawing.Point(0, 549);
            this.RunningStatusSection.Name = "RunningStatusSection";
            this.RunningStatusSection.Padding = new System.Windows.Forms.Padding(1);
            this.RunningStatusSection.Radius = 4;
            this.RunningStatusSection.Size = new System.Drawing.Size(796, 25);
            this.RunningStatusSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            this.RunningStatusSection.TabIndex = 9;
            // 
            // RunningStatusSectionMsgRegion
            // 
            this.RunningStatusSectionMsgRegion.Controls.Add(this.RunningStatusSectionMsgRegionMsgZone);
            this.RunningStatusSectionMsgRegion.Controls.Add(this.RunningStatusSectionMsgRegionInfoZone);
            this.RunningStatusSectionMsgRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegion.Location = new System.Drawing.Point(1, 1);
            this.RunningStatusSectionMsgRegion.Margin = new System.Windows.Forms.Padding(1);
            this.RunningStatusSectionMsgRegion.Name = "RunningStatusSectionMsgRegion";
            this.RunningStatusSectionMsgRegion.Size = new System.Drawing.Size(689, 23);
            this.RunningStatusSectionMsgRegion.TabIndex = 1;
            // 
            // RunningStatusSectionMsgRegionMsgZone
            // 
            this.RunningStatusSectionMsgRegionMsgZone.BackColor = System.Drawing.Color.White;
            this.RunningStatusSectionMsgRegionMsgZone.Controls.Add(this.RunningStatusSectionMsgRegionLabelMsg3);
            this.RunningStatusSectionMsgRegionMsgZone.Controls.Add(this.RunningStatusSectionMsgRegionLabelMsg2);
            this.RunningStatusSectionMsgRegionMsgZone.Controls.Add(this.RunningStatusSectionMsgRegionLabelMsg1);
            this.RunningStatusSectionMsgRegionMsgZone.Controls.Add(this.RunningStatusSectionMsgRegionLabelMsg);
            this.RunningStatusSectionMsgRegionMsgZone.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegionMsgZone.Location = new System.Drawing.Point(0, 0);
            this.RunningStatusSectionMsgRegionMsgZone.Name = "RunningStatusSectionMsgRegionMsgZone";
            this.RunningStatusSectionMsgRegionMsgZone.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
            this.RunningStatusSectionMsgRegionMsgZone.Size = new System.Drawing.Size(146, 23);
            this.RunningStatusSectionMsgRegionMsgZone.TabIndex = 18;
            // 
            // RunningStatusSectionMsgRegionLabelMsg3
            // 
            this.RunningStatusSectionMsgRegionLabelMsg3.AutoSize = true;
            this.RunningStatusSectionMsgRegionLabelMsg3.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegionLabelMsg3.Location = new System.Drawing.Point(81, 4);
            this.RunningStatusSectionMsgRegionLabelMsg3.Name = "RunningStatusSectionMsgRegionLabelMsg3";
            this.RunningStatusSectionMsgRegionLabelMsg3.Size = new System.Drawing.Size(29, 12);
            this.RunningStatusSectionMsgRegionLabelMsg3.TabIndex = 20;
            this.RunningStatusSectionMsgRegionLabelMsg3.Text = "msg3";
            this.RunningStatusSectionMsgRegionLabelMsg3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RunningStatusSectionMsgRegionLabelMsg2
            // 
            this.RunningStatusSectionMsgRegionLabelMsg2.AutoSize = true;
            this.RunningStatusSectionMsgRegionLabelMsg2.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegionLabelMsg2.Location = new System.Drawing.Point(52, 4);
            this.RunningStatusSectionMsgRegionLabelMsg2.Name = "RunningStatusSectionMsgRegionLabelMsg2";
            this.RunningStatusSectionMsgRegionLabelMsg2.Size = new System.Drawing.Size(29, 12);
            this.RunningStatusSectionMsgRegionLabelMsg2.TabIndex = 19;
            this.RunningStatusSectionMsgRegionLabelMsg2.Text = "msg2";
            this.RunningStatusSectionMsgRegionLabelMsg2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RunningStatusSectionMsgRegionLabelMsg1
            // 
            this.RunningStatusSectionMsgRegionLabelMsg1.AutoSize = true;
            this.RunningStatusSectionMsgRegionLabelMsg1.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegionLabelMsg1.Location = new System.Drawing.Point(23, 4);
            this.RunningStatusSectionMsgRegionLabelMsg1.Name = "RunningStatusSectionMsgRegionLabelMsg1";
            this.RunningStatusSectionMsgRegionLabelMsg1.Size = new System.Drawing.Size(29, 12);
            this.RunningStatusSectionMsgRegionLabelMsg1.TabIndex = 18;
            this.RunningStatusSectionMsgRegionLabelMsg1.Text = "msg1";
            this.RunningStatusSectionMsgRegionLabelMsg1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RunningStatusSectionMsgRegionLabelMsg
            // 
            this.RunningStatusSectionMsgRegionLabelMsg.AutoSize = true;
            this.RunningStatusSectionMsgRegionLabelMsg.Dock = System.Windows.Forms.DockStyle.Left;
            this.RunningStatusSectionMsgRegionLabelMsg.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg.Name = "RunningStatusSectionMsgRegionLabelMsg";
            this.RunningStatusSectionMsgRegionLabelMsg.Size = new System.Drawing.Size(23, 12);
            this.RunningStatusSectionMsgRegionLabelMsg.TabIndex = 17;
            this.RunningStatusSectionMsgRegionLabelMsg.Text = "msg";
            this.RunningStatusSectionMsgRegionLabelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RunningStatusSectionMsgRegionInfoZone
            // 
            this.RunningStatusSectionMsgRegionInfoZone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(145)))), ((int)(((byte)(242)))));
            this.RunningStatusSectionMsgRegionInfoZone.Controls.Add(this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1);
            this.RunningStatusSectionMsgRegionInfoZone.Controls.Add(this.RunningStatusSectionMsgRegionInfoZoneLabelInfo);
            this.RunningStatusSectionMsgRegionInfoZone.Dock = System.Windows.Forms.DockStyle.Right;
            this.RunningStatusSectionMsgRegionInfoZone.Location = new System.Drawing.Point(610, 0);
            this.RunningStatusSectionMsgRegionInfoZone.Name = "RunningStatusSectionMsgRegionInfoZone";
            this.RunningStatusSectionMsgRegionInfoZone.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
            this.RunningStatusSectionMsgRegionInfoZone.Size = new System.Drawing.Size(79, 23);
            this.RunningStatusSectionMsgRegionInfoZone.TabIndex = 21;
            // 
            // RunningStatusSectionMsgRegionInfoZoneLabelInfo1
            // 
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.AutoSize = true;
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.Dock = System.Windows.Forms.DockStyle.Right;
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.Location = new System.Drawing.Point(7, 4);
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.Name = "RunningStatusSectionMsgRegionInfoZoneLabelInfo1";
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.TabIndex = 18;
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RunningStatusSectionMsgRegionInfoZoneLabelInfo
            // 
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Font = new System.Drawing.Font("Agency FB", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Location = new System.Drawing.Point(7, 4);
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Name = "RunningStatusSectionMsgRegionInfoZoneLabelInfo";
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Size = new System.Drawing.Size(68, 15);
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.TabIndex = 17;
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Text = "Powerd By Liggit";
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RunningProgressSection
            // 
            this.RunningProgressSection.BorderColor = System.Drawing.Color.Empty;
            this.RunningProgressSection.BorderWidthOnBottom = 0;
            this.RunningProgressSection.Controls.Add(this.RunningProgressSectionProgressBar);
            this.RunningProgressSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RunningProgressSection.Location = new System.Drawing.Point(0, 519);
            this.RunningProgressSection.Name = "RunningProgressSection";
            this.RunningProgressSection.Radius = 4;
            this.RunningProgressSection.Size = new System.Drawing.Size(796, 30);
            this.RunningProgressSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.RunningProgressSection.TabIndex = 12;
            // 
            // RunningProgressSectionProgressBar
            // 
            this.RunningProgressSectionProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RunningProgressSectionProgressBar.Location = new System.Drawing.Point(0, 0);
            this.RunningProgressSectionProgressBar.Name = "RunningProgressSectionProgressBar";
            this.RunningProgressSectionProgressBar.Size = new System.Drawing.Size(796, 30);
            this.RunningProgressSectionProgressBar.TabIndex = 0;
            // 
            // RunningMessageSectionRichTextBox
            // 
            this.RunningMessageSectionRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RunningMessageSectionRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RunningMessageSectionRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.RunningMessageSectionRichTextBox.Name = "RunningMessageSectionRichTextBox";
            this.RunningMessageSectionRichTextBox.Size = new System.Drawing.Size(796, 44);
            this.RunningMessageSectionRichTextBox.TabIndex = 0;
            this.RunningMessageSectionRichTextBox.Text = "";
            // 
            // RunningMessageSection
            // 
            this.RunningMessageSection.BorderColor = System.Drawing.Color.Transparent;
            this.RunningMessageSection.BorderWidthOnBottom = 0;
            this.RunningMessageSection.BorderWidthOnTop = 1;
            this.RunningMessageSection.Controls.Add(this.RunningMessageSectionRichTextBox);
            this.RunningMessageSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.RunningMessageSection.Location = new System.Drawing.Point(0, 475);
            this.RunningMessageSection.Name = "RunningMessageSection";
            this.RunningMessageSection.Radius = 4;
            this.RunningMessageSection.Size = new System.Drawing.Size(796, 44);
            this.RunningMessageSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            this.RunningMessageSection.TabIndex = 11;
            // 
            // GroundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Name = "GroundForm";
            this.Text = "Ground Form";
            this.Load += new System.EventHandler(this.GroundForm_Load);
            this.BasePanel.ResumeLayout(false);
            this.RunningStatusSection.ResumeLayout(false);
            this.RunningStatusSectionMsgRegion.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionMsgZone.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionMsgZone.PerformLayout();
            this.RunningStatusSectionMsgRegionInfoZone.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionInfoZone.PerformLayout();
            this.RunningProgressSection.ResumeLayout(false);
            this.RunningMessageSection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion


        public ContainerPanel RunningMessageSection;
        public RichTextBox RunningMessageSectionRichTextBox;

        public ContainerPanel RunningStatusSection;
        public Panel RunningStatusSectionMsgRegion;
        public Label RunningStatusSectionMsgRegionLabelMsg;
        public Label RunningStatusSectionMsgRegionLabelMsg1;
        public Label RunningStatusSectionMsgRegionLabelMsg2;
        public Label RunningStatusSectionMsgRegionLabelMsg3;
        public Label RunningStatusSectionMsgRegionInfoZoneLabelInfo;

        public ContainerPanel RunningProgressSection;
        public ProgressBar RunningProgressSectionProgressBar;
        public Panel RunningStatusSectionMsgRegionMsgZone;
        public Panel RunningStatusSectionMsgRegionInfoZone;
        public Label RunningStatusSectionMsgRegionInfoZoneLabelInfo1;
    }
}