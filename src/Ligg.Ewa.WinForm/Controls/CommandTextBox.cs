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

namespace Ligg.EasyWinApp.WinForm.Controls
{
    public partial class CommandTextBox : UserControl
    {
        public CommandTextBox()
        {
            InitializeComponent();
            textBox.AutoSize = false;
            //textBox.ForeColor= Color.FromArgb(180, 180, 180);
            //textBox.BackColor = Color.Transparent;
        }

        //#event
        public event EventHandler OnEnterKeyDown;

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
        private void textBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_count == 0)
            {
                textBox.Text = "";
            }

            textBox.ForeColor = CustForeColor;
            _count++;
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 13)
            {
                if (OnEnterKeyDown != null)
                {
                    OnEnterKeyDown(this, null);
                }

            }
        }

        private void CommandTextBox_BackColorChanged(object sender, EventArgs e)
        {
            textBox.BackColor = BackColor;
        }

        private void CommandTextBox_ForeColorChanged(object sender, EventArgs e)
        {
            textBox.ForeColor = ForeColor;
        }
    }
}
