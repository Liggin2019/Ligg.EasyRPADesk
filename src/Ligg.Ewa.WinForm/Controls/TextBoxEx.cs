using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ligg.WinForm.Controls
{
    public partial class TextBoxEx : UserControl
    {
        public TextBoxEx()
        {
            InitializeComponent();
            textBox.AutoSize = false;
        }

        //#event
        public event EventHandler OnTextChanged;
        public string Text
        {
            get => textBox.Text;
            set => textBox.Text = value;
        }

        public string Type
        {
            set
            {
                if (value.ToLower()=="password")
                    textBox.PasswordChar = '*';
            }
        }



        public Color CustForeColor;
        private Int64 _count = 0;
        private void TextBoxEx_BackColorChanged(object sender, EventArgs e)
        {
            textBox.BackColor = BackColor;
        }

        private void TextBoxEx_ForeColorChanged(object sender, EventArgs e)
        {
            textBox.ForeColor = ForeColor;
        }

        private void textBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_count == 0)
            {
                textBox.Text = "";
            }

            textBox.ForeColor = CustForeColor;
            _count++;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (OnTextChanged != null)
            {
                OnTextChanged(this, null);
            }
        }
    }
}
