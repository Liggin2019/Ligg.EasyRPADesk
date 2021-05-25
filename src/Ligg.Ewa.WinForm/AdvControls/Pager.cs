using System;
using System.Windows.Forms;
using Ligg.WinForm.Resources;

namespace Ligg.WinForm.Controls
{
    public partial class Pager : UserControl
    {
        public Pager()
        {
            InitializeComponent();
        }

        //#event
        public event EventHandler OnPageChanged;

        //#property
        private int _pageIndex = 1;
        public virtual int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        private int _pageSize = 30;
        public virtual int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
            }
        }

        public virtual int CurrentPageSize
        {
            get { return _pageSize; }
        }

        private int _recordCount = 0;
        public virtual int RecordCount
        {
            get { return _recordCount; }
            set { _recordCount = value; }
        }

        private int _pageCount = 0;
        public int PageCount
        {
            get
            {
                if (_pageSize != 0)
                {
                    _pageCount = GetPageCount();
                }
                return _pageCount;
            }
        }

        //#method
        private void Pager_Load(object sender, EventArgs e)
        {
            Height = 28;
            this.BackColor = StyleSheet.PagerBackColor;
            SetComponentByCulture();
        }

        private void linkLabelFirst_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PageIndex = 1;
            DrawControl(true);
        }

        private void linkLabelPrev_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PageIndex = Math.Min(PageCount, PageIndex - 1);
            DrawControl(true);
        }

        private void linkLabelNext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PageIndex = Math.Min(PageCount, PageIndex + 1);
            DrawControl(true);
        }

        private void linkLabelLast_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PageIndex = PageCount;
            DrawControl(true);
        }

        private void textBoxPageSize_TextChanged(object sender, EventArgs e)
        {
            int num1 = 0;
            if (int.TryParse(textBoxPageSize.Text.Trim(), out num1) && num1 > 0)
            {
                _pageSize = num1;
            }
            //DrawControl(true);
        }

        private void textBoxPageSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == 13)
            //{
            //    pictureBoxGo_Click(null, null);
            //}
        }

        //private void textBoxPageSize_MouseLeave(object sender, EventArgs e)
        //{
        //    DrawControl(true);
        //}

        private void textBoxGoToPageNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pictureBoxGo_Click(null, null);
            }
        }

        private void pictureBoxGo_Click(object sender, EventArgs e)
        {
            int num1 = 0;
            if (!(int.TryParse(textBoxPageSize.Text.Trim(), out num1) && num1 > 0))
            {
                textBoxPageSize.Text = PageSize.ToString();
            }

            int num2 = 0;
            if (int.TryParse(textBoxGoToPageNo.Text.Trim(), out num2) && num2 > 0) 
            {
                if (num2 < PageCount | num2 == PageCount)
                {
                    PageIndex = num2;
                    DrawControl(true);
                }
                else
                {
                    textBoxGoToPageNo.Text ="";
                }
            }
            else
            {
                textBoxGoToPageNo.Text ="";
            }

        }

        //#func
        public void SetComponentByCulture()
        {
            labelPage.Text = WinformRes.Page;
            labelTotalRecordCountText.Text = WinformRes.RecordCount;
            labelPageSize.Text = WinformRes.PageSize;
            linkLabelFirst.Text = WinformRes.FirstPage;
            linkLabelLast.Text = WinformRes.LastPage;
            linkLabelPrev.Text = WinformRes.PrevPage;
            linkLabelNext.Text = WinformRes.NextPage;
            labelGoToText.Text = WinformRes.GoToPage;
        }

        public void DrawControl(int count)
        {
            _recordCount = count;
            DrawControl(false);
        }

        private void DrawControl(bool callEvent)
        {
            labelCurrentPageNo.Text = PageIndex.ToString();
            labelTotalPageNo.Text = PageCount.ToString();
            labelTotalRecordCount.Text = RecordCount.ToString();
            textBoxPageSize.Text = _pageSize.ToString();

            if (callEvent && OnPageChanged != null)
            {
                OnPageChanged(this, null);//当前分页数字改变时，触发委托事件
            }
            SetFormCtrEnabled();
            if (PageCount == 1)//有且仅有一页
            {
                linkLabelFirst.Enabled = false;
                linkLabelPrev.Enabled = false;
                linkLabelNext.Enabled = false;
                linkLabelLast.Enabled = false;
                //pictureBoxGo.Enabled = false;
            }
            else if (PageIndex == 1)//第一页
            {
                linkLabelFirst.Enabled = false;
                linkLabelPrev.Enabled = false;
            }
            else if (PageIndex == PageCount)//最后一页
            {
                linkLabelNext.Enabled = false;
                linkLabelLast.Enabled = false;
            }
        }

        private void SetFormCtrEnabled()
        {
            linkLabelFirst.Enabled = true;
            linkLabelPrev.Enabled = true;
            linkLabelNext.Enabled = true;
            linkLabelLast.Enabled = true;
            pictureBoxGo.Enabled = true;
        }

        //#proc
        private int GetPageCount()
        {
            if (_pageSize == 0)
            {
                return 0;
            }
            int pageCount = RecordCount / _pageSize;
            if (RecordCount % _pageSize == 0)
            {
                pageCount = RecordCount / _pageSize;
            }
            else
            {
                pageCount = RecordCount / _pageSize + 1;
            }
            return pageCount;
        }



    }
}