
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.WinFormBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;


namespace Ligg.RpaDesk.WinForm.Controls
{
    public partial class DataGridViewEx : UserControl
    {
        public DataGridViewEx()
        {
            InitializeComponent();

        }

        //#event
        public event EventHandler OnPagerClick;

        private GetDataGridViewExValueOption _getValueType;
        private string _dataFilePath = "";
        private TxtDataType _dataSourceType;
        private DataTable _dataSourceObject = null;
        public string DataFilePath
        {
            set { _dataFilePath = value; }
        }

        public string DataSourceText
        {
            set
            {
                try
                {
                    var dataSourceStr = value;
                    if (_dataSourceType == TxtDataType.Undefined)
                        dataSourceStr = FileHelper.GetPath(dataSourceStr, _dataFilePath);
                    _dataSourceObject = dataSourceStr.ConvertToGeneric<DataTable>(true, _dataSourceType);

                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + "." + "SetDataSourceText Error: ControlName=" + Name + "; _dataSourceType=" + _dataSourceType.ToString() + "; " + ex.Message);
                }
            }
        }

        public string Value
        {
            get
            {
                var rst = "";
                if (_dataSourceObject == null) return string.Empty;
                if (_dataSourceObject.Rows.Count == 0) return string.Empty;
                //if (_getValueType == GetDataGridViewExValueOption.FullData) 
                //var dt= _dataSourceObject.Clone();
                var dt = new DataTable();
                if (_getValueType == GetDataGridViewExValueOption.FullData)
                {
                    for (int count = 0; count < dataGridView.Columns.Count; count++)
                    {
                        var dataColumn = new DataColumn(dataGridView.Columns[count].Name.ToString());
                        dt.Columns.Add(dataColumn);
                    }
                    for (int count = 0; count < dataGridView.Rows.Count; count++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            dr[i] = Convert.ToString(dataGridView.Rows[count].Cells[i].Value);
                        }
                        dt.Rows.Add(dr);
                    }

                    rst = dt.ConvertToJson();
                }

                else if (_getValueType == GetDataGridViewExValueOption.SelectededRowsIds)
                {
                    var selectedRowCount = dataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
                    if (selectedRowCount > 0)
                    {
                        for (int i = 0; i < selectedRowCount; i++)
                        {
                            var index = dataGridView.SelectedRows[i].Index;
                            var val = Convert.ToString(dataGridView.Rows[index].Cells["Id"].Value);
                            rst = i == 0 ? val : (rst + "," + val);
                        }

                    }
                }


                return rst;
            }
            set
            {
               
            }
        }

        public new string Text
        {
            get;
            private set;
        }

        private List<Annex> _annexList = new List<Annex>();
        public List<Annex> AnnexList
        {
            private get
            {
                return _annexList;
            }
            set
            {
                if (value != null)
                {
                    _annexList = value;
                }
            }
        }
        public string StyleText
        {
            set
            {
                dataGridView.AllowUserToAddRows = false;
                if (!string.IsNullOrEmpty(value))
                {
                    var dict = value.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var dataSrcTypeStr = dict.GetLdictValue("DataSourceType");
                    _dataSourceType = dataSrcTypeStr.GetTextDataType();

                    var getValueTypeStr = dict.GetLdictValue("GetValueType");
                    _getValueType = EnumHelper.GetByName<GetDataGridViewExValueOption>(getValueTypeStr, GetDataGridViewExValueOption.FullData);

                    var readOnly = dict.GetLdictValue("ReadOnly").ToLower() == "true";
                    dataGridView.ReadOnly = readOnly;

                    ControlBaseHelper.SetControlBackColor(dataGridView, value);
                    ControlBaseHelper.SetControlForeColor(dataGridView, value);
                    ControlBaseHelper.SetControlFont(dataGridView, value);
                }

            }
        }

        private void DataGridViewEx_Load(object sender, EventArgs e)
        {
            RefreshDataSource();
        }
        public void SetTextByCulture()
        {
        }


        public void RefreshDataSource()
        {
            dataGridView.DataSource = _dataSourceObject;

            if (_dataSourceObject != null)
            {
                int width = 0;
                for (int i = 0; i < this.dataGridView.Columns.Count; i++)
                {
                    //将每一列都调整为自动适应模式
                    this.dataGridView.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);
                    //记录整个DataGridView的宽度
                    width += this.dataGridView.Columns[i].Width;
                }
                //判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，
                //则将DataGridView的列自动调整模式设置为显示的列即可，
                //如果是小于原来设定的宽度，将模式改为填充。
                if (width > this.dataGridView.Size.Width)
                {
                    this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                }
                else
                {
                    this.dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
                //冻结某列 从左开始 0，1，2
                dataGridView.Columns[1].Frozen = true;
            }
        }

        private void AddRows(string str)
        {
        }


    }

    public enum GetDataGridViewExValueOption
    {
        FullData = 0,
        SelectededRowsIds = 1,

    }
}
