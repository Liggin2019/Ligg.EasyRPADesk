﻿namespace Ligg.EasyWinApp.WinForm.Forms
{
    public partial class DilalogForm : BaseForm
    {
        protected int MainSectionMainDivisionUpRegionHeight = 0;
        protected int MainSectionMainDivisionMidRegionHeight = 0;
        protected DilalogForm()
        {
            InitializeComponent();
            InitZoneFormComponent();
        }


        //#proc
        private void InitZoneFormComponent()
        {
            GroundPanel.BackColor = StyleSheet.GroundColor;
            ResizeBaseComponent();
            //MainSection.Height=
            //MainSectionUpRegion.Height = MainSectionMainDivisionUpRegionHeight;
            //MainSectionMainRegion.Height = MainSectionMainDivision.Height - MainSectionMainDivisionUpRegionHeight;
            
        }






    }
}
