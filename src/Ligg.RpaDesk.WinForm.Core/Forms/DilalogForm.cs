
using Ligg.WinFormBase;
namespace Ligg.RpaDesk.WinForm.Forms
{
    public partial class DilalogForm : GroundForm
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
            BasePanel.BackColor = StyleSheet.GroundColor;
            ResizeGroundComponent();
            //MainSection.Height=
            //MainSectionUpRegion.Height = MainSectionMainDivisionUpRegionHeight;
            //MainSectionMainRegion.Height = MainSectionMainDivision.Height - MainSectionMainDivisionUpRegionHeight;
            
        }






    }
}
