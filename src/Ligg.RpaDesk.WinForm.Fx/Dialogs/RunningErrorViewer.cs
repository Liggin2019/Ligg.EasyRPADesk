using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using  Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.WinForm.Helpers;
using Ligg.RpaDesk.WinForm.Resources;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Dialogs
{
    public partial class RunningErrorViewer
    {
        public string AdditionalInfo { get; set; }
        public string ErrorText { get; set; }
        public string ErrorMsg { get; set; }
        public string OccurringTime { get; set; }

        public int FormWidth;
        private double _hwRatio;

        public RunningErrorViewer()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            if (Owner != null)
            {
                Left = Owner.Location.X + (Owner.Width / 2 - Width / 2);
            }
            else
            {
                var rect = new Rectangle();
                rect = Screen.GetWorkingArea(this);
                Left = rect.Width / 2 - Width / 2;
            }
            //imageTextButtonSendErrorInfo.st = "2,5,1";
            imageTextButtonSendErrorInfo.Offset = "2,5,1";
            BasePanel.BackColor = StyleSheet.NavigationSectionBackColor;
        }


        private void RunningErrorViewer_Load(object sender, EventArgs e)
        {
            Text = TextRes.ApplicationRunningError;
            //var ddd = ValidationRes.EmailFormat;
            imageTextButtonSendErrorInfo.Text = TextRes.SendErrorInfoToDev;
            labelError.Text = TextRes.ErrorMsg + ":";
            richTextBoxError.Text = ErrorText + "; "+ErrorMsg;
            richTextBoxError.Height = Height - panelTop.Height - panelBottom.Height - 28;
            OccurringTime = SystemTimeHelper.UtcNow().ToString("yy-MM-dd HH:mm");
        }

        private void richTextBoxError_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FormWidth = Width + 50;
            ResizeComponent();
        }

        private void imageTextButtonSendErrorInfo_Click(object sender, EventArgs e)
        {
            var userName = AdditionalInfo.ExtractSubStringsByTwoDifferentItentifiers("UserName:", ",", true) == null ?
                "" : AdditionalInfo.ExtractSubStringsByTwoDifferentItentifiers("UserName:", ",", true)[0];
            var application = AdditionalInfo.ExtractSubStringsByTwoDifferentItentifiers("Application:", ",", true)[0];
            var architecture = AdditionalInfo.ExtractSubStringsByTwoDifferentItentifiers("Architecture:", ",", true)[0];
            var helpdeskMail = AdditionalInfo.ExtractSubStringsByTwoDifferentItentifiers("Helpdeskemail:", ",", true)[0];

            var valueText = new ValueText { Value = ".NET Framework", Text = "" };
            var listValueText = new List<ValueText>();
            listValueText.Add(valueText);
            var installedNetFxes = SystemInfoHelper.GetInstalledSoftwareList(listValueText);
            var installedNetFxesStr = "";
            for (int index = 0; index < installedNetFxes.Count; index++)
            {
                var installedSoftware = installedNetFxes[index];
                if (index == 0)
                {
                    installedNetFxesStr = installedSoftware.ProductName + " " +
                                      installedSoftware.VersionString;
                }
                else
                {
                    installedNetFxesStr = installedNetFxesStr + "; " + installedSoftware.ProductName + " " +
                                       installedSoftware.VersionString;
                }

            }
            var body = "Dear Developer%0D%0A"
                       + "Following is system running error information for you:" + "%0D%0A" 
                       + " EXCEPTION MESSAGE: " + ErrorMsg + "%0D%0A"
                       + "Architecture: " + architecture + "%0D%0A"
                       + "Application: " + application + "%0D%0A"
                       + (userName.IsNullOrEmpty() ? "" : "UserName" + userName + "%0D%0A")

                       + "%0D%0A"
                       + "ADDITIONAL INFORMATION:" + "%0D%0A"
                       + "Assembly Bits : " + IntPtr.Size * 8 + "%0D%0A"
                       + "OS : " + SystemInfoHelper.GetSystemInfo("osinfo") + " " + SystemInfoHelper.GetSystemInfo("osbit") + " bits" + "%0D%0A"
                       + "Machine Name: " + SystemInfoHelper.GetSystemInfo("machinename") + "%0D%0A"
                       + "IP: " + SystemInfoHelper.GetSystemInfo("Ips") + "%0D%0A"
                       + "Windows Account: " + SystemInfoHelper.GetSystemInfo("currentuser") + "%0D%0A"
                       + "Installed dotNetFx: " + installedNetFxesStr + "%0D%0A"

                       + "%0D%0A"
                       + "Occurring Time: " + OccurringTime + "%0D%0A";

            body = body.Replace("\n", "%0D%0A");
            var subject = ErrorText + ", occured at " + OccurringTime;
            LocalEmailHelper.Send(helpdeskMail, subject, body);

        }

        private void ResizeComponent()
        {
            _hwRatio = Height * 1.00 / Width;
            this.Width = FormWidth;
            this.Height = Convert.ToInt32(Width * _hwRatio);
            richTextBoxError.Height = Height - panelTop.Height - panelBottom.Height - 28;
        }

    }
}
