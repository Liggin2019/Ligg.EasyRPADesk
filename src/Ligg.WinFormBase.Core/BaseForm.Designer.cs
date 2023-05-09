
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
            this.BasePanel.BackColor = System.Drawing.Color.White;
            this.BasePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BasePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BasePanel.Location = new System.Drawing.Point(2, 24);
            this.BasePanel.Name = "BasePanel";
            this.BasePanel.Size = new System.Drawing.Size(464, 366);
            this.BasePanel.TabIndex = 0;
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 392);
            this.Controls.Add(this.BasePanel);
            this.Name = "BaseForm";
            this.Text = "BaseForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel BasePanel;
    }
}