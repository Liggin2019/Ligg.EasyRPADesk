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
    public partial class StatusLight : UserControl
    {
        public StatusLight()
        {
            InitializeComponent();
            ControlHelper.SetLabelStyleByText(labelText, StyleSheet.LabelClass_Level2);
            Undefine();
        }

        public event EventHandler OnLightClick;

        private ArrangementType _arrangementType;
        private int _msgHeight;
        private int _labelWidth;

        private bool _hideLight;
        private TxtDataType _dataSourceType;

        private UniversalResult _dataSourceObject;
        public string DataSourceText
        {
            //private get => _dataSource;
            set
            {
                try
                {
                    _dataSourceObject = value.ConvertToGeneric<UniversalResult>(true, _dataSourceType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + "." + "SetDataSourceText Error: ControlName=" + Name + "; _dataSourceType=" + _dataSourceType.ToString() + "; " + ex.Message);
                }

            }
        }

        private string _text;
        public override string Text
        {
            get => _text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _text = value;
                    labelText.Text = value;
                }
                else
                {
                    _text = "";
                    labelText.Text = "";
                }
            }
        }

        public bool Value
        {
            get
            {
                if (_dataSourceObject != null)
                {
                    return _dataSourceObject.Success;
                }
                return false;
            }
        }

        public string Message
        {
            get
            {
                if (_dataSourceObject != null)
                {
                    return _dataSourceObject.Message;
                }
                else return UniversalStatus.Undefined.ToString();

            }
        }

        public string StyleText
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var dict = value.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var argmtTypeStr = dict.GetLdictValue("ArrangementType");
                    _arrangementType = EnumHelper.GetByName(argmtTypeStr, ArrangementType.Horizonal);

                    if (_arrangementType == ArrangementType.Horizonal)
                    {
                        var lblWidthStr = dict.GetLdictValue("LabelWidth");
                        _labelWidth = lblWidthStr.IsPlusIntegerOrZero() ? Convert.ToInt32(lblWidthStr) : 0;
                        textBoxMessage.Multiline = false;
                    }
                    else
                    {
                        var msgHeightStr = dict.GetLdictValue("MsgHeight");
                        _msgHeight = msgHeightStr.IsPlusIntegerOrZero() ? Convert.ToInt32(msgHeightStr) : 26;
                        textBoxMessage.BorderStyle = BorderStyle.FixedSingle;
                        textBoxMessage.Multiline = true;
                        textBoxMessage.ScrollBars = ScrollBars.Vertical;
                    }

                    var msgHasBorderStr = dict.GetLdictValue("MsgHasBorder");
                    if (!msgHasBorderStr.IsNullOrEmpty())
                        if (msgHasBorderStr.ToLower() == "true")
                        {
                            textBoxMessage.BorderStyle = BorderStyle.FixedSingle;
                        }


                    var dataSourceTypeStr = dict.GetLdictValue("DataSourceType");
                    _dataSourceType = dataSourceTypeStr.GetTextDataType();


                    var hideLightStr = dict.GetLdictValue("HideLight");
                    if (!hideLightStr.IsNullOrEmpty())
                        _hideLight = hideLightStr.ToLower() == "true";

                    ControlBaseHelper.SetControlForeColor(labelText, value);
                    ControlBaseHelper.SetControlFont(labelText, value);

                    var msgForetColorStr = dict.GetLdictValue("MsgForeColor");
                    if (!msgForetColorStr.IsNullOrEmpty())
                        ControlBaseHelper.SetControlForeColor(textBoxMessage, "ForeColor:" + msgForetColorStr + "|");

                }

            }
        }

        private void StatusLight_Load(object sender, EventArgs e)
        {
            if (_hideLight) pictureBoxLight.Visible = false;
            RefreshDataSource();
        }

        private void StatusLight_Paint(object sender, PaintEventArgs e)
        {
            if (_arrangementType == ArrangementType.Horizonal)
            {
                panelTop.Dock = DockStyle.Left;

                pictureBoxLight.Width = _hideLight ? 0 : Height;
                pictureBoxLight.Height = Height;

                panelLabel.Width = _labelWidth;
                textBoxMessage.Left = pictureBoxLight.Width + panelLabel.Width;
                textBoxMessage.Top = (Height - textBoxMessage.Height) / 2;
                textBoxMessage.Width = Width - pictureBoxLight.Width - panelLabel.Width;

                if (textBoxMessage.Width == 0) textBoxMessage.Visible = false;
            }
            else
            {
                panelTop.Dock = DockStyle.Top;
                textBoxMessage.Dock = DockStyle.Top;
                textBoxMessage.Height = _msgHeight;
                if (_msgHeight == 0) textBoxMessage.Visible = false;

                panelTop.Height = Height - _msgHeight;
                pictureBoxLight.Height = panelTop.Height;
                pictureBoxLight.Width = _hideLight ? 0 : pictureBoxLight.Height;

                panelLabel.Width = panelTop.Width - pictureBoxLight.Width;
            }
        }

        private void pictureBoxLight_Click(object sender, EventArgs e)
        {
            OnLightClick?.Invoke(this, null);
        }

        public void RefreshDataSource()
        {
            if (_dataSourceObject == null)
            {
                Undefine();
                return;
            }

            SetLightColor(_dataSourceObject.Success);
            textBoxMessage.Text = _dataSourceObject.Message;
        }

        private void SetLightColor(bool success)
        {
            if (success)
            {
                pictureBoxLight.BackgroundImage = imageList1.Images[1];
            }
            else
            {
                pictureBoxLight.BackgroundImage = imageList1.Images[2];
            }
        }

        private void Undefine()
        {
            pictureBoxLight.BackgroundImage = imageList1.Images[0];
            textBoxMessage.Text = "";// UniversalStatus.Undefined.ToString();
        }

        private enum ArrangementType
        {
            Horizonal = 0,
            Vertical,
        }

        private enum DataSourceType
        {
            TextData = 0,
            UniversalResult = 1,
        }

    }
}
