
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Dialogs
{
    partial class TextInputDialog
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
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.textButtonOk = new Ligg.RpaDesk.WinForm.Controls.TextButton();
            this.textButtonCancel = new Ligg.RpaDesk.WinForm.Controls.TextButton();
            this.BasePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroundPanel
            // 
            this.BasePanel.Controls.Add(this.textButtonCancel);
            this.BasePanel.Controls.Add(this.textButtonOk);
            this.BasePanel.Controls.Add(this.textBoxInput);
            this.BasePanel.Size = new System.Drawing.Size(310, 95);
            // 
            // textBoxInput
            // 
            this.textBoxInput.Location = new System.Drawing.Point(30, 22);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(254, 21);
            this.textBoxInput.TabIndex = 0;
            // 
            // textButtonOk
            // 
            this.textButtonOk.Checked = false;
            this.textButtonOk.HasBorder = false;
            this.textButtonOk.Location = new System.Drawing.Point(27, 58);
            this.textButtonOk.Name = "textButtonOk";
            this.textButtonOk.Radius = 0;
            this.textButtonOk.RoundStyle = RoundStyle.None;
            this.textButtonOk.CheckType = ControlCheckType.Focus;
            this.textButtonOk.Size = new System.Drawing.Size(75, 23);
            this.textButtonOk.TabIndex = 1;
            this.textButtonOk.Text = "OK";
            this.textButtonOk.UseVisualStyleBackColor = true;
            this.textButtonOk.Click += new System.EventHandler(this.textButtonOk_Click);
            // 
            // textButtonCancel
            // 
            this.textButtonCancel.Checked = false;
            this.textButtonCancel.HasBorder = false;
            this.textButtonCancel.Location = new System.Drawing.Point(209, 58);
            this.textButtonCancel.Name = "textButtonCancel";
            this.textButtonCancel.Radius = 0;
            this.textButtonCancel.RoundStyle = RoundStyle.None;
            this.textButtonCancel.CheckType = ControlCheckType.Focus;
            this.textButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.textButtonCancel.TabIndex = 2;
            this.textButtonCancel.Text = "Cancel";
            this.textButtonCancel.UseVisualStyleBackColor = true;
            this.textButtonCancel.Click += new System.EventHandler(this.textButtonCancel_Click);
            // 
            // TextInputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 125);
            this.KeyPreview = true;
            this.Name = "TextInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CheckInputDialog";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.TextInputDialog_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Dialog_KeyPress);
            this.BasePanel.ResumeLayout(false);
            this.BasePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxInput;
        private Ligg.RpaDesk.WinForm.Controls.TextButton textButtonOk;
        private Ligg.RpaDesk.WinForm.Controls.TextButton textButtonCancel;
    }
}