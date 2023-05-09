using System.Windows.Forms;
using Ligg.RpaDesk.WinForm.Controls;
using Ligg.RpaDesk.WinForm.Helpers;

namespace Ligg.RpaDesk.WinForm.Forms
{
    partial class FrameForm
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

        //sv++
        protected void toolStrip_Paint(object sender, PaintEventArgs e)
        {
            var toolStrip1 = sender as ToolStrip;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            if (toolStrip1.RenderMode == ToolStripRenderMode.System)
            {
                var rect = new System.Drawing.Rectangle(0, 0, toolStrip1.Width, toolStrip1.Height - 2);
                e.Graphics.SetClip(rect);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameForm));
            this.TopNavSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.TopNavSectionRightRegion = new System.Windows.Forms.Panel();
            this.TopNavSectionCenterRegion = new System.Windows.Forms.Panel();
            this.TopNavSectionLeftRegion = new System.Windows.Forms.Panel();
            this.ToolBarSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.ToolBarSectionRightRegion = new System.Windows.Forms.Panel();
            this.ToolBarSectionCenterRegion = new System.Windows.Forms.Panel();
            this.ToolBarSectionPublicRegion = new System.Windows.Forms.Panel();
            this.ToolBarSectionPublicRegionToolStrip = new System.Windows.Forms.ToolStrip();
            this.ToolBarSectionLeftRegion = new System.Windows.Forms.Panel();
            this.MiddleNavSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.MiddleNavSectionCenterRegion = new System.Windows.Forms.Panel();
            this.MiddleNavSectionRightRegion = new System.Windows.Forms.Panel();
            this.MiddleNavSectionLeftRegion = new System.Windows.Forms.Panel();
            this.DownNavSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.DownNavSectionCenterRegion = new System.Windows.Forms.Panel();
            this.DownNavSectionRightRegion = new System.Windows.Forms.Panel();
            this.DownNavSectionLeftRegion = new System.Windows.Forms.Panel();
            this.MainSection = new Ligg.RpaDesk.WinForm.Controls.ContainerPanel();
            this.MainSectionMainDivision = new System.Windows.Forms.Panel();
            this.MainSectionMainDivisionDownRegion = new System.Windows.Forms.Panel();
            this.MainSectionMainDivisionMiddleRegion = new System.Windows.Forms.Panel();
            this.MainSectionMainDivisionUpRegion = new System.Windows.Forms.Panel();
            this.MainSectionRightDivision = new System.Windows.Forms.Panel();
            this.MainSectionRightDivisionDownRegion = new System.Windows.Forms.Panel();
            this.MainSectionRightDivisionMiddleRegion = new System.Windows.Forms.Panel();
            this.MainSectionRightDivisionUpRegion = new System.Windows.Forms.Panel();
            this.MainSectionHorizontalResizeDivision1 = new System.Windows.Forms.Panel();
            this.LeftHorizontalResizeButton1 = new System.Windows.Forms.PictureBox();
            this.MainSectionSplitter1 = new System.Windows.Forms.Splitter();
            this.MainSectionLeftNavDivision1 = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivision1DownRegion = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivision1MiddleRegion = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivision1UpRegion = new System.Windows.Forms.Panel();
            this.MainSectionHorizontalResizeDivision = new System.Windows.Forms.Panel();
            this.LeftHorizontalResizeButton = new System.Windows.Forms.PictureBox();
            this.MainSectionSplitter = new System.Windows.Forms.Splitter();
            this.MainSectionLeftNavDivision = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivisionDownRegion = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivisionMiddleRegion = new System.Windows.Forms.Panel();
            this.MainSectionLeftNavDivisionUpRegion = new System.Windows.Forms.Panel();
            this.PictureList = new System.Windows.Forms.ImageList(this.components);
            this.RunningMessageSection.SuspendLayout();
            this.RunningStatusSection.SuspendLayout();
            this.RunningStatusSectionMsgRegion.SuspendLayout();
            this.RunningProgressSection.SuspendLayout();
            this.RunningStatusSectionMsgRegionMsgZone.SuspendLayout();
            this.RunningStatusSectionMsgRegionInfoZone.SuspendLayout();
            this.BasePanel.SuspendLayout();
            this.TopNavSection.SuspendLayout();
            this.ToolBarSection.SuspendLayout();
            this.ToolBarSectionPublicRegion.SuspendLayout();
            this.MiddleNavSection.SuspendLayout();
            this.DownNavSection.SuspendLayout();
            this.MainSection.SuspendLayout();
            this.MainSectionMainDivision.SuspendLayout();
            this.MainSectionRightDivision.SuspendLayout();
            this.MainSectionHorizontalResizeDivision1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeftHorizontalResizeButton1)).BeginInit();
            this.MainSectionLeftNavDivision1.SuspendLayout();
            this.MainSectionHorizontalResizeDivision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LeftHorizontalResizeButton)).BeginInit();
            this.MainSectionLeftNavDivision.SuspendLayout();
            this.SuspendLayout();
            // 
            // RunningMessageSection
            // 
            this.RunningMessageSection.BackColor = System.Drawing.SystemColors.Window;
            this.RunningMessageSection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.RunningMessageSection.Location = new System.Drawing.Point(0, 639);
            this.RunningMessageSection.Padding = new System.Windows.Forms.Padding(2);
            this.RunningMessageSection.Size = new System.Drawing.Size(1020, 49);
            // 
            // RunningMessageSectionRichTextBox
            // 
            this.RunningMessageSectionRichTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.RunningMessageSectionRichTextBox.Location = new System.Drawing.Point(2, 2);
            this.RunningMessageSectionRichTextBox.Size = new System.Drawing.Size(1016, 45);
            // 
            // RunningStatusSection
            // 
            this.RunningStatusSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.RunningStatusSection.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.RunningStatusSection.Location = new System.Drawing.Point(0, 718);
            this.RunningStatusSection.Size = new System.Drawing.Size(1020, 24);
            // 
            // RunningStatusSectionMsgRegion
            // 
            this.RunningStatusSectionMsgRegion.Size = new System.Drawing.Size(558, 22);
            // 
            // RunningStatusSectionMsgRegionLabelMsg
            // 
            this.RunningStatusSectionMsgRegionLabelMsg.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg.Text = "";
            // 
            // RunningStatusSectionMsgRegionLabelMsg1
            // 
            this.RunningStatusSectionMsgRegionLabelMsg1.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg1.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg1.Text = "";
            // 
            // RunningStatusSectionMsgRegionLabelMsg2
            // 
            this.RunningStatusSectionMsgRegionLabelMsg2.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg2.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg2.Text = "";
            // 
            // RunningStatusSectionMsgRegionLabelMsg3
            // 
            this.RunningStatusSectionMsgRegionLabelMsg3.Location = new System.Drawing.Point(0, 4);
            this.RunningStatusSectionMsgRegionLabelMsg3.Size = new System.Drawing.Size(0, 12);
            this.RunningStatusSectionMsgRegionLabelMsg3.Text = "";
            // 
            // RunningStatusSectionMsgRegionInfoZoneLabelInfo
            // 
            this.RunningStatusSectionMsgRegionInfoZoneLabelInfo.Size = new System.Drawing.Size(68, 14);
            // 
            // RunningProgressSection
            // 
            this.RunningProgressSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.RunningProgressSection.Location = new System.Drawing.Point(0, 688);
            this.RunningProgressSection.Size = new System.Drawing.Size(1020, 30);
            // 
            // RunningProgressSectionProgressBar
            // 
            this.RunningProgressSectionProgressBar.Size = new System.Drawing.Size(1020, 30);
            // 
            // RunningStatusSectionMsgRegionMsgZone
            // 
            this.RunningStatusSectionMsgRegionMsgZone.Size = new System.Drawing.Size(146, 22);
            // 
            // RunningStatusSectionMsgRegionInfoZone
            // 
            this.RunningStatusSectionMsgRegionInfoZone.Location = new System.Drawing.Point(479, 0);
            this.RunningStatusSectionMsgRegionInfoZone.Size = new System.Drawing.Size(79, 22);
            // 
            // BasePanel
            // 
            this.BasePanel.Controls.Add(this.MainSection);
            this.BasePanel.Controls.Add(this.DownNavSection);
            this.BasePanel.Controls.Add(this.MiddleNavSection);
            this.BasePanel.Controls.Add(this.ToolBarSection);
            this.BasePanel.Controls.Add(this.TopNavSection);
            this.BasePanel.Size = new System.Drawing.Size(1020, 742);
            this.BasePanel.Controls.SetChildIndex(this.RunningStatusSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.RunningProgressSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.RunningMessageSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.TopNavSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.ToolBarSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.MiddleNavSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.DownNavSection, 0);
            this.BasePanel.Controls.SetChildIndex(this.MainSection, 0);
            // 
            // TopNavSection
            // 
            this.TopNavSection.BackColor = System.Drawing.Color.White;
            this.TopNavSection.BorderColor = System.Drawing.Color.Empty;
            this.TopNavSection.BorderWidthOnBottom = 0;
            this.TopNavSection.Controls.Add(this.TopNavSectionRightRegion);
            this.TopNavSection.Controls.Add(this.TopNavSectionCenterRegion);
            this.TopNavSection.Controls.Add(this.TopNavSectionLeftRegion);
            this.TopNavSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopNavSection.Location = new System.Drawing.Point(0, 0);
            this.TopNavSection.Name = "TopNavSection";
            this.TopNavSection.Radius = 4;
            this.TopNavSection.Size = new System.Drawing.Size(1020, 28);
            this.TopNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.TopNavSection.TabIndex = 22;
            // 
            // TopNavSectionRightRegion
            // 
            this.TopNavSectionRightRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this.TopNavSectionRightRegion.Location = new System.Drawing.Point(820, 0);
            this.TopNavSectionRightRegion.Name = "TopNavSectionRightRegion";
            this.TopNavSectionRightRegion.Size = new System.Drawing.Size(200, 28);
            this.TopNavSectionRightRegion.TabIndex = 24;
            // 
            // TopNavSectionCenterRegion
            // 
            this.TopNavSectionCenterRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.TopNavSectionCenterRegion.Location = new System.Drawing.Point(164, 0);
            this.TopNavSectionCenterRegion.Name = "TopNavSectionCenterRegion";
            this.TopNavSectionCenterRegion.Size = new System.Drawing.Size(152, 28);
            this.TopNavSectionCenterRegion.TabIndex = 23;
            // 
            // TopNavSectionLeftRegion
            // 
            this.TopNavSectionLeftRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.TopNavSectionLeftRegion.Location = new System.Drawing.Point(0, 0);
            this.TopNavSectionLeftRegion.Name = "TopNavSectionLeftRegion";
            this.TopNavSectionLeftRegion.Size = new System.Drawing.Size(164, 28);
            this.TopNavSectionLeftRegion.TabIndex = 20;
            // 
            // ToolBarSection
            // 
            this.ToolBarSection.BackColor = System.Drawing.Color.White;
            this.ToolBarSection.BorderColor = System.Drawing.Color.White;
            this.ToolBarSection.BorderWidthOnBottom = 0;
            this.ToolBarSection.Controls.Add(this.ToolBarSectionRightRegion);
            this.ToolBarSection.Controls.Add(this.ToolBarSectionCenterRegion);
            this.ToolBarSection.Controls.Add(this.ToolBarSectionPublicRegion);
            this.ToolBarSection.Controls.Add(this.ToolBarSectionLeftRegion);
            this.ToolBarSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolBarSection.Location = new System.Drawing.Point(0, 28);
            this.ToolBarSection.Name = "ToolBarSection";
            this.ToolBarSection.Radius = 4;
            this.ToolBarSection.Size = new System.Drawing.Size(1020, 59);
            this.ToolBarSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.ToolBarSection.TabIndex = 21;
            // 
            // ToolBarSectionRightRegion
            // 
            this.ToolBarSectionRightRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this.ToolBarSectionRightRegion.Location = new System.Drawing.Point(761, 0);
            this.ToolBarSectionRightRegion.Name = "ToolBarSectionRightRegion";
            this.ToolBarSectionRightRegion.Size = new System.Drawing.Size(154, 59);
            this.ToolBarSectionRightRegion.TabIndex = 25;
            // 
            // ToolBarSectionCenterRegion
            // 
            this.ToolBarSectionCenterRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.ToolBarSectionCenterRegion.Location = new System.Drawing.Point(158, 0);
            this.ToolBarSectionCenterRegion.Name = "ToolBarSectionCenterRegion";
            this.ToolBarSectionCenterRegion.Size = new System.Drawing.Size(127, 59);
            this.ToolBarSectionCenterRegion.TabIndex = 24;
            // 
            // ToolBarSectionPublicRegion
            // 
            this.ToolBarSectionPublicRegion.Controls.Add(this.ToolBarSectionPublicRegionToolStrip);
            this.ToolBarSectionPublicRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this.ToolBarSectionPublicRegion.Location = new System.Drawing.Point(915, 0);
            this.ToolBarSectionPublicRegion.Name = "ToolBarSectionPublicRegion";
            this.ToolBarSectionPublicRegion.Size = new System.Drawing.Size(105, 59);
            this.ToolBarSectionPublicRegion.TabIndex = 23;
            // 
            // ToolBarSectionPublicRegionToolStrip
            // 
            this.ToolBarSectionPublicRegionToolStrip.AutoSize = false;
            this.ToolBarSectionPublicRegionToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(145)))), ((int)(((byte)(242)))));
            this.ToolBarSectionPublicRegionToolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolBarSectionPublicRegionToolStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 4, 2);
            this.ToolBarSectionPublicRegionToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolBarSectionPublicRegionToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolBarSectionPublicRegionToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolBarSectionPublicRegionToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolBarSectionPublicRegionToolStrip.Name = "ToolBarSectionPublicRegionToolStrip";
            this.ToolBarSectionPublicRegionToolStrip.Size = new System.Drawing.Size(105, 59);
            this.ToolBarSectionPublicRegionToolStrip.TabIndex = 22;
            this.ToolBarSectionPublicRegionToolStrip.Tag = "$Main";

            //sv++
            //ToolBarSectionPublicRegionToolStrip.BackColor = Color.Transparent;
            //ToolBarSectionPublicRegionToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            ToolBarSectionPublicRegionToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            ToolBarSectionPublicRegionToolStrip.Paint += new System.Windows.Forms.PaintEventHandler(toolStrip_Paint);

            // 
            // ToolBarSectionLeftRegion
            // 
            this.ToolBarSectionLeftRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.ToolBarSectionLeftRegion.Location = new System.Drawing.Point(0, 0);
            this.ToolBarSectionLeftRegion.Name = "ToolBarSectionLeftRegion";
            this.ToolBarSectionLeftRegion.Size = new System.Drawing.Size(158, 59);
            this.ToolBarSectionLeftRegion.TabIndex = 19;
            // 
            // MiddleNavSection
            // 
            this.MiddleNavSection.BorderColor = System.Drawing.Color.Empty;
            this.MiddleNavSection.BorderWidthOnBottom = 0;
            this.MiddleNavSection.Controls.Add(this.MiddleNavSectionCenterRegion);
            this.MiddleNavSection.Controls.Add(this.MiddleNavSectionRightRegion);
            this.MiddleNavSection.Controls.Add(this.MiddleNavSectionLeftRegion);
            this.MiddleNavSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.MiddleNavSection.Location = new System.Drawing.Point(0, 87);
            this.MiddleNavSection.Name = "MiddleNavSection";
            this.MiddleNavSection.Radius = 4;
            this.MiddleNavSection.Size = new System.Drawing.Size(1020, 26);
            this.MiddleNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.MiddleNavSection.TabIndex = 18;
            // 
            // MiddleNavSectionCenterRegion
            // 
            this.MiddleNavSectionCenterRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.MiddleNavSectionCenterRegion.Location = new System.Drawing.Point(46, 0);
            this.MiddleNavSectionCenterRegion.Name = "MiddleNavSectionCenterRegion";
            this.MiddleNavSectionCenterRegion.Size = new System.Drawing.Size(128, 26);
            this.MiddleNavSectionCenterRegion.TabIndex = 18;
            // 
            // MiddleNavSectionRightRegion
            // 
            this.MiddleNavSectionRightRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this.MiddleNavSectionRightRegion.Location = new System.Drawing.Point(865, 0);
            this.MiddleNavSectionRightRegion.Name = "MiddleNavSectionRightRegion";
            this.MiddleNavSectionRightRegion.Size = new System.Drawing.Size(155, 26);
            this.MiddleNavSectionRightRegion.TabIndex = 17;
            // 
            // MiddleNavSectionLeftRegion
            // 
            this.MiddleNavSectionLeftRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.MiddleNavSectionLeftRegion.Location = new System.Drawing.Point(0, 0);
            this.MiddleNavSectionLeftRegion.Name = "MiddleNavSectionLeftRegion";
            this.MiddleNavSectionLeftRegion.Size = new System.Drawing.Size(46, 26);
            this.MiddleNavSectionLeftRegion.TabIndex = 16;
            // 
            // DownNavSection
            // 
            this.DownNavSection.BorderColor = System.Drawing.Color.Empty;
            this.DownNavSection.BorderWidthOnBottom = 0;
            this.DownNavSection.Controls.Add(this.DownNavSectionCenterRegion);
            this.DownNavSection.Controls.Add(this.DownNavSectionRightRegion);
            this.DownNavSection.Controls.Add(this.DownNavSectionLeftRegion);
            this.DownNavSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.DownNavSection.Location = new System.Drawing.Point(0, 113);
            this.DownNavSection.Name = "DownNavSection";
            this.DownNavSection.Radius = 4;
            this.DownNavSection.Size = new System.Drawing.Size(1020, 26);
            this.DownNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.DownNavSection.TabIndex = 15;
            // 
            // DownNavSectionCenterRegion
            // 
            this.DownNavSectionCenterRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.DownNavSectionCenterRegion.Location = new System.Drawing.Point(81, 0);
            this.DownNavSectionCenterRegion.Name = "DownNavSectionCenterRegion";
            this.DownNavSectionCenterRegion.Size = new System.Drawing.Size(78, 26);
            this.DownNavSectionCenterRegion.TabIndex = 15;
            // 
            // DownNavSectionRightRegion
            // 
            this.DownNavSectionRightRegion.Dock = System.Windows.Forms.DockStyle.Right;
            this.DownNavSectionRightRegion.Location = new System.Drawing.Point(896, 0);
            this.DownNavSectionRightRegion.Name = "DownNavSectionRightRegion";
            this.DownNavSectionRightRegion.Size = new System.Drawing.Size(124, 26);
            this.DownNavSectionRightRegion.TabIndex = 14;
            // 
            // DownNavSectionLeftRegion
            // 
            this.DownNavSectionLeftRegion.Dock = System.Windows.Forms.DockStyle.Left;
            this.DownNavSectionLeftRegion.Location = new System.Drawing.Point(0, 0);
            this.DownNavSectionLeftRegion.Name = "DownNavSectionLeftRegion";
            this.DownNavSectionLeftRegion.Size = new System.Drawing.Size(81, 26);
            this.DownNavSectionLeftRegion.TabIndex = 13;
            // 
            // MainSection
            // 
            this.MainSection.BorderColor = System.Drawing.Color.Empty;
            this.MainSection.BorderWidthOnBottom = 0;
            this.MainSection.Controls.Add(this.MainSectionMainDivision);
            this.MainSection.Controls.Add(this.MainSectionRightDivision);
            this.MainSection.Controls.Add(this.MainSectionHorizontalResizeDivision1);
            this.MainSection.Controls.Add(this.MainSectionSplitter1);
            this.MainSection.Controls.Add(this.MainSectionLeftNavDivision1);
            this.MainSection.Controls.Add(this.MainSectionHorizontalResizeDivision);
            this.MainSection.Controls.Add(this.MainSectionSplitter);
            this.MainSection.Controls.Add(this.MainSectionLeftNavDivision);
            this.MainSection.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSection.Location = new System.Drawing.Point(0, 139);
            this.MainSection.Name = "MainSection";
            this.MainSection.Radius = 4;
            this.MainSection.Size = new System.Drawing.Size(1020, 367);
            this.MainSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            this.MainSection.TabIndex = 12;
            // 
            // MainSectionMainDivision
            // 
            this.MainSectionMainDivision.BackColor = System.Drawing.Color.White;
            this.MainSectionMainDivision.Controls.Add(this.MainSectionMainDivisionDownRegion);
            this.MainSectionMainDivision.Controls.Add(this.MainSectionMainDivisionMiddleRegion);
            this.MainSectionMainDivision.Controls.Add(this.MainSectionMainDivisionUpRegion);
            this.MainSectionMainDivision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSectionMainDivision.Location = new System.Drawing.Point(371, 0);
            this.MainSectionMainDivision.Name = "MainSectionMainDivision";
            this.MainSectionMainDivision.Size = new System.Drawing.Size(522, 367);
            this.MainSectionMainDivision.TabIndex = 13;
            // 
            // MainSectionMainDivisionDownRegion
            // 
            this.MainSectionMainDivisionDownRegion.BackColor = System.Drawing.Color.White;
            this.MainSectionMainDivisionDownRegion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainSectionMainDivisionDownRegion.Location = new System.Drawing.Point(0, 309);
            this.MainSectionMainDivisionDownRegion.Name = "MainSectionMainDivisionDownRegion";
            this.MainSectionMainDivisionDownRegion.Size = new System.Drawing.Size(522, 58);
            this.MainSectionMainDivisionDownRegion.TabIndex = 2;
            // 
            // MainSectionMainDivisionMiddleRegion
            // 
            this.MainSectionMainDivisionMiddleRegion.BackColor = System.Drawing.Color.White;
            this.MainSectionMainDivisionMiddleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionMainDivisionMiddleRegion.Location = new System.Drawing.Point(0, 28);
            this.MainSectionMainDivisionMiddleRegion.Name = "MainSectionMainDivisionMiddleRegion";
            this.MainSectionMainDivisionMiddleRegion.Size = new System.Drawing.Size(522, 117);
            this.MainSectionMainDivisionMiddleRegion.TabIndex = 1;
            // 
            // MainSectionMainDivisionUpRegion
            // 
            this.MainSectionMainDivisionUpRegion.BackColor = System.Drawing.Color.White;
            this.MainSectionMainDivisionUpRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionMainDivisionUpRegion.Location = new System.Drawing.Point(0, 0);
            this.MainSectionMainDivisionUpRegion.Name = "MainSectionMainDivisionUpRegion";
            this.MainSectionMainDivisionUpRegion.Size = new System.Drawing.Size(522, 28);
            this.MainSectionMainDivisionUpRegion.TabIndex = 0;
            // 
            // MainSectionRightDivision
            // 
            this.MainSectionRightDivision.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionRightDivision.Controls.Add(this.MainSectionRightDivisionDownRegion);
            this.MainSectionRightDivision.Controls.Add(this.MainSectionRightDivisionMiddleRegion);
            this.MainSectionRightDivision.Controls.Add(this.MainSectionRightDivisionUpRegion);
            this.MainSectionRightDivision.Dock = System.Windows.Forms.DockStyle.Right;
            this.MainSectionRightDivision.Location = new System.Drawing.Point(893, 0);
            this.MainSectionRightDivision.Name = "MainSectionRightDivision";
            this.MainSectionRightDivision.Size = new System.Drawing.Size(127, 367);
            this.MainSectionRightDivision.TabIndex = 12;
            // 
            // MainSectionRightDivisionDownRegion
            // 
            this.MainSectionRightDivisionDownRegion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainSectionRightDivisionDownRegion.Location = new System.Drawing.Point(0, 332);
            this.MainSectionRightDivisionDownRegion.Name = "MainSectionRightDivisionDownRegion";
            this.MainSectionRightDivisionDownRegion.Size = new System.Drawing.Size(127, 35);
            this.MainSectionRightDivisionDownRegion.TabIndex = 2;
            // 
            // MainSectionRightDivisionMiddleRegion
            // 
            this.MainSectionRightDivisionMiddleRegion.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionRightDivisionMiddleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionRightDivisionMiddleRegion.Location = new System.Drawing.Point(0, 53);
            this.MainSectionRightDivisionMiddleRegion.Name = "MainSectionRightDivisionMiddleRegion";
            this.MainSectionRightDivisionMiddleRegion.Size = new System.Drawing.Size(127, 98);
            this.MainSectionRightDivisionMiddleRegion.TabIndex = 1;
            // 
            // MainSectionRightDivisionUpRegion
            // 
            this.MainSectionRightDivisionUpRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionRightDivisionUpRegion.Location = new System.Drawing.Point(0, 0);
            this.MainSectionRightDivisionUpRegion.Name = "MainSectionRightDivisionUpRegion";
            this.MainSectionRightDivisionUpRegion.Size = new System.Drawing.Size(127, 53);
            this.MainSectionRightDivisionUpRegion.TabIndex = 0;
            // 
            // MainSectionHorizontalResizeDivision1
            // 
            this.MainSectionHorizontalResizeDivision1.BackColor = System.Drawing.Color.White;
            this.MainSectionHorizontalResizeDivision1.Controls.Add(this.LeftHorizontalResizeButton1);
            this.MainSectionHorizontalResizeDivision1.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainSectionHorizontalResizeDivision1.Location = new System.Drawing.Point(332, 0);
            this.MainSectionHorizontalResizeDivision1.Name = "MainSectionHorizontalResizeDivision1";
            this.MainSectionHorizontalResizeDivision1.Size = new System.Drawing.Size(39, 367);
            this.MainSectionHorizontalResizeDivision1.TabIndex = 14;
            // 
            // LeftHorizontalResizeButton1
            // 
            this.LeftHorizontalResizeButton1.BackColor = System.Drawing.Color.Transparent;
            this.LeftHorizontalResizeButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LeftHorizontalResizeButton1.Dock = System.Windows.Forms.DockStyle.Top;
            this.LeftHorizontalResizeButton1.Location = new System.Drawing.Point(0, 0);
            this.LeftHorizontalResizeButton1.Name = "LeftHorizontalResizeButton1";
            this.LeftHorizontalResizeButton1.Size = new System.Drawing.Size(39, 50);
            this.LeftHorizontalResizeButton1.TabIndex = 99;
            this.LeftHorizontalResizeButton1.TabStop = false;
            this.LeftHorizontalResizeButton1.Click += new System.EventHandler(this.HorizontalResizeButton1_Click);
            // 
            // MainSectionSplitter1
            // 
            this.MainSectionSplitter1.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionSplitter1.Location = new System.Drawing.Point(331, 0);
            this.MainSectionSplitter1.Name = "MainSectionSplitter1";
            this.MainSectionSplitter1.Size = new System.Drawing.Size(1, 367);
            this.MainSectionSplitter1.TabIndex = 15;
            this.MainSectionSplitter1.TabStop = false;
            // 
            // MainSectionLeftNavDivision1
            // 
            this.MainSectionLeftNavDivision1.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionLeftNavDivision1.Controls.Add(this.MainSectionLeftNavDivision1DownRegion);
            this.MainSectionLeftNavDivision1.Controls.Add(this.MainSectionLeftNavDivision1MiddleRegion);
            this.MainSectionLeftNavDivision1.Controls.Add(this.MainSectionLeftNavDivision1UpRegion);
            this.MainSectionLeftNavDivision1.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainSectionLeftNavDivision1.Location = new System.Drawing.Point(193, 0);
            this.MainSectionLeftNavDivision1.Name = "MainSectionLeftNavDivision1";
            this.MainSectionLeftNavDivision1.Size = new System.Drawing.Size(138, 367);
            this.MainSectionLeftNavDivision1.TabIndex = 5;
            // 
            // MainSectionLeftNavDivision1DownRegion
            // 
            this.MainSectionLeftNavDivision1DownRegion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainSectionLeftNavDivision1DownRegion.Location = new System.Drawing.Point(0, 319);
            this.MainSectionLeftNavDivision1DownRegion.Name = "MainSectionLeftNavDivision1DownRegion";
            this.MainSectionLeftNavDivision1DownRegion.Size = new System.Drawing.Size(138, 48);
            this.MainSectionLeftNavDivision1DownRegion.TabIndex = 5;
            // 
            // MainSectionLeftNavDivision1MiddleRegion
            // 
            this.MainSectionLeftNavDivision1MiddleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionLeftNavDivision1MiddleRegion.Location = new System.Drawing.Point(0, 69);
            this.MainSectionLeftNavDivision1MiddleRegion.Name = "MainSectionLeftNavDivision1MiddleRegion";
            this.MainSectionLeftNavDivision1MiddleRegion.Size = new System.Drawing.Size(138, 49);
            this.MainSectionLeftNavDivision1MiddleRegion.TabIndex = 4;
            // 
            // MainSectionLeftNavDivision1UpRegion
            // 
            this.MainSectionLeftNavDivision1UpRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionLeftNavDivision1UpRegion.Location = new System.Drawing.Point(0, 0);
            this.MainSectionLeftNavDivision1UpRegion.Name = "MainSectionLeftNavDivision1UpRegion";
            this.MainSectionLeftNavDivision1UpRegion.Size = new System.Drawing.Size(138, 69);
            this.MainSectionLeftNavDivision1UpRegion.TabIndex = 3;
            // 
            // MainSectionHorizontalResizeDivision
            // 
            this.MainSectionHorizontalResizeDivision.BackColor = System.Drawing.Color.White;
            this.MainSectionHorizontalResizeDivision.Controls.Add(this.LeftHorizontalResizeButton);
            this.MainSectionHorizontalResizeDivision.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainSectionHorizontalResizeDivision.Location = new System.Drawing.Point(114, 0);
            this.MainSectionHorizontalResizeDivision.Name = "MainSectionHorizontalResizeDivision";
            this.MainSectionHorizontalResizeDivision.Size = new System.Drawing.Size(79, 367);
            this.MainSectionHorizontalResizeDivision.TabIndex = 11;
            // 
            // LeftHorizontalResizeButton
            // 
            this.LeftHorizontalResizeButton.BackColor = System.Drawing.Color.Transparent;
            this.LeftHorizontalResizeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LeftHorizontalResizeButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.LeftHorizontalResizeButton.Location = new System.Drawing.Point(0, 0);
            this.LeftHorizontalResizeButton.Name = "LeftHorizontalResizeButton";
            this.LeftHorizontalResizeButton.Size = new System.Drawing.Size(79, 50);
            this.LeftHorizontalResizeButton.TabIndex = 7;
            this.LeftHorizontalResizeButton.TabStop = false;
            this.LeftHorizontalResizeButton.Click += new System.EventHandler(this.HorizontalResizeButton_Click);
            // 
            // MainSectionSplitter
            // 
            this.MainSectionSplitter.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionSplitter.Location = new System.Drawing.Point(113, 0);
            this.MainSectionSplitter.Name = "MainSectionSplitter";
            this.MainSectionSplitter.Size = new System.Drawing.Size(1, 367);
            this.MainSectionSplitter.TabIndex = 6;
            this.MainSectionSplitter.TabStop = false;
            // 
            // MainSectionLeftNavDivision
            // 
            this.MainSectionLeftNavDivision.BackColor = System.Drawing.SystemColors.Window;
            this.MainSectionLeftNavDivision.Controls.Add(this.MainSectionLeftNavDivisionDownRegion);
            this.MainSectionLeftNavDivision.Controls.Add(this.MainSectionLeftNavDivisionMiddleRegion);
            this.MainSectionLeftNavDivision.Controls.Add(this.MainSectionLeftNavDivisionUpRegion);
            this.MainSectionLeftNavDivision.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainSectionLeftNavDivision.Location = new System.Drawing.Point(0, 0);
            this.MainSectionLeftNavDivision.Name = "MainSectionLeftNavDivision";
            this.MainSectionLeftNavDivision.Size = new System.Drawing.Size(113, 367);
            this.MainSectionLeftNavDivision.TabIndex = 2;
            // 
            // MainSectionLeftNavDivisionDownRegion
            // 
            this.MainSectionLeftNavDivisionDownRegion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainSectionLeftNavDivisionDownRegion.Location = new System.Drawing.Point(0, 335);
            this.MainSectionLeftNavDivisionDownRegion.Name = "MainSectionLeftNavDivisionDownRegion";
            this.MainSectionLeftNavDivisionDownRegion.Size = new System.Drawing.Size(113, 32);
            this.MainSectionLeftNavDivisionDownRegion.TabIndex = 2;
            // 
            // MainSectionLeftNavDivisionMiddleRegion
            // 
            this.MainSectionLeftNavDivisionMiddleRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionLeftNavDivisionMiddleRegion.Location = new System.Drawing.Point(0, 28);
            this.MainSectionLeftNavDivisionMiddleRegion.Name = "MainSectionLeftNavDivisionMiddleRegion";
            this.MainSectionLeftNavDivisionMiddleRegion.Size = new System.Drawing.Size(113, 70);
            this.MainSectionLeftNavDivisionMiddleRegion.TabIndex = 1;
            // 
            // MainSectionLeftNavDivisionUpRegion
            // 
            this.MainSectionLeftNavDivisionUpRegion.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainSectionLeftNavDivisionUpRegion.Location = new System.Drawing.Point(0, 0);
            this.MainSectionLeftNavDivisionUpRegion.Name = "MainSectionLeftNavDivisionUpRegion";
            this.MainSectionLeftNavDivisionUpRegion.Size = new System.Drawing.Size(113, 28);
            this.MainSectionLeftNavDivisionUpRegion.TabIndex = 0;
            // 
            // PictureList
            // 
            this.PictureList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("PictureList.ImageStream")));
            this.PictureList.TransparentColor = System.Drawing.Color.Transparent;
            this.PictureList.Images.SetKeyName(0, "go_left_old.png");
            this.PictureList.Images.SetKeyName(1, "go_right_old.png");
            this.PictureList.Images.SetKeyName(2, "go_left.png");
            this.PictureList.Images.SetKeyName(3, "go_right.png");
            // 
            // FrameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Name = "FrameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Frame Form";
            this.RunningMessageSection.ResumeLayout(false);
            this.RunningStatusSection.ResumeLayout(false);
            this.RunningStatusSectionMsgRegion.ResumeLayout(false);
            this.RunningProgressSection.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionMsgZone.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionMsgZone.PerformLayout();
            this.RunningStatusSectionMsgRegionInfoZone.ResumeLayout(false);
            this.RunningStatusSectionMsgRegionInfoZone.PerformLayout();
            this.BasePanel.ResumeLayout(false);
            this.TopNavSection.ResumeLayout(false);
            this.ToolBarSection.ResumeLayout(false);
            this.ToolBarSectionPublicRegion.ResumeLayout(false);
            this.MiddleNavSection.ResumeLayout(false);
            this.DownNavSection.ResumeLayout(false);
            this.MainSection.ResumeLayout(false);
            this.MainSectionMainDivision.ResumeLayout(false);
            this.MainSectionRightDivision.ResumeLayout(false);
            this.MainSectionHorizontalResizeDivision1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LeftHorizontalResizeButton1)).EndInit();
            this.MainSectionLeftNavDivision1.ResumeLayout(false);
            this.MainSectionHorizontalResizeDivision.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LeftHorizontalResizeButton)).EndInit();
            this.MainSectionLeftNavDivision.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion
        protected ContainerPanel TopNavSection;
        protected ContainerPanel ToolBarSection;
        protected ContainerPanel MiddleNavSection;
        protected ContainerPanel DownNavSection;
        protected ContainerPanel MainSection;

        protected System.Windows.Forms.Panel DownNavSectionLeftRegion;
        protected System.Windows.Forms.Panel MiddleNavSectionLeftRegion;
        protected System.Windows.Forms.Panel MiddleNavSectionRightRegion;
        protected System.Windows.Forms.Panel DownNavSectionRightRegion;

        protected System.Windows.Forms.Panel MainSectionLeftNavDivision;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivision1;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivisionMiddleRegion;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivisionUpRegion;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivision1MiddleRegion;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivision1UpRegion;
        protected System.Windows.Forms.Splitter MainSectionSplitter;
        protected System.Windows.Forms.Splitter MainSectionSplitter1;
        protected Panel ToolBarSectionPublicRegion;
        protected System.Windows.Forms.ToolStrip ToolBarSectionPublicRegionToolStrip;
        protected System.Windows.Forms.Panel MainSectionHorizontalResizeDivision;
        protected System.Windows.Forms.PictureBox LeftHorizontalResizeButton;
        protected System.Windows.Forms.PictureBox LeftHorizontalResizeButton1;
        protected System.Windows.Forms.Panel MainSectionRightDivision;
        protected System.Windows.Forms.Panel MainSectionMainDivision;
        protected System.Windows.Forms.Panel MainSectionMainDivisionUpRegion;
        protected System.Windows.Forms.Panel ToolBarSectionLeftRegion;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivisionDownRegion;
        protected System.Windows.Forms.Panel MainSectionLeftNavDivision1DownRegion;
        protected System.Windows.Forms.Panel MainSectionRightDivisionDownRegion;
        protected System.Windows.Forms.Panel MainSectionRightDivisionMiddleRegion;
        protected System.Windows.Forms.Panel MainSectionRightDivisionUpRegion;
        protected System.Windows.Forms.Panel MainSectionMainDivisionDownRegion;
        protected System.Windows.Forms.Panel MainSectionMainDivisionMiddleRegion;
        protected Panel TopNavSectionRightRegion;
        protected Panel TopNavSectionCenterRegion;
        protected Panel TopNavSectionLeftRegion;
        protected Panel MiddleNavSectionCenterRegion;
        protected Panel ToolBarSectionRightRegion;
        protected Panel ToolBarSectionCenterRegion;
        protected Panel DownNavSectionCenterRegion;
        protected Panel MainSectionHorizontalResizeDivision1;
        private ImageList PictureList;



    }
}