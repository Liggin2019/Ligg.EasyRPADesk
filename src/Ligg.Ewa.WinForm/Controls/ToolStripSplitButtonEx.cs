using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.Helpers;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ligg.EasyWinApp.Parser.Helpers;
using Ligg.EasyWinApp.Parser.DataModel;

namespace Ligg.EasyWinApp.WinForm.Controls
{
    public sealed class ToolStripSplitButtonEx : ToolStripSplitButton
    {
        //#event
        public event EventHandler OnMenuItemClick;
        public Transaction CurrentTransaction
        {
            get;
            set;
        }

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


        private List<Annex> _annexList = new List<Annex>();
        private List<SubMenuItem> _dataList = new List<SubMenuItem>();
        public ToolStripSplitButtonEx(List<SubMenuItem> dataSource, List<Annex> annexList)
        {
            try
            {
                if (annexList != null) _annexList = annexList;
                if (dataSource != null) _dataList = dataSource;
                CurrentTransaction = new Transaction();
                InitComponent();
                //this.on
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "ToolStripSplitButtonEx Error: " + ex.Message);
            }
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            var menuItemName = (sender as ToolStripMenuItem).Name;
            this.Tag = menuItemName;
            var menuItem = _dataList.Find(x => x.Name == menuItemName);
            //var args = new ContextMenuItemClickEventArgs(null, menuItemName, menuItem.Action);
            if (OnMenuItemClick != null)
            {
                CurrentTransaction.Action = menuItem.Action;
                CurrentTransaction.DisplayName = FunctionHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
                CurrentTransaction.ControlName = menuItemName;
                CurrentTransaction.ExecModeFlag = menuItem.ExecModeFlag;
                CurrentTransaction.ShowRunningStatusFlag = menuItem.ShowRunningStatusFlag;
                CurrentTransaction.WriteIntoLogFlag = menuItem.WriteIntoLogFlag;
                OnMenuItemClick(this, null);
            }
        }

        //#proc
        public void SetTextByCulture()
        {
            try
            {
                foreach (var menuItem in _dataList)
                {
                    var menuItemControl = this.DropDownItems.Find(menuItem.Name, true);
                    menuItemControl[0].Text = FunctionHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "SetTextByCulture Error: " + ex.Message);
            }
        }

        private void InitComponent()
        {
            try
            {
                DropDownItems.Clear();
                var topItems = new List<SubMenuItem>();
                topItems = _dataList.FindAll(x => x.ParentId == _topId);

                foreach (var menuItem in _dataList.FindAll(x => x.ParentId == _topId))
                {
                    AddNodes(null, menuItem);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "RefreshDataSource Error: " + ex.Message);
            }
        }

        private void AddNodes(ToolStripMenuItem toolStripMenuItem, SubMenuItem menuItem)
        {
            try
            {

                if (menuItem.ControlTypeName == null)
                    menuItem.ControlTypeName = "";

                if (menuItem.ControlTypeName.ToLower().Contains("Separator".ToLower()))
                {
                    var menuItemControl = new System.Windows.Forms.ToolStripSeparator();
                    menuItemControl.Name = menuItem.Name;
                    menuItemControl.Visible = !menuItem.Invisible;
                    menuItemControl.Enabled = !menuItem.Disabled;
                    if (!menuItem.Id.IsNullOrEmpty())
                    {
                        toolStripMenuItem.DropDownItems.Add(menuItemControl);
                    }
                    else
                    {
                        DropDownItems.Add(menuItemControl);
                    }
                }
                else
                {
                    var menuItemControl = new ToolStripMenuItem();
                    menuItemControl.Name = menuItem.Name;
                    var displayName = FunctionHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
                    menuItemControl.Text = displayName;
                    menuItemControl.TextAlign = ContentAlignment.TopCenter;
                    var img1 = ControlHelper.GetImage(menuItem.ImageUrl);
                    if (img1 != null) menuItemControl.Image = img1;
                    menuItemControl.Visible = !menuItem.Invisible;
                    menuItemControl.Enabled = !menuItem.Disabled;
                    //menuItemControl.AutoSize = false;
                    menuItemControl.ImageScaling = ToolStripItemImageScaling.None;

                    if (menuItem.ParentId == _topId)
                    {
                        this.DropDownItems.Add(menuItemControl);
                    }
                    else
                    {
                        toolStripMenuItem.DropDownItems.Add(menuItemControl);
                    }
                    var item = menuItem;
                    var subMenuItems = _dataList.FindAll(x => x.ParentId == menuItem.Id);
                    if (subMenuItems.Count > 0)
                    {
                        foreach (var menuItem1 in subMenuItems)
                        {
                            AddNodes(menuItemControl, menuItem1);
                        }

                    }
                    else
                    {
                        menuItemControl.Click += new System.EventHandler(menuItem1_Click);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + "." + "AddNodes Error: " + ex.Message);
            }
        }

    }

}
