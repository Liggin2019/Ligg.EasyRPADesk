namespace Ligg.WinForm.Controls
{
    partial class Pager
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pager));
            this.textBoxGoToPageNo = new System.Windows.Forms.TextBox();
            this.linkLabelLast = new System.Windows.Forms.LinkLabel();
            this.linkLabelNext = new System.Windows.Forms.LinkLabel();
            this.linkLabelPrev = new System.Windows.Forms.LinkLabel();
            this.linkLabelFirst = new System.Windows.Forms.LinkLabel();
            this.labelCurrentPageNo = new System.Windows.Forms.Label();
            this.labelTotalRecordCountText = new System.Windows.Forms.Label();
            this.labelTotalRecordCount = new System.Windows.Forms.Label();
            this.labelPageSize = new System.Windows.Forms.Label();
            this.labelSlash = new System.Windows.Forms.Label();
            this.labelTotalPageNo = new System.Windows.Forms.Label();
            this.textBoxPageSize = new System.Windows.Forms.TextBox();
            this.labelPage = new System.Windows.Forms.Label();
            this.labelGoToText = new System.Windows.Forms.Label();
            this.pictureBoxGo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGo)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxGoToPageNo
            // 
            this.textBoxGoToPageNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBoxGoToPageNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.textBoxGoToPageNo.Location = new System.Drawing.Point(459, 1);
            this.textBoxGoToPageNo.Name = "textBoxGoToPageNo";
            this.textBoxGoToPageNo.Size = new System.Drawing.Size(33, 20);
            this.textBoxGoToPageNo.TabIndex = 46;
            this.textBoxGoToPageNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxGoToPageNo_KeyPress);
            // 
            // linkLabelLast
            // 
            this.linkLabelLast.AutoSize = true;
            this.linkLabelLast.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabelLast.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.linkLabelLast.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelLast.LinkColor = System.Drawing.Color.Black;
            this.linkLabelLast.Location = new System.Drawing.Point(388, 1);
            this.linkLabelLast.Name = "linkLabelLast";
            this.linkLabelLast.Size = new System.Drawing.Size(30, 15);
            this.linkLabelLast.TabIndex = 45;
            this.linkLabelLast.TabStop = true;
            this.linkLabelLast.Text = "Last";
            this.linkLabelLast.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelLast.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLast_LinkClicked);
            // 
            // linkLabelNext
            // 
            this.linkLabelNext.AutoSize = true;
            this.linkLabelNext.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabelNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.linkLabelNext.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelNext.LinkColor = System.Drawing.Color.Black;
            this.linkLabelNext.Location = new System.Drawing.Point(356, 1);
            this.linkLabelNext.Name = "linkLabelNext";
            this.linkLabelNext.Size = new System.Drawing.Size(32, 15);
            this.linkLabelNext.TabIndex = 44;
            this.linkLabelNext.TabStop = true;
            this.linkLabelNext.Text = "Next";
            this.linkLabelNext.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNext_LinkClicked);
            // 
            // linkLabelPrev
            // 
            this.linkLabelPrev.AutoSize = true;
            this.linkLabelPrev.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabelPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.linkLabelPrev.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelPrev.LinkColor = System.Drawing.Color.Black;
            this.linkLabelPrev.Location = new System.Drawing.Point(325, 1);
            this.linkLabelPrev.Name = "linkLabelPrev";
            this.linkLabelPrev.Size = new System.Drawing.Size(31, 15);
            this.linkLabelPrev.TabIndex = 43;
            this.linkLabelPrev.TabStop = true;
            this.linkLabelPrev.Text = "Prev";
            this.linkLabelPrev.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelPrev.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPrev_LinkClicked);
            // 
            // linkLabelFirst
            // 
            this.linkLabelFirst.AutoSize = true;
            this.linkLabelFirst.Dock = System.Windows.Forms.DockStyle.Left;
            this.linkLabelFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.linkLabelFirst.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelFirst.LinkColor = System.Drawing.Color.Black;
            this.linkLabelFirst.Location = new System.Drawing.Point(290, 1);
            this.linkLabelFirst.Name = "linkLabelFirst";
            this.linkLabelFirst.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.linkLabelFirst.Size = new System.Drawing.Size(35, 15);
            this.linkLabelFirst.TabIndex = 42;
            this.linkLabelFirst.TabStop = true;
            this.linkLabelFirst.Text = "First";
            this.linkLabelFirst.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabelFirst.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelFirst_LinkClicked);
            // 
            // labelCurrentPageNo
            // 
            this.labelCurrentPageNo.AutoSize = true;
            this.labelCurrentPageNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelCurrentPageNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelCurrentPageNo.ForeColor = System.Drawing.Color.Red;
            this.labelCurrentPageNo.Location = new System.Drawing.Point(37, 1);
            this.labelCurrentPageNo.Name = "labelCurrentPageNo";
            this.labelCurrentPageNo.Size = new System.Drawing.Size(14, 15);
            this.labelCurrentPageNo.TabIndex = 49;
            this.labelCurrentPageNo.Text = "1";
            this.labelCurrentPageNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTotalRecordCountText
            // 
            this.labelTotalRecordCountText.AutoSize = true;
            this.labelTotalRecordCountText.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelTotalRecordCountText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelTotalRecordCountText.Location = new System.Drawing.Point(75, 1);
            this.labelTotalRecordCountText.Name = "labelTotalRecordCountText";
            this.labelTotalRecordCountText.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelTotalRecordCountText.Size = new System.Drawing.Size(87, 15);
            this.labelTotalRecordCountText.TabIndex = 50;
            this.labelTotalRecordCountText.Text = "Record Count";
            this.labelTotalRecordCountText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelTotalRecordCount
            // 
            this.labelTotalRecordCount.AutoSize = true;
            this.labelTotalRecordCount.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelTotalRecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelTotalRecordCount.ForeColor = System.Drawing.Color.Red;
            this.labelTotalRecordCount.Location = new System.Drawing.Point(162, 1);
            this.labelTotalRecordCount.Name = "labelTotalRecordCount";
            this.labelTotalRecordCount.Size = new System.Drawing.Size(28, 15);
            this.labelTotalRecordCount.TabIndex = 51;
            this.labelTotalRecordCount.Text = "100";
            this.labelTotalRecordCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelPageSize
            // 
            this.labelPageSize.AutoSize = true;
            this.labelPageSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelPageSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelPageSize.Location = new System.Drawing.Point(190, 1);
            this.labelPageSize.Name = "labelPageSize";
            this.labelPageSize.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelPageSize.Size = new System.Drawing.Size(68, 15);
            this.labelPageSize.TabIndex = 53;
            this.labelPageSize.Text = "Page Size";
            this.labelPageSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelSlash
            // 
            this.labelSlash.AutoSize = true;
            this.labelSlash.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelSlash.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelSlash.Location = new System.Drawing.Point(51, 1);
            this.labelSlash.Name = "labelSlash";
            this.labelSlash.Size = new System.Drawing.Size(10, 15);
            this.labelSlash.TabIndex = 56;
            this.labelSlash.Text = "/";
            this.labelSlash.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTotalPageNo
            // 
            this.labelTotalPageNo.AutoSize = true;
            this.labelTotalPageNo.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelTotalPageNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelTotalPageNo.ForeColor = System.Drawing.Color.Red;
            this.labelTotalPageNo.Location = new System.Drawing.Point(61, 1);
            this.labelTotalPageNo.Name = "labelTotalPageNo";
            this.labelTotalPageNo.Size = new System.Drawing.Size(14, 15);
            this.labelTotalPageNo.TabIndex = 57;
            this.labelTotalPageNo.Text = "1";
            this.labelTotalPageNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxPageSize
            // 
            this.textBoxPageSize.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBoxPageSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.textBoxPageSize.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxPageSize.Location = new System.Drawing.Point(258, 1);
            this.textBoxPageSize.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxPageSize.Name = "textBoxPageSize";
            this.textBoxPageSize.Size = new System.Drawing.Size(32, 20);
            this.textBoxPageSize.TabIndex = 46;
            this.textBoxPageSize.Text = "100";
            this.textBoxPageSize.TextChanged += new System.EventHandler(this.textBoxPageSize_TextChanged);
            this.textBoxPageSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxPageSize_KeyPress);
            // 
            // labelPage
            // 
            this.labelPage.AutoSize = true;
            this.labelPage.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelPage.Location = new System.Drawing.Point(1, 1);
            this.labelPage.Name = "labelPage";
            this.labelPage.Size = new System.Drawing.Size(36, 15);
            this.labelPage.TabIndex = 1;
            this.labelPage.Text = "Page";
            this.labelPage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelGoToText
            // 
            this.labelGoToText.AutoSize = true;
            this.labelGoToText.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelGoToText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.labelGoToText.Location = new System.Drawing.Point(418, 1);
            this.labelGoToText.Name = "labelGoToText";
            this.labelGoToText.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelGoToText.Size = new System.Drawing.Size(41, 15);
            this.labelGoToText.TabIndex = 59;
            this.labelGoToText.Text = "Go to";
            this.labelGoToText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBoxGo
            // 
            this.pictureBoxGo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxGo.BackgroundImage")));
            this.pictureBoxGo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxGo.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxGo.Location = new System.Drawing.Point(492, 1);
            this.pictureBoxGo.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxGo.Name = "pictureBoxGo";
            this.pictureBoxGo.Size = new System.Drawing.Size(16, 20);
            this.pictureBoxGo.TabIndex = 60;
            this.pictureBoxGo.TabStop = false;
            this.pictureBoxGo.Visible = false;
            this.pictureBoxGo.Click += new System.EventHandler(this.pictureBoxGo_Click);
            // 
            // Pager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pictureBoxGo);
            this.Controls.Add(this.textBoxGoToPageNo);
            this.Controls.Add(this.labelGoToText);
            this.Controls.Add(this.linkLabelLast);
            this.Controls.Add(this.linkLabelNext);
            this.Controls.Add(this.linkLabelPrev);
            this.Controls.Add(this.linkLabelFirst);
            this.Controls.Add(this.textBoxPageSize);
            this.Controls.Add(this.labelPageSize);
            this.Controls.Add(this.labelTotalRecordCount);
            this.Controls.Add(this.labelTotalRecordCountText);
            this.Controls.Add(this.labelTotalPageNo);
            this.Controls.Add(this.labelSlash);
            this.Controls.Add(this.labelCurrentPageNo);
            this.Controls.Add(this.labelPage);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(78)))), ((int)(((byte)(151)))));
            this.Name = "Pager";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Size = new System.Drawing.Size(511, 22);
            this.Load += new System.EventHandler(this.Pager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxGoToPageNo;
        private System.Windows.Forms.LinkLabel linkLabelLast;
        private System.Windows.Forms.LinkLabel linkLabelNext;
        private System.Windows.Forms.LinkLabel linkLabelPrev;
        private System.Windows.Forms.LinkLabel linkLabelFirst;
        private System.Windows.Forms.Label labelCurrentPageNo;
        private System.Windows.Forms.Label labelTotalRecordCountText;
        private System.Windows.Forms.Label labelTotalRecordCount;
        private System.Windows.Forms.Label labelPageSize;
        private System.Windows.Forms.Label labelSlash;
        private System.Windows.Forms.Label labelTotalPageNo;
        private System.Windows.Forms.TextBox textBoxPageSize;
        private System.Windows.Forms.Label labelPage;
        private System.Windows.Forms.Label labelGoToText;
        private System.Windows.Forms.PictureBox pictureBoxGo;

    }
}
