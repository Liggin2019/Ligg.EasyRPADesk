namespace Ligg.RpaDesk.WinForm.Controls
{
    partial class StatusLight
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusLight));
            this.pictureBoxLight = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelLabel = new System.Windows.Forms.Panel();
            this.labelText = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLight)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxLight
            // 
            this.pictureBoxLight.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxLight.BackgroundImage")));
            this.pictureBoxLight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBoxLight.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBoxLight.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxLight.Name = "pictureBoxLight";
            this.pictureBoxLight.Size = new System.Drawing.Size(57, 56);
            this.pictureBoxLight.TabIndex = 0;
            this.pictureBoxLight.TabStop = false;
            this.pictureBoxLight.Click += new System.EventHandler(this.pictureBoxLight_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "light_grey_72.png");
            this.imageList1.Images.SetKeyName(1, "light_green_72.png");
            this.imageList1.Images.SetKeyName(2, "light_red_72.png");
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.Window;
            this.panelTop.Controls.Add(this.panelLabel);
            this.panelTop.Controls.Add(this.pictureBoxLight);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(460, 56);
            this.panelTop.TabIndex = 2;
            // 
            // panelLabel
            // 
            this.panelLabel.Controls.Add(this.labelText);
            this.panelLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLabel.Location = new System.Drawing.Point(57, 0);
            this.panelLabel.Name = "panelLabel";
            this.panelLabel.Size = new System.Drawing.Size(337, 56);
            this.panelLabel.TabIndex = 1;
            // 
            // labelText
            // 
            this.labelText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelText.Location = new System.Drawing.Point(0, 0);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(337, 56);
            this.labelText.TabIndex = 2;
            this.labelText.Text = "labelName";
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxMessage.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxMessage.Location = new System.Drawing.Point(268, 95);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.ReadOnly = true;
            this.textBoxMessage.Size = new System.Drawing.Size(100, 14);
            this.textBoxMessage.TabIndex = 4;
            // 
            // StatusLight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.panelTop);
            this.Name = "StatusLight";
            this.Size = new System.Drawing.Size(460, 317);
            this.Load += new System.EventHandler(this.StatusLight_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.StatusLight_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLight)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelLabel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLight;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Panel panelLabel;
    }
}
