using System;
using System.Drawing;
using System.Windows.Forms;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.WinForm.Forms;
using Ligg.RpaDesk.WinForm.Helpers;
using Ligg.RpaDesk.WinForm.Resources;
using Ligg.WinFormBase;

using Ligg.RpaDesk.Parser.DataModels;
using  Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser;
using Ligg.RpaDesk.Parser.Helpers;

namespace Ligg.RpaDesk.WinForm.Dialogs
{
    public partial class TextInputDialog : BaseForm
    {
        public TextInputDialog()
        {
            InitializeComponent();
            Text = TextRes.plsInputText;
            textButtonOk.Text = TextRes.Ok;
            textButtonOk.BackColor = StyleSheet.ControlBackColor;
            textButtonOk.HasBorder = true;
            textButtonOk.CheckType = ControlCheckType.Focus;  
            textButtonOk.Font=new Font("Font:Arial", 8.7f, new FontStyle());
            textButtonOk.Checked = true;

            textButtonCancel.Text = TextRes.Cancel;
            textButtonCancel.BackColor = StyleSheet.ControlBackColor;
            textButtonCancel.HasBorder = true;
            textButtonOk.CheckType = ControlCheckType.Focus;
            textButtonOk.Font = new Font("Font:Arial", 8.7f, new FontStyle());

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            if (Owner != null)
            {
                Left = Owner.Location.X + (Owner.Width / 2 - Width / 2);
            }
            else
            {
                var rect = new Rectangle();
                rect = Screen.GetWorkingArea(this);
                Left = rect.Width / 2 - Width / 2;
                Top = rect.Height > Height ? (rect.Height / 2 - Height / 2) / 3 : 10;
            }
            //StartPosition = FormStartPosition.CenterParent;
        }

        private string _inputText;
        public string InputText
        {
            get { return _inputText; }
            set { _inputText = value; }
        }

        private bool _isOk;
        public bool IsOk
        {
            get { return _isOk; }
            set { _isOk = value; }
        }

        public string VerificationRule
        {
            get;
            set;
        }

        public string VerificationParams
        {
            get;
            set;
        }


        private void TextInputDialog_Load(object sender, EventArgs e)
        {
            if (VerificationRule.ToLower().Contains("password"))
            {
                textBoxInput.PasswordChar = '*';
            }

        }

        private void textButtonOk_Click(object sender, EventArgs e)
        {
            try
            {
                _isOk = false;
                if (string.IsNullOrEmpty(textBoxInput.Text))
                {
                    MessageHelper.Popup(TextRes.Input + ValidationRes.CanNotBeNull);
                    return;
                }

                if (VerificationRule.IsNullOrEmpty()) _isOk = true;
                else _isOk = GetHelper.VerifyPassword(textBoxInput.Text, VerificationRule, VerificationParams);
                if (_isOk)
                {
                    _inputText = textBoxInput.Text;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageHelper.Popup(ex.Message);
            }
        }


        private void Dialog_KeyPress(object sender, KeyPressEventArgs e)
        {
            //'enter'
            if (e.KeyChar == '\r')
            {
                textButtonOk_Click(null, null);
            }
            //'esc'
            if ((int)(e.KeyChar) == Convert.ToChar(Keys.Escape))
            {
                _isOk = false;
                this.Close();
            }
        }

        private void textButtonCancel_Click(object sender, EventArgs e)
        {
            _isOk = false;
            this.Close();
        }


    }
}
