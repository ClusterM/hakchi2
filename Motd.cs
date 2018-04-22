using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CommonMark;

namespace com.clusterrr.hakchi_gui
{
    public partial class Motd : Form
    {
        private string html;

        public Motd(string message)
        {
            InitializeComponent();

            Color color = this.BackColor;
            string text = CommonMarkConverter.Convert(message);
            this.html = String.Format(
                Properties.Resources.motdTemplateHTML,
                Properties.Resources.motdTemplateCSS,
                this.Text,
                text,
                $"rgb({color.R},{color.G},{color.B})");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Motd_Shown(object sender, EventArgs e)
        {
            webBrowser.DocumentText = html;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() == "about:blank") return;

            //cancel the current event
            e.Cancel = true;

            //this opens the URL in the user's default browser
            Process.Start(e.Url.ToString());
        }
    }
}
