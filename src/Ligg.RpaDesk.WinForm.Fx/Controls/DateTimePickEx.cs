using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.WinForm.Helpers;
using Ligg.WinFormBase;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ligg.RpaDesk.WinForm.Controls
{
    public partial class DateTimePickerEx : UserControl
    {
        public DateTimePickerEx()
        {
           
            InitializeComponent();
        }

        public event EventHandler ValueChanged;
        public string Value
        {
            get
            {
                return dateTimePicker.Value.ToString(dateTimePicker.CustomFormat);
            }
            set
            {
                dateTimePicker.Value = value.ParseToDateTime(dateTimePicker.CustomFormat);
            }
        }

        public string StyleText
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var dict = value.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var custFormat = dict.GetLdictValue("CustomFormat");
                    dateTimePicker.CustomFormat = custFormat;
                    ControlBaseHelper.SetControlBackColor(dateTimePicker, value);
                    ControlBaseHelper.SetControlForeColor(dateTimePicker, value);
                    ControlBaseHelper.SetControlFont(dateTimePicker, value);
                }

            }
        }

        private void DateTimePickerEx_Load(object sender, EventArgs e)
        {
            dateTimePicker.Format = DateTimePickerFormat.Custom;
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, null);
        }
    }
}
