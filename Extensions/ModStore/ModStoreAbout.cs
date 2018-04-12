using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace com.clusterrr.hakchi_gui
{
    partial class ModStoreAbout : Form
    {
        public ModStoreAbout()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", "Hakchi Mod Store");
            this.labelProductName.Text = "Hakchi Mod Store";
            this.labelCopyright.Text = "'TheOtherGuys'";
            this.labelCompanyName.Text = "http://www.hackhiresources.com";
            this.textBoxDescription.Text = "Hakchi Mod Store:\r\n" +
                "Developed by CompCom and Swingflip ('TheOtherGuys')\r\n" +
                "Powered by www.hakchiresources.com\r\n" +
                "Exclusively for Hakchi2ce\r\n\r\n" +
                "Special thanks to 'TheOtherGuys', 'TeamShinkansen', 'TeamHakchiResources''";
        }

        private void labelCompanyName_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }
    }
}
