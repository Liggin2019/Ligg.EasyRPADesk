﻿using System.Windows.Forms;
using Ligg.EasyWinApp.WinForm.Controls;

namespace Ligg.EasyWinApp.WinForm.Forms
{
    partial class DilalogForm
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
            this.MainSection = new Ligg.EasyWinApp.WinForm.Controls.ContainerPanel();
            this.RunningMessageSection.SuspendLayout();
            this.RunningStatusSection.SuspendLayout();
            this.RunningStatusSectionBackTaskRegion.SuspendLayout();
            this.GroundPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // RunningMessageSection
            // 
            this.RunningMessageSection.BackColor = System.Drawing.SystemColors.Window;
            this.RunningMessageSection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.RunningMessageSection.Location = new System.Drawing.Point(0, 517);
            this.RunningMessageSection.Padding = new System.Windows.Forms.Padding(2);
            this.RunningMessageSection.Size = new System.Drawing.Size(916, 49);
            // 
            // RunningMessageSectionRichTextBox
            // 
            this.RunningMessageSectionRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.RunningMessageSectionRichTextBox.Location = new System.Drawing.Point(2, 3);
            this.RunningMessageSectionRichTextBox.Size = new System.Drawing.Size(912, 44);
            // 
            // RunningStatusSection
            // 
            this.RunningStatusSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.RunningStatusSection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.RunningStatusSection.Location = new System.Drawing.Point(0, 566);
            this.RunningStatusSection.Size = new System.Drawing.Size(916, 24);
            // 
            // RunningStatusSectionMsgRegion
            // 
            this.RunningStatusSectionMsgRegion.Size = new System.Drawing.Size(536, 22);
            // 
            // RunningStatusSectionMsgRegionLabelMsg2
            // 
            this.RunningStatusSectionMsgRegionLabelMsg2.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg2.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            // 
            // RunningStatusSectionMsgRegionLabelMsg1
            // 
            this.RunningStatusSectionMsgRegionLabelMsg1.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg1.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg1.Text = "";
            // 
            // RunningStatusSectionMsgRegionLabelMsg
            // 
            this.RunningStatusSectionMsgRegionLabelMsg.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg.Text = "";
            // 
            // RunningStatusSectionBackgroundTaskRegionImageTextButton
            // 
            this.RunningStatusSectionBackTaskRegionImageTextButton.Location = new System.Drawing.Point(0, 0);
            this.RunningStatusSectionBackTaskRegionImageTextButton.Size = new System.Drawing.Size(150, 22);
            this.RunningStatusSectionBackTaskRegionImageTextButton.Text = " Task List 0/0";
            // 
            // RunningStatusSectionBackgroundTaskRegion
            // 
            this.RunningStatusSectionBackTaskRegion.Location = new System.Drawing.Point(765, 1);
            this.RunningStatusSectionBackTaskRegion.Size = new System.Drawing.Size(150, 22);

            // 
            // RunningStatusSectionMsgRegionLabelMsg3
            // 
            this.RunningStatusSectionMsgRegionLabelMsg3.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg3.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg3.Text = "";

            // 
            // GroundPanel
            // 
            this.GroundPanel.BackColor = System.Drawing.Color.DarkOrange;
            this.GroundPanel.Controls.Add(this.MainSection);
            this.GroundPanel.Size = new System.Drawing.Size(916, 590);
            this.GroundPanel.Controls.SetChildIndex(this.MainSection, 0);
            this.GroundPanel.Controls.SetChildIndex(this.RunningStatusSection, 0);
            this.GroundPanel.Controls.SetChildIndex(this.RunningMessageSection, 0);
            // 
            // MainSection
            // 
            this.MainSection.BackColor = System.Drawing.Color.Red;
            this.MainSection.BorderColor = System.Drawing.Color.Empty;
            this.MainSection.BorderWidthOnBottom = 0;
            this.MainSection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSection.Location = new System.Drawing.Point(0, 0);
            this.MainSection.Name = "MainSection";
            this.MainSection.Radius = 4;
            this.MainSection.Size = new System.Drawing.Size(916, 590);
            this.MainSection.StyleType = Ligg.EasyWinApp.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.MainSection.TabIndex = 12;
            // 
            // ZoneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(920, 620);
            this.Name = "ZoneForm";
            this.Text = "Zone Form";
            this.RunningMessageSection.ResumeLayout(false);
            this.RunningStatusSection.ResumeLayout(false);
            this.RunningStatusSectionBackTaskRegion.ResumeLayout(false);
            this.GroundPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ContainerPanel MainSection;

        //public System.Windows.Forms.ToolStripLabel ToolBarSectionPublicRegionLabelMsg;


    }
}