using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ligg.Base.DataModel;
using Ligg.Base.DataModel.Enums;
using Ligg.Base.Extension;
using Ligg.Base.Handlers;
using Ligg.Base.Helpers;
using Ligg.WinForm.DataModel;
using Ligg.WinForm.DataModel.Enums;
using Ligg.WinForm.Helpers;

namespace Ligg.WinForm.Controls
{
    public partial class SortedListView : UserControl
    {
        public SortedListView()
        {
            InitializeComponent();
        }

        //#constructor
        public SortedListView(string headerCfgXmPath, string contentMenuCfgXmPath)
        {
            try
            {
                InitComponent(headerCfgXmPath, contentMenuCfgXmPath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "ContextMenuStripEx Error: " + ex.Message);
            }
        }

        //#event
        public event EventHandler OnRightClickToSetContentMenu;
        public event EventHandler OnDoubleClick;
        public event EventHandler OnColumnHeaderClick;
        public event EventHandler OnContextMenuItemClick;

        //#property-private 
        private bool _isDuringInitHeader = false;
        private bool _isDuringClickColumn = false;
        private bool _isDuringChangeTextByCulture = false;
        private int _currentSortColumnIndex = -1;

        //#property-public 
        private List<ListViewHeader> _headers = new List<ListViewHeader>();
        private List<Annex> _headersAnnexes = null;

        private ContextMenuStripEx _contextMenuStripSelectNone;
        private ContextMenuStripEx _contextMenuStripSelectOne;
        private ContextMenuStripEx _contextMenuStripSelectMany;
        private List<ContextMenuItem> _contentMenuItems = new List<ContextMenuItem>();
        private List<Annex> _contentMenuItemsAnnexes = null;


        private string _orderFieldName;
        public string OrderFieldName
        {
            get { return _orderFieldName; }
            set { _orderFieldName = value; }
        }

        private bool _isOrderDescending = false;
        public bool IsOrderDescending
        {
            get { return _isOrderDescending; }
            set { _isOrderDescending = value; }
        }

        private bool _hasPager = false;
        public bool HasPager
        {
            get { return _hasPager; }
            set { _hasPager = value; }
        }

        public bool CanOrder
        {
            get;
            set;
        }

        public int RowHeight
        {
            set
            {
                //C#中ListView控件Detail显示，是没有行高这个属性的，但可以通过设置imagelist“撑高”行距。
                smallImageList.ImageSize = new Size(1, value);
            }
        }

        public int RightMargin { private get; set; }

        public ListView.SelectedListViewItemCollection SelectedItems
        {
            get { return listView.SelectedItems; }
        }

        private String _viewTypeName;
        public String ViewTypeName
        {
            set
            {
                _viewTypeName = value;
                try
                {
                    var viewTypeInt = EnumHelper.GetIdByName<View>(_viewTypeName);
                    if (string.IsNullOrEmpty(_viewTypeName))
                    {
                        listView.View = View.Details;
                    }
                    else
                    {
                        listView.View = (View)Enum.ToObject(typeof(View), viewTypeInt);
                    }
                }
                catch (Exception)
                {
                    listView.View = View.Details;
                }
            }
        }

        //#method
        private void ListViewEx_Load(object sender, EventArgs e)
        {
            //listView.CheckBoxes = true;
            listView.FullRowSelect = true;
            listView.GridLines = false;
            listView.SmallImageList = smallImageList;
            listView.LargeImageList = largeImageList;
        }

        private void listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (_isDuringInitHeader & _isDuringClickColumn & _isDuringChangeTextByCulture)
            {
                ColumnHeader header = this.listView.Columns[e.ColumnIndex];
                foreach (var headerCfg in _headers)
                {
                    if (header.Name == headerCfg.Name && headerCfg.Width > -1)
                    {
                        header.Width = headerCfg.Width;
                    }
                }
            }
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!CanOrder) return;
            var headerName = this.listView.Columns[e.Column].Name;
            var header = _headers.Find(x => x.Name == headerName);
            if (!header.CanOrder) return;

            _isDuringClickColumn = true;
            string asc = ((char)0x25bc).ToString().PadLeft(2, ' ');
            string des = ((char)0x25b2).ToString().PadLeft(2, ' ');

            _orderFieldName = this.listView.Columns[e.Column].Name;
            if (_isOrderDescending == false)
            {
                _isOrderDescending = true;
                string oldStr = this.listView.Columns[e.Column].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                this.listView.Columns[e.Column].Text = oldStr + asc;
            }
            else if (_isOrderDescending == true)
            {
                _isOrderDescending = false;
                string oldStr = this.listView.Columns[e.Column].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                this.listView.Columns[e.Column].Text = oldStr + des;
            }

            if (!_hasPager)
            {
                var sortType = ListViewSortType.String;
                try
                {
                    sortType = (ListViewSortType)Enum.ToObject(typeof(ListViewSortType), header.OrderType);
                }
                catch (Exception)
                {
                    sortType = ListViewSortType.String;
                }
                listView.ListViewItemSorter = new ListViewItemSorter(e.Column, sortType, _isOrderDescending ? SortOrder.Descending : SortOrder.Ascending);
                this.listView.Sort();
            }
            else
            {
                if (OnColumnHeaderClick != null)
                {
                    var args = new ListViewColumnClickEventArgs(listView.Columns[e.Column].Name, _isOrderDescending);
                    OnColumnHeaderClick(this, args);
                }
            }
            int rowCount = this.listView.Items.Count;
            if (_currentSortColumnIndex != -1)
            {
                if (e.Column != _currentSortColumnIndex)
                {
                    this.listView.Columns[_currentSortColumnIndex].Text = this.listView.Columns[_currentSortColumnIndex].Text.TrimEnd((char)0x25bc, (char)0x25b2, ' ');
                }
            }
            _currentSortColumnIndex = e.Column;
            _isDuringClickColumn = false;
        }

        private void listView_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem xy = listView.GetItemAt(e.X, e.Y);
            if (xy == null)
            {
                listView.SelectedItems.Clear();
            }

            if (e.Button == MouseButtons.Right)
            {
                //listView.SelectedItems.Clear();
                Point point = this.PointToClient(listView.PointToScreen(new Point(e.X, e.Y)));
                if (xy == null)
                {
                    if (_contextMenuStripSelectNone != null)
                    {
                        if (OnRightClickToSetContentMenu != null)
                        {
                            OnRightClickToSetContentMenu(_contextMenuStripSelectNone, null);
                        }
                        _contextMenuStripSelectNone.Show(this, point);
                    }
                }
                if (xy != null)
                {
                    if (listView.SelectedIndices.Count == 0)
                    {
                        //no this case
                    }
                    else if (listView.SelectedIndices.Count == 1)
                    {
                        if (_contextMenuStripSelectOne != null)
                        {
                            if (OnRightClickToSetContentMenu != null)
                            {
                                OnRightClickToSetContentMenu(_contextMenuStripSelectOne, null);
                            }
                            _contextMenuStripSelectOne.Show(this, point);
                        }
                    }
                    else
                    {
                        if (_contextMenuStripSelectMany != null)
                        {
                            if (OnRightClickToSetContentMenu != null)
                            {
                                OnRightClickToSetContentMenu(_contextMenuStripSelectMany, null);
                            }
                            _contextMenuStripSelectMany.Show(this, point);
                        }
                    }
                }
            }
        }


        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnDoubleClick != null)
            {
                OnDoubleClick(this, null);
            }
        }

        private void contextMenuItem_Click(object sender, EventArgs e)
        {
            var contextMenuStripEx = sender as ContextMenuStripEx;
            var args = (ContextMenuItemClickEventArgs)e;
            var args1 = new ContextMenuItemClickEventArgs(contextMenuStripEx, args.ItemName, args.Action);
            if (OnContextMenuItemClick != null)
            {
                OnContextMenuItemClick(this, args1);
            }
        }

        //#proc
        public void InitComponent(string headerCfgXmPath, string contentMenuCfgXmPath)
        {
            try
            {
                var xmlMgr = new XmlHandler(headerCfgXmPath);
                _headers = xmlMgr.ConvertToObject<List<ListViewHeader>>();
                var dir = FileHelper.GetFileDetailByOption(headerCfgXmPath, FilePathComposition.Directory);
                var fileTitle = FileHelper.GetFileDetailByOption(headerCfgXmPath, FilePathComposition.FileTitle);
                var headerAnnexesCfgXmlPath = dir + "\\" + fileTitle + "Annexes.xml";
                if (File.Exists(headerAnnexesCfgXmlPath))
                {
                    var xmlMgr1 = new XmlHandler(headerAnnexesCfgXmlPath);
                    _headersAnnexes = xmlMgr1.ConvertToObject<List<Annex>>();
                }
                InitHeader();

                if (File.Exists(contentMenuCfgXmPath))
                {
                    xmlMgr = new XmlHandler(contentMenuCfgXmPath);
                    _contentMenuItems = xmlMgr.ConvertToObject<List<ContextMenuItem>>();
                    foreach (var contentMenuItem in _contentMenuItems)
                    {
                        contentMenuItem.Type = EnumHelper.GetIdByName<ListViewContextMenuItemType>(contentMenuItem.TypeName);
                    }
                    dir = FileHelper.GetFileDetailByOption(contentMenuCfgXmPath, FilePathComposition.Directory);
                    fileTitle = FileHelper.GetFileDetailByOption(contentMenuCfgXmPath, FilePathComposition.FileTitle);
                    var cttMenuItemsAnnexesCfgXmlPath = dir + "\\" + fileTitle + "Annexes.xml";
                    if (File.Exists(cttMenuItemsAnnexesCfgXmlPath))
                    {
                        var xmlMgr1 = new XmlHandler(cttMenuItemsAnnexesCfgXmlPath);
                        _contentMenuItemsAnnexes = xmlMgr1.ConvertToObject<List<Annex>>();
                    }
                    InitContentMenus();
                }

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "Init Error: " + ex.Message);
            }
        }


        public void SetTextByCulture()
        {
            try
            {
                string shortLangName = CultureHelper.CurrentLanguageCode;
                _isDuringChangeTextByCulture = true;
                for (int i = 0; i < listView.Columns.Count; i++)
                {
                    var header = _headers.Find(x => x.Name == listView.Columns[i].Name);
                    listView.Columns[i].Text = LayoutHelper.GetControlDisplayName(true, "", header.Name, _headersAnnexes, "");
                }
                _isDuringChangeTextByCulture = false;

                _contextMenuStripSelectNone.SetTextByCulture();
                _contextMenuStripSelectOne.SetTextByCulture();
                _contextMenuStripSelectMany.SetTextByCulture();

            }
            catch (Exception ex)
            {
                _isDuringChangeTextByCulture = false;
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "SetTextByCulture Error: " + ex.Message);
            }
        }

        private void InitHeader()
        {
            try
            {
                if (_headers.Count > 0)
                {
                    _isDuringInitHeader = true;
                    CheckHeaderConfigData(_headers);

                    var totalWidthDefined = 0;
                    foreach (var headerCfg in _headers)
                    {
                        if (headerCfg.Width > -1)
                        {
                            totalWidthDefined = totalWidthDefined + headerCfg.Width;
                        }
                    }

                    foreach (var headerCfg in _headers)
                    {
                        var header = new ColumnHeader();
                        header.Name = headerCfg.Name;
                        header.Text = LayoutHelper.GetControlDisplayName(true, "", headerCfg.Name, _headersAnnexes, "");
                        header.TextAlign = headerCfg.TextAlignType == 1 ? HorizontalAlignment.Left :
                                               (headerCfg.TextAlignType == 2 ? HorizontalAlignment.Center : HorizontalAlignment.Right);
                        if (headerCfg.Width > -1)
                        {
                            header.Width = headerCfg.Width;
                        }
                        else
                        {
                            if (totalWidthDefined + RightMargin < listView.Width)
                            {
                                header.Width = listView.Width - totalWidthDefined - RightMargin;
                            }
                        }
                        listView.Columns.Add(header);
                    }
                    _isDuringInitHeader = false;
                }

            }
            catch (Exception ex)
            {
                _isDuringInitHeader = false;
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "InitHeader Error: " + ex.Message);
            }
        }

        private void CheckHeaderConfigData(List<ListViewHeader> headers)
        {
            if (headers.FindAll(x => x.Width == -1).Count > 1)
            {
                var header = headers.Find(x => x.Width == -1);
                throw new ArgumentException("listView autosized header qty can't be greater than 1! , Header Name=" + header.Name);
            }

            foreach (var header in headers)
            {
                if (headers.FindAll(x => x.Name == header.Name).Count > 1)
                {
                    throw new ArgumentException("listView header can't have duplicated name! , Header Name=" + header.Name);
                }
            }
        }

        private void InitContentMenus()
        {
            try
            {
                var itemList = _contentMenuItems.FindAll(x => x.Type == (int)ListViewContextMenuItemType.SelectNone
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOrOne | x.Type == (int)ListViewContextMenuItemType.SelectNoneOrMany
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOneOrMany);
                _contextMenuStripSelectNone = new ContextMenuStripEx(true, itemList, _contentMenuItemsAnnexes);
                _contextMenuStripSelectNone.Name = "contextMenuStripSelectNone";
                _contextMenuStripSelectNone.OnItemClick += new System.EventHandler(contextMenuItem_Click);

                itemList = _contentMenuItems.FindAll(x => x.Type == (int)ListViewContextMenuItemType.SelectOne
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOrOne | x.Type == (int)ListViewContextMenuItemType.SelectOneOrMany
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOneOrMany);
                _contextMenuStripSelectOne = new ContextMenuStripEx(true, itemList, _contentMenuItemsAnnexes);
                _contextMenuStripSelectOne.Name = "contextMenuStripSelectOne";
                _contextMenuStripSelectOne.OnItemClick += new System.EventHandler(contextMenuItem_Click);

                itemList = _contentMenuItems.FindAll(x => x.Type == (int)ListViewContextMenuItemType.SelectMany
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOrMany | x.Type == (int)ListViewContextMenuItemType.SelectOneOrMany
                    | x.Type == (int)ListViewContextMenuItemType.SelectNoneOneOrMany);
                _contextMenuStripSelectMany = new ContextMenuStripEx(true, itemList, _contentMenuItemsAnnexes);
                _contextMenuStripSelectMany.Name = "contextMenuStripSelectMany";
                _contextMenuStripSelectMany.OnItemClick += new System.EventHandler(contextMenuItem_Click);

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "InitContentMenus Error: " + ex.Message);
            }
        }

        public void Render(List<ListViewItem> listViewItems)
        {
            try
            {
                listView.Items.Clear();
                foreach (var listViewItem in listViewItems)
                {
                    listView.Items.Add(listViewItem);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "Render Error: " + ex.Message);
            }
        }


        public void Render(List<IdValueText> idValTextList)
        {
            try
            {
                listView.Items.Clear();
                var ids = idValTextList.Select(x => x.Id).Distinct();
                foreach (var id in ids)
                {
                    //if (!smalImageList.Images.ContainsKey(".jpg"))
                    //{
                    //    smalImageList.Images.Add(".jpg", FileHelper.GetFileIcon("e:\\temp\\runningParams1.txt", true));
                    //}
                    var idValTextListWtSameId = idValTextList.FindAll(x => x.Id == id);
                    var listViewItemArray = new string[_headers.Count];
                    int i = 0;
                    foreach (var header in _headers)
                    {
                        var idValueText = idValTextListWtSameId.Find(x => x.Value == header.Name);
                        if (idValueText != null)
                        {
                            listViewItemArray[i] = idValueText.Text;
                            if (!header.StringFormat.IsNullOrEmpty())
                            {
                                if (header.StringFormat.StartsWith("Ctime"))
                                {
                                    var fmt = header.StringFormat.Split('^')[1].Trim();
                                    {
                                        if (!idValueText.Text.IsNullOrEmpty())
                                        {
                                            listViewItemArray[i] = Convert.ToDateTime(idValueText.Text).ToString(fmt, DateTimeFormatInfo.InvariantInfo);
                                        }
                                        else
                                        {
                                            listViewItemArray[i] = "";
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            listViewItemArray[i] = "";
                        }

                        i++;
                    }
                    var listViewItem = new ListViewItem(listViewItemArray, "");
                    listView.Items.Add(listViewItem);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "Render Error: " + ex.Message);
            }
        }

        public void Render(string listXmlPath, string nodeName)
        {
            try
            {
                var xmlMgr = new XmlHandler(listXmlPath);
                var idValTextList = xmlMgr.GetChildNodeInnerTextsToIdValueTextList(nodeName);
                Render(idValTextList);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "Render Error: " + ex.Message);
            }
        }

        public void AddItem(List<ListViewItem> listViewItems)
        {
            foreach (var listViewItem in listViewItems)
            {
                listView.Items.Add(listViewItem);
            }
        }

        public List<string> GetSelectedItemsTextsByHeader(string headerName)
        {
            var strArry = new List<string>();
            var header = listView.Columns[headerName];
            if (header != null)
            {
                var index = header.Index;
                for (int i = 0; i < listView.SelectedItems.Count; i++)
                {
                    strArry.Add(listView.SelectedItems[i].SubItems[index].Text);
                }
            }
            return strArry;
        }



    }


    //#definition
    public class ListViewHeader
    {
        public string Name;
        public string DisplayName;
        public int Width;
        public bool CanOrder;
        public int OrderType;
        public string StringFormat;
        public int TextAlignType;
    }


    public class ListViewItemSorter : IComparer
    {
        private int _sortColumn;
        private ListViewSortType _sortType;
        private SortOrder _sortOrder;

        public ListViewItemSorter(int sortColumn, ListViewSortType sortType, SortOrder sortOrder)
        {
            _sortColumn = sortColumn;
            _sortType = sortType;
            _sortOrder = sortOrder;
        }

        public int Compare(object x, object y)
        {
            var listViewItemX = (ListViewItem)x;
            var listViewItemY = (ListViewItem)y;
            string strX = listViewItemX.SubItems[_sortColumn].Text;
            string strY = listViewItemY.SubItems[_sortColumn].Text;
            // Nulls first (null means less, since it's blank)
            if (strX == null)
            {
                if (strY == null) return 0;
                return -1;
            }
            if (strY == null) return 1;
            var compareResult = 0;

            if (_sortType == ListViewSortType.FileSize)
            {
                // Convert the non-KB part to a number
                double numX = 0;
                double numY = 0;
                if (strX.EndsWith("KB") || strX.EndsWith("GB") || strX.EndsWith("MB"))
                    double.TryParse(strX.Substring(0, strX.Length - 3), out numX);
                if (strX.EndsWith("Bytes"))
                    double.TryParse(strX.Substring(0, strX.Length - 6), out numX);
                if (strY.EndsWith("KB") || strY.EndsWith("GB") || strY.EndsWith("MB"))
                    double.TryParse(strY.Substring(0, strY.Length - 3), out numY);
                if (strY.EndsWith("Bytes"))
                    double.TryParse(strX.Substring(0, strY.Length - 6), out numY);
                long bytesX;
                long bytesY;
                if (strX.EndsWith("KB"))
                    bytesX = (long)numX * 1024;
                else if (strX.EndsWith("MB"))
                    bytesX = (long)numX * 1048576;
                else if (strX.EndsWith("GB"))
                    bytesX = (long)numX * 1073741824;
                else
                    bytesX = (long)numX;

                if (strY.EndsWith("KB"))
                    bytesY = (long)numY * 1024;
                else if (strY.EndsWith("MB"))
                    bytesY = (long)numY * 1048576;
                else if (strY.EndsWith("GB"))
                    bytesY = (long)numY * 1073741824;
                else
                    bytesY = (long)numY;
                compareResult = bytesX.CompareTo(bytesY);
            }
            else if (_sortType == ListViewSortType.Numeral)
            {
                double numX = 0;
                double.TryParse(strX, out numX);
                double numY = 0;
                double.TryParse(strY, out numY);
                compareResult = numX.CompareTo(numY);
            }
            else
            {
                compareResult = String.Compare(strX, strY);
            }

            if (_sortOrder == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            if (_sortOrder == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            // Return '0' to indicate they are equal
            return 0;
        }
    }

    public class ListViewColumnClickEventArgs : EventArgs
    {
        public ListViewColumnClickEventArgs(string columnName, bool isDescending)
        {
            ColumnName = columnName;
            IsDescending = isDescending;
        }

        public string ListViewExName { get; set; }
        public string ColumnName { get; set; }
        public bool IsDescending { get; set; }
    }

    public enum ListViewContextMenuItemType
    {
        SelectNone = 10,
        SelectOne = 11,
        SelectMany = 12,
        SelectNoneOrOne = 21,
        SelectNoneOrMany = 22,
        SelectOneOrMany = 23,
        SelectNoneOneOrMany = 33
    }

}
