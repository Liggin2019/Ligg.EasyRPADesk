using System;
using System.Drawing;
using System.Windows.Forms;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.RpaDesk.WinForm.Resources;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Forms
{
    public partial class FrameForm : GroundForm
    {
        private bool _isHorLeftNavDivisionCurrentlyShown;
        private bool _isHorLeftNavDivision1CurrentlyShown;
        private readonly ToolTip _horizontalResizeButtonToolTip = new ToolTip();
        private readonly ToolTip _horizontalResizeButton1ToolTip = new ToolTip();
        private int _mainSectionLeftNavDivisionWidth;
        private int _mainSectionLeftNavDivision1Width;

        protected int TopNavSectionHeight = 30;
        protected int TopNavSectionLeftRegionWidth = 10;
        protected int TopNavSectionRightRegionWidth = 10;


        protected int ToolBarSectionHeight = 64;
        protected int ToolBarSectionPublicRegionWidth = 105;
        protected int ToolBarSectionLeftRegionWidth = 10;
        protected int ToolBarSectionRightRegionWidth = 10;


        protected int MiddleNavSectionHeight = 28;
        protected int MiddleNavSectionLeftRegionWidth = 10;
        protected int MiddleNavSectionRightRegionWidth = 10;

        protected int DownNavSectionHeight = 28;
        protected int DownNavSectionLeftRegionWidth = 10;
        protected int DownNavSectionRightRegionWidth = 10;

        protected int MainSectionLeftNavDivisionWidth = 80;
        protected int MainSectionLeftNavDivisionUpRegionHeight = 24;
        protected int MainSectionLeftNavDivisionDownRegionHeight = 0;


        protected int MainSectionLeftNavDivision1Width = 80;
        protected int MainSectionLeftNavDivision1UpRegionHeight = 24;
        protected int MainSectionLeftNavDivision1DownRegionHeight = 0;

        protected int MainSectionHorizontalResizeDivisionWidth = 6;
        protected int HorizontalResizeButtonPosX = 0;
        protected int HorizontalResizeButtonPosY = 0;

        protected int MainSectionHorizontalResizeDivision1Width = 6;
        protected int HorizontalResizeButton1PosX = 0;
        protected int HorizontalResizeButton1PosY = 0;

        protected int MainSectionMainDivisionUpRegionHeight = 0;
        protected int MainSectionMainDivisionDownRegionHeight = 0;

        protected int MainSectionRightDivisionWidth = 80;
        protected int MainSectionRightDivisionUpRegionHeight = 0;
        protected int MainSectionRightDivisionDownRegionHeight = 0;

        protected ResizableDivisionStatus HorizontalResizableDivisionStatus = ResizableDivisionStatus.None;
        protected ResizableDivisionStatus HorizontalResizableDivision1Status = ResizableDivisionStatus.None;

        protected FrameForm()
        {
            InitializeComponent();
            InitFrameComponent();
        }

        private void HorizontalResizeButton_Click(object sender, EventArgs e)
        {
            if (_isHorLeftNavDivisionCurrentlyShown)
            {
                MainSectionLeftNavDivision.Width = 0;
                LeftHorizontalResizeButton.BackgroundImage = PictureList.Images["go_right.png"];
                _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.ShowLeftDivision);
                MainSectionSplitter.Visible = false;
                _isHorLeftNavDivisionCurrentlyShown = false;
            }
            else
            {
                MainSectionLeftNavDivision.Width = MainSectionLeftNavDivisionWidth;
                LeftHorizontalResizeButton.BackgroundImage = PictureList.Images["go_left.png"];
                _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.HideLeftDivision);
                MainSectionSplitter.Visible = true;
                _isHorLeftNavDivisionCurrentlyShown = true;
            }
        }

        private void HorizontalResizeButton1_Click(object sender, EventArgs e)
        {
            if (_isHorLeftNavDivision1CurrentlyShown)
            {
                MainSectionLeftNavDivision1.Width = 0;
                LeftHorizontalResizeButton1.BackgroundImage = PictureList.Images["go_right.png"];
                _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.ShowLeftDivision);
                MainSectionSplitter1.Visible = false;
                _isHorLeftNavDivision1CurrentlyShown = false;
            }
            else
            {
                MainSectionLeftNavDivision1.Width = MainSectionLeftNavDivision1Width;
                LeftHorizontalResizeButton1.BackgroundImage = PictureList.Images["go_left.png"];
                _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.HideLeftDivision);
                MainSectionSplitter1.Visible = true;
                _isHorLeftNavDivision1CurrentlyShown = true;
            }
        }

        //#proc
        private void InitFrameComponent()
        {
            BasePanel.BackColor = StyleSheet.GroundColor;

            TopNavSection.BackColor = StyleSheet.BaseColor;
            TopNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            TopNavSection.RoundStyle = RoundStyle.None;
            TopNavSection.Radius = 0;
            TopNavSection.BorderWidthOnLeft = 0;
            TopNavSection.BorderWidthOnTop = 0;
            TopNavSection.BorderWidthOnRight = 0;
            TopNavSection.BorderWidthOnBottom = 1;
            TopNavSection.BorderColor = StyleSheet.ControlBorderColor;
            TopNavSection.Padding = new Padding(2);

            ToolBarSection.BackColor = StyleSheet.BaseColor;
            ToolBarSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.None;
            ToolBarSection.RoundStyle = RoundStyle.None;
            ToolBarSection.Radius = 0;
            ToolBarSection.BorderWidthOnLeft = 0;
            ToolBarSection.BorderWidthOnTop = 0;
            ToolBarSection.BorderWidthOnRight = 0;
            ToolBarSection.BorderWidthOnBottom = 0;
            ToolBarSection.BorderColor = StyleSheet.ControlBorderColor;
            ToolBarSection.Padding = new Padding(2);
            ToolBarSectionPublicRegionToolStrip.BackColor = StyleSheet.BaseColor;
            ToolBarSectionLeftRegion.BackColor = StyleSheet.BaseColor;

            MiddleNavSection.BackColor = StyleSheet.NavigationSectionBackColor;
            MiddleNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            MiddleNavSection.RoundStyle = RoundStyle.None;
            MiddleNavSection.Radius = 0;
            MiddleNavSection.BorderWidthOnLeft = 0;
            MiddleNavSection.BorderWidthOnTop = 0;
            MiddleNavSection.BorderWidthOnRight = 0;
            MiddleNavSection.BorderWidthOnBottom = 1;
            MiddleNavSection.BorderColor = StyleSheet.ControlBorderColor;
            MiddleNavSection.Padding = new Padding(2);

            MiddleNavSectionLeftRegion.BackColor = StyleSheet.NavigationSectionBackColor;
            MiddleNavSectionRightRegion.BackColor = StyleSheet.NavigationSectionBackColor;

            DownNavSection.BackColor = StyleSheet.ShortcutSectionBackColor;
            DownNavSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            DownNavSection.RoundStyle = RoundStyle.None;
            DownNavSection.Radius = 0;
            DownNavSection.BorderWidthOnLeft = 0;
            DownNavSection.BorderWidthOnTop = 0;
            DownNavSection.BorderWidthOnRight = 0;
            DownNavSection.BorderWidthOnBottom = 1;
            DownNavSection.BorderColor = StyleSheet.ControlBorderColor;
            DownNavSection.Padding = new Padding(2);

            DownNavSectionLeftRegion.BackColor = StyleSheet.ShortcutSectionBackColor;
            DownNavSectionRightRegion.BackColor = StyleSheet.ShortcutSectionBackColor;

            MainSection.BackColor = StyleSheet.GroundColor;
            MainSection.StyleType = Ligg.RpaDesk.WinForm.Controls.ContainerPanel.ContainerPanelStyle.Borders;
            MainSection.RoundStyle = RoundStyle.None;
            MainSection.Radius = 0;
            MainSection.BorderWidthOnLeft = 0;
            MainSection.BorderWidthOnTop = 0;
            MainSection.BorderWidthOnRight = 0;
            MainSection.BorderWidthOnBottom = 1;
            MainSection.BorderColor = StyleSheet.ControlBorderColor;
            MainSection.Padding = new Padding(2);

            MainSectionLeftNavDivision.BackColor = StyleSheet.GroundColor;
            MainSectionLeftNavDivisionUpRegion.BackColor = StyleSheet.GroundColor;
            MainSectionLeftNavDivisionMiddleRegion.BackColor = StyleSheet.GroundColor;

            MainSectionLeftNavDivision1.BackColor = StyleSheet.GroundColor;
            MainSectionLeftNavDivision1UpRegion.BackColor = StyleSheet.GroundColor;
            MainSectionLeftNavDivision1MiddleRegion.BackColor = StyleSheet.GroundColor;
            MainSectionSplitter.BackColor = StyleSheet.ControlBorderColor;

            MainSectionHorizontalResizeDivision.BackColor = StyleSheet.MainSectionHorizontalResizeDivisionBackColor;
            LeftHorizontalResizeButton.Size = new Size(6, 50);
        }

        public void InitFrameHorizontalResizableDivisionStatus()
        {
            if (HorizontalResizableDivisionStatus == ResizableDivisionStatus.None)
            {
                LeftHorizontalResizeButton.Visible = false;
                MainSectionSplitter.Visible = false;
                LeftHorizontalResizeButton.Visible = false;
                MainSectionHorizontalResizeDivision.Width = 0;
                _mainSectionLeftNavDivisionWidth = MainSectionLeftNavDivisionWidth;
            }
            else if (HorizontalResizableDivisionStatus == ResizableDivisionStatus.Hidden)
            {
                LeftHorizontalResizeButton.BackgroundImage = PictureList.Images["go_right.png"];
                _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.ShowLeftDivision);
                LeftHorizontalResizeButton.Visible = true;
                MainSectionHorizontalResizeDivision.Width = MainSectionHorizontalResizeDivisionWidth;
                MainSectionSplitter.Visible = false;
                _isHorLeftNavDivisionCurrentlyShown= false;
                _mainSectionLeftNavDivisionWidth = 0;

            }
            else if (HorizontalResizableDivisionStatus == ResizableDivisionStatus.Shown)
            {
                LeftHorizontalResizeButton.BackgroundImage = PictureList.Images["go_left.png"];
                _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.HideLeftDivision);
                LeftHorizontalResizeButton.Visible = true;
                MainSectionHorizontalResizeDivision.Width = MainSectionHorizontalResizeDivisionWidth;
                MainSectionSplitter.Visible = true;
                _isHorLeftNavDivisionCurrentlyShown = true;
                _mainSectionLeftNavDivisionWidth = MainSectionLeftNavDivisionWidth;

                //var posY = Convert.ToInt16(MainSectionHorizontalResizeDivision.Height / 3.2 - LeftHorizontalResizeButton.Height / 3.2);
                //LeftHorizontalResizeButton.Location = new Point(0, posY);
            }
            else
            {
                LeftHorizontalResizeButton.Visible = false;
                MainSectionSplitter.Visible = false;
                LeftHorizontalResizeButton.Visible = false;
                MainSectionHorizontalResizeDivision.Width = 0;
                _mainSectionLeftNavDivisionWidth = MainSectionLeftNavDivisionWidth;
            }

            if (HorizontalResizableDivision1Status == ResizableDivisionStatus.None)
            {
                LeftHorizontalResizeButton1.Visible = false;
                MainSectionSplitter1.Visible = false;
                LeftHorizontalResizeButton1.Visible = false;
                MainSectionHorizontalResizeDivision1.Width = 0;
                _mainSectionLeftNavDivision1Width = MainSectionLeftNavDivision1Width;
            }
            else if (HorizontalResizableDivision1Status == ResizableDivisionStatus.Hidden)
            {
                LeftHorizontalResizeButton1.BackgroundImage = PictureList.Images["go_right.png"];
                _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.ShowLeftDivision);
                LeftHorizontalResizeButton1.Visible = true;
                MainSectionHorizontalResizeDivision1.Width = MainSectionHorizontalResizeDivision1Width;
                MainSectionSplitter1.Visible = false;
                _isHorLeftNavDivision1CurrentlyShown = false;
                _mainSectionLeftNavDivision1Width = 0;

            }
            else if (HorizontalResizableDivision1Status == ResizableDivisionStatus.Shown)
            {
                LeftHorizontalResizeButton1.BackgroundImage = PictureList.Images["go_left.png"];
                _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.HideLeftDivision);
                LeftHorizontalResizeButton1.Visible = true;
                MainSectionHorizontalResizeDivision1.Width = MainSectionHorizontalResizeDivision1Width;
                MainSectionSplitter1.Visible = true;
                _isHorLeftNavDivision1CurrentlyShown = true;
                _mainSectionLeftNavDivision1Width = MainSectionLeftNavDivision1Width;

                //var posY = Convert.ToInt16(MainSectionHorizontalResizeDivision1.Height / 3.2 - LeftHorizontalResizeButton1.Height / 3.2);
                //LeftHorizontalResizeButton1.Location = new Point(0, posY);
            }
            else
            {
                LeftHorizontalResizeButton1.Visible = false;
                MainSectionSplitter1.Visible = false;
                LeftHorizontalResizeButton1.Visible = false;
                MainSectionHorizontalResizeDivision1.Width = 0;
                _mainSectionLeftNavDivision1Width = MainSectionLeftNavDivision1Width;
            }

        }

        protected void ResizeFrameComponent()
        {
            ResizeGroundComponent();

            TopNavSection.Height = TopNavSectionHeight;
            TopNavSectionLeftRegion.Width = TopNavSectionLeftRegionWidth;
            TopNavSectionRightRegion.Width = TopNavSectionRightRegionWidth;
            TopNavSectionCenterRegion.Width = TopNavSection.Width - TopNavSectionLeftRegionWidth - TopNavSectionRightRegionWidth - 4;

            ToolBarSection.Height = ToolBarSectionHeight;
            ToolBarSectionPublicRegion.Width = ToolBarSectionPublicRegionWidth;
            ToolBarSectionLeftRegion.Width = ToolBarSectionLeftRegionWidth;
            ToolBarSectionRightRegion.Width = ToolBarSectionRightRegionWidth; 
            ToolBarSectionCenterRegion.Width = ToolBarSection.Width - ToolBarSectionPublicRegionWidth- ToolBarSectionLeftRegionWidth - ToolBarSectionRightRegionWidth - 4;

            MiddleNavSection.Height = MiddleNavSectionHeight;
            MiddleNavSectionLeftRegion.Width = MiddleNavSectionLeftRegionWidth;
            MiddleNavSectionRightRegion.Width = MiddleNavSectionRightRegionWidth;
            MiddleNavSectionCenterRegion.Width = MiddleNavSection.Width - MiddleNavSectionLeftRegionWidth - MiddleNavSectionRightRegionWidth - 4;

            DownNavSection.Height = DownNavSectionHeight;
            DownNavSectionLeftRegion.Width = DownNavSectionLeftRegionWidth;
            DownNavSectionRightRegion.Width = DownNavSectionRightRegionWidth;
            DownNavSectionCenterRegion.Width = DownNavSection.Width - DownNavSectionLeftRegionWidth - DownNavSectionRightRegionWidth - 4;

            MainSection.Height = BasePanel.Height - TopNavSectionHeight - ToolBarSectionHeight - MiddleNavSectionHeight - DownNavSectionHeight - RunningMessageSection.Height - RunningProgressSection.Height - RunningStatusSection.Height;

            MainSectionLeftNavDivision.Width = _mainSectionLeftNavDivisionWidth;
            MainSectionLeftNavDivisionUpRegion.Height = MainSectionLeftNavDivisionUpRegionHeight;
            MainSectionLeftNavDivisionDownRegion.Height = MainSectionLeftNavDivisionDownRegionHeight;
            MainSectionLeftNavDivisionMiddleRegion.Height = MainSectionLeftNavDivision.Height - MainSectionLeftNavDivisionUpRegionHeight - MainSectionLeftNavDivisionDownRegionHeight; ;

            MainSectionLeftNavDivision1.Width = _mainSectionLeftNavDivision1Width;
            MainSectionLeftNavDivision1UpRegion.Height = MainSectionLeftNavDivision1UpRegionHeight;
            MainSectionLeftNavDivision1DownRegion.Height = MainSectionLeftNavDivision1DownRegionHeight;
            MainSectionLeftNavDivision1MiddleRegion.Height = MainSectionLeftNavDivision1.Height - MainSectionLeftNavDivision1DownRegionHeight - MainSectionLeftNavDivision1UpRegionHeight; ;

            MainSectionMainDivisionUpRegion.Height = MainSectionMainDivisionUpRegionHeight;
            MainSectionMainDivisionDownRegion.Height = MainSectionMainDivisionDownRegionHeight;
            MainSectionMainDivisionMiddleRegion.Height = MainSectionMainDivision.Height - MainSectionMainDivisionUpRegionHeight - MainSectionMainDivisionDownRegionHeight;

            MainSectionRightDivision.Width = MainSectionRightDivisionWidth;
            MainSectionRightDivisionUpRegion.Height = MainSectionRightDivisionUpRegionHeight;
            MainSectionRightDivisionDownRegion.Height = MainSectionRightDivisionDownRegionHeight;
            MainSectionRightDivisionMiddleRegion.Height = MainSectionRightDivision.Height - MainSectionRightDivisionUpRegionHeight - MainSectionRightDivisionDownRegionHeight; ;


        }

        protected void SetFrameTextByCulture(bool isOnLoad, bool supportMultiCulture)
        {
            if (isOnLoad)
            {
                if (_isHorLeftNavDivisionCurrentlyShown)
                {
                    _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.HideLeftDivision);
                }
                else
                {
                    _horizontalResizeButtonToolTip.SetToolTip(LeftHorizontalResizeButton, TextRes.ShowLeftDivision);
                }

                if (_isHorLeftNavDivision1CurrentlyShown)
                {
                    _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.HideLeftDivision);
                }
                else
                {
                    _horizontalResizeButton1ToolTip.SetToolTip(LeftHorizontalResizeButton1, TextRes.ShowLeftDivision);
                }

                if (!supportMultiCulture) ToolBarSectionPublicRegionWidth = 0;
            }

            ResetGroundTextByCulture();
        }



    }
}
