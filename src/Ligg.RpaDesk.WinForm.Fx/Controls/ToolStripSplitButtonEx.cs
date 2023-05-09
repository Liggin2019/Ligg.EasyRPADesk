using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.WinFormBase;
using  Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Parser.DataModels;

namespace Ligg.RpaDesk.WinForm.Controls
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
        private List<SubMenuItem> _dataSource = new List<SubMenuItem>();

        public ToolStripSplitButtonEx(List<SubMenuItem> dataSource, List<Annex> annexList)
        {
            if (annexList != null) _annexList = annexList;
            if (dataSource != null) _dataSource = dataSource;
            CurrentTransaction = new Transaction();
            RefreshDataSource();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            var menuItemName = (sender as ToolStripMenuItem).Name;
            this.Tag = menuItemName;
            var menuItem = _dataSource.Find(x => x.Name == menuItemName);
            //var args = new ContextMenuItemClickEventArgs(null, menuItemName, menuItem.Action);
            if (OnMenuItemClick != null)
            {
                CurrentTransaction.Action = menuItem.Action;
                CurrentTransaction.DisplayName = CommonHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
                CurrentTransaction.UiItemName = menuItemName;
                CurrentTransaction.ShowRunningStatus = menuItem.ShowRunningStatus;
                //CurrentTransaction.ExecMode= menuItem.ExecMode;
                //CurrentTransaction.WriteIntoLog = menuItem.WriteIntoLog;
                OnMenuItemClick(this, null);
            }
        }

        public void SetTextByCulture()
        {
            foreach (var menuItem in _dataSource)
            {
                var menuItemControl = this.DropDownItems.Find(menuItem.Name, true);
                menuItemControl[0].Text = CommonHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
            }
        }

        private void RefreshDataSource()
        {
            if (_dataSource.Count == 0) return;

            DropDownItems.Clear();
            var topItems = new List<SubMenuItem>();
            topItems = _dataSource.FindAll(x => x.ParentId == _topId);

            foreach (var menuItem in _dataSource.FindAll(x => x.ParentId == _topId))
            {
                AddNodes(null, menuItem);
            }
        }

        private void AddNodes(ToolStripMenuItem toolStripMenuItem, SubMenuItem menuItem)
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
                var displayName = CommonHelper.GetDisplayName(_annexList.Count > 0, "", menuItem.Name, _annexList, menuItem.DisplayName);
                menuItemControl.Text = displayName;
                menuItemControl.TextAlign = ContentAlignment.TopCenter;
                var img1 = ControlBaseHelper.GetImage(menuItem.ImageUrl);
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
                var subMenuItems = _dataSource.FindAll(x => x.ParentId == menuItem.Id);
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

    }
}
