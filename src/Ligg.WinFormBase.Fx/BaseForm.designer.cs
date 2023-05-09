namespace Ligg.WinFormBase
{
    partial class BaseForm
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
            this.BasePanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // BasePanel
            // 
            this.BasePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.BasePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BasePanel.Location = new System.Drawing.Point(2, 28);
            this.BasePanel.Name = "BasePanel";
            this.BasePanel.Size = new System.Drawing.Size(296, 270);
            this.BasePanel.TabIndex = 0;
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.BasePanel);
            this.Name = "BaseForm";
            this.Text = "Base Form";
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel BasePanel;

    }
}