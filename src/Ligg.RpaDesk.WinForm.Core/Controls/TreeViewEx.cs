
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.WinFormBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ligg.Infrastructure.Utilities.DataParserUtil;


namespace Ligg.RpaDesk.WinForm.Controls
{
    public partial class TreeViewEx : UserControl
    {
        public TreeViewEx()
        {
            InitializeComponent();
        }

        //#event
        public event EventHandler LeafClick;

        private ImageList _imageList = new ImageList();
        private GetTreeItemValueOption _getValueType;
        private string _imageUrls;
        private string _dataFilePath = "";
        private TxtDataType _dataSourceType;
        private List<TreeItem> _dataSourceObject = new List<TreeItem>();
        private string _topId = "0";
        private string _openId;
        public string DataFilePath
        {
            set { _dataFilePath = value; }
        }

        public List<TreeItem> DataSourceObject
        {
            private get => _dataSourceObject;
            set
            {
                if (value != null)
                {
                    _dataSourceObject = value;
                }
                else
                {
                    _dataSourceObject = (List<TreeItem>)value;
                }
            }
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
                    _dataSourceObject = dataSourceStr.ConvertToGeneric<List<TreeItem>>(true, _dataSourceType);

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
                if (_getValueType == GetTreeItemValueOption.Id) return _openId;
                else if (_getValueType == GetTreeItemValueOption.DisplayName) return Text;
                else
                {
                    var treeItem = _dataSourceObject.Find(x => x.Id == _openId);
                    if (treeItem == null) return "";
                    if (_getValueType == GetTreeItemValueOption.Name) return treeItem.Name;
                    else if (_getValueType == GetTreeItemValueOption.Value) return treeItem.Value;
                    else return treeItem.Value1;
                }
            }
            set
            {
                _openId = value;
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
                if (!string.IsNullOrEmpty(value))
                {
                    var dict = value.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
                    var hasBorderStr = dict.GetLdictValue("HasBorder");
                    treeView.BorderStyle = BorderStyle.None;
                    if (hasBorderStr.ToLower() == "true")
                    {
                        treeView.BorderStyle = BorderStyle.FixedSingle;
                    }

                    var topIdStr = dict.GetLdictValue("TopId");
                    if (!topIdStr.IsNullOrEmpty()) _topId = topIdStr;

                    var dataSrcTypeStr = dict.GetLdictValue("DataSourceType");
                    _dataSourceType = dataSrcTypeStr.GetTextDataType();

                    var getValueTypeStr = dict.GetLdictValue("GetValueType");
                    _getValueType = EnumHelper.GetByName<GetTreeItemValueOption>(getValueTypeStr, GetTreeItemValueOption.Id);

                    _imageUrls = dict.GetLdictValue("ImageUrls");
                    if (!_imageUrls.IsNullOrEmpty()) SetImageUrls();

                    ControlBaseHelper.SetControlForeColor(treeView, value);
                    ControlBaseHelper.SetControlFont(treeView, value);
                }

            }
        }

        private void TreeViewEx_Load(object sender, EventArgs e)
        {
            RefreshDataSource();

        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Bounds.Contains(e.Location))
            {
                if (LeafClick != null)
                {
                    var id = e.Node.Name.ToString();
                    var item = _dataSourceObject.Find(x => x.Id == id);
                    _openId = item.Id;
                    Text = item.DisplayName;
                    LeafClick(this, null);
                }
            }
            else
            {
            }
        }

        public void SetImageUrls()
        {
            if (!_imageUrls.IsNullOrEmpty())
            {
                var urlArry = _imageUrls.GetLarrayArray(true, true);
                foreach (var url in urlArry)
                {
                    var image = ControlBaseHelper.GetImage(url);
                    if (image != null)
                        _imageList.Images.Add(image);
                }
                treeView.ImageList = _imageList;

            }
        }

        public void SetTextByCulture()
        {
            RefreshDataSource();
            if (!_openId.IsNullOrEmpty())
            {
                var node = treeView.Nodes.Find(_openId, true).FirstOrDefault();
                if (node != null)
                    treeView.SelectedNode = node;
            }
        }


        public void RefreshDataSource()
        {
            treeView.Nodes.Clear();
            var topItems = new List<TreeItem>();
            if (_dataSourceObject.Count == 0) return;

            topItems = _dataSourceObject.FindAll(x => x.ParentId == _topId);
            foreach (var item in topItems)
            {
                var displayName = CommonHelper.GetDisplayName(_annexList.Count > 0, "", item.Name, _annexList, item.DisplayName);
                if (_imageList.Images.Count != 0) treeView.Nodes.Add(item.Id, displayName, item.ImageIndex, item.SelectedImageIndex);
                else treeView.Nodes.Add(item.Id, displayName);
                AddNodes(item);
            }

            if (!_openId.IsNullOrEmpty())
            {
                var node = treeView.Nodes.Find(_openId, true).FirstOrDefault();
                if (node != null)
                    treeView.SelectedNode = node;
            }
        }

        private void AddNodes(TreeItem treeItem)
        {
            TreeNode nd = treeView.Nodes.Find(treeItem.Id, true).FirstOrDefault();
            foreach (var item in _dataSourceObject.Where(x => x.ParentId == treeItem.Id))
            {
                var displayName = CommonHelper.GetDisplayName(_annexList.Count > 0, "", item.Name, _annexList, item.DisplayName);
                if (_imageList.Images.Count != 0)
                    nd.Nodes.Add(item.Id, displayName, item.ImageIndex, item.SelectedImageIndex);
                else nd.Nodes.Add(item.Id, displayName);
                AddNodes(item);
            }
        }


    }
}
