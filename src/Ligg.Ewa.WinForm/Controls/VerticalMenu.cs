using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.Parser.Helpers;



namespace Ligg.EasyWinApp.WinForm.Controls
{
    public partial class VerticalMenu : UserControl
    {
        public VerticalMenu()
        {
            InitializeComponent();
        }

        //#event
        public event EventHandler OnLastLevelNodeClick;
        public event EventHandler OnNotLastLevelNodeClick;

        private string _topId = "0";
        public string TopId
        {
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    _topId = value;
                }
            }
        }

        private string OpenId
        {
            get;
            set;
        }

        public string Value
        {
            get;
            private set;
        }

        public new string Text
        {
            get;
            private set;
        }

        public bool SetNameAsValue
        {
            get;
            set;
        }
        public string ImageUrl
        {
            set
            {
                try
                {
                    if (!value.IsNullOrEmpty())
                    {
                        var paramArry = value.GetParamArray(true, false);
                        var dir = paramArry[0];
                        var fileArry = paramArry[1].GetSubParamArray(true, true);

                        foreach (var file in fileArry)
                        {
                            var path = dir + "\\" + file;
                            _imageList.Images.Add(Image.FromFile(path));
                        }

                        for (var i = 0; i < fileArry.Length; i++)
                        {
                            _imageList.Images.SetKeyName(i, fileArry[i]);
                        }
                        treeView.ImageList = _imageList;

                        //** noted by liggin2019 at 180404
                        //test11: if do TreeNode - all.Add(string key, string text, 1,2), all nodes  have ImageIndex = 0 & selectedImageIndex = 2 --img count=3 //weird
                        //test12: if do TreeNode - all.Add(string key, string text, 0,1), all nodes  have ImageIndex = 0 & selectedImageIndex = 1--img count=2 //ok
                        //test13: if do TreeNode - all.Add(string key, string text, 1), all nodes  have ImageIndex = 0 --img count=2 //weird
                        //test14: if do TreeNode - all.Add(string key, string text, 2), all nodes  have ImageIndex = 0 --img count=3 //weird
                        //test2: if do TreeNode1.Add(string key, string text) &TreeNode2.Add(string key, string text, 1,2) , the node1 has ImageIndex=0;  the node2 has ImageIndex=1&selectedImageIndex=2; --img count=3
                        //test3: if do TreeNode1.Add(string key, string text) &TreeNode2.Add(string key, string text, 1) , the node1 has ImageIndex=0;  the node2 has ImageIndex=1&selectedImageIndex=0; --img count=2
                        //test4: if do TreeNode1.Add(string key, string text, 0) &TreeNode2.Add(string key, string text, 1,2) , the node1 has ImageIndex=0;  the node2 has ImageIndex=1&selectedImageIndex=2; --img count=3

                        // Set the whole TreeView control's default image and selected image indexes.
                        //treeView.ImageIndex = 0;
                        //treeView.SelectedImageIndex = 1;

                        //TreeNode Add(string key, string text, string imageKey, string selectedImageKey);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + "." + "ImageUrl.Set Error: " + ex.Message);
                }


            }
        }
        public List<Annex> AnnexList = new List<Annex>();
        private ImageList _imageList = new ImageList();
        private List<TreeItem> _dataList = new List<TreeItem>();
        public List<TreeItem> DataSource
        {
            set
            {
                try
                {
                    if (value.Count != 0)
                    {
                        _dataList = value;
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + "." + "DataSource.Set Error: " + ex.Message);
                }
            }
        }

        public void SetTextByCulture()
        {
            try
            {
                InitComponent();
                if (!OpenId.IsNullOrEmpty())
                {
                    var node = treeView.Nodes.Find(OpenId, true).FirstOrDefault();
                    if (node != null)
                        treeView.SelectedNode = node;
                }
            }

            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "SetTextByCulture Error: " + ex.Message);
            }
        }

        private void TreeViewEx_Load(object sender, EventArgs e)
        {
            try
            {
                InitComponent();
                if (!OpenId.IsNullOrEmpty())
                {
                    var node = treeView.Nodes.Find(OpenId, true).FirstOrDefault();
                    if (node != null)
                        treeView.SelectedNode = node;
                }
            }

            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "AddNodes Error: " + ex.Message);
            }
        }


        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node.Bounds.Contains(e.Location))
                {
                    var previousSelectedNode = treeView.SelectedNode;
                    var id = e.Node.Name.ToString();
                    var item = _dataList.Find(x => x.Id == id);

                    if (SetNameAsValue)
                    {
                        Value = item.Name;
                        OpenId = item.Id;
                    }
                    else
                    {
                        Value = id;
                    }
                    Text = item.DisplayName;
                    if (OnLastLevelNodeClick != null)
                    {
                        OnLastLevelNodeClick(this, null);
                    }
                }
                else
                {
                    if (OnNotLastLevelNodeClick != null)
                    {
                        OnNotLastLevelNodeClick(this, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "treeView_NodeMouseClick Error: " + ex.Message);
            }
        }
        private void InitComponent()
        {
            try
            {
                treeView.Nodes.Clear();
                var topItems = new List<TreeItem>();
                topItems = _dataList.FindAll(x => x.ParentId == _topId);

                foreach (var item in topItems)
                {
                    var displayName = FunctionHelper.GetDisplayName(AnnexList.Count > 0, "", item.Name, AnnexList, item.DisplayName);
                    treeView.Nodes.Add(item.Id, displayName, item.ImageKey, item.SelectedImageKey);
                    AddNodes(item);
                }
            }

            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "RefreshDataSource Error: " + ex.Message);
            }
        }

        private void AddNodes(TreeItem treeItem)
        {
            try
            {
                TreeNode nd = treeView.Nodes.Find(treeItem.Id, true).FirstOrDefault();
                foreach (var item in _dataList.Where(x => x.ParentId == treeItem.Id))
                {
                    var displayName = FunctionHelper.GetDisplayName(AnnexList.Count > 0, "", item.Name, AnnexList, item.DisplayName);
                    if (!item.ImageKey.IsNullOrEmpty() & !item.SelectedImageKey.IsNullOrEmpty()) nd.Nodes.Add(item.Id, displayName, item.ImageKey, item.SelectedImageKey);
                    else if (!item.ImageKey.IsNullOrEmpty()) nd.Nodes.Add(item.Id, displayName, item.ImageKey);
                    else if (!item.SelectedImageKey.IsNullOrEmpty()) nd.Nodes.Add(item.Id, displayName, "", item.SelectedImageKey);
                    else nd.Nodes.Add(item.Id, displayName);
                    AddNodes(item);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "AddNodes Error: " + ex.Message);
            }
        }

    }
}
