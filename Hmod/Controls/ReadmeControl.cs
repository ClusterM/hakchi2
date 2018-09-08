using CommonMark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    public partial class ReadmeControl : UserControl
    {
        private HmodReadme Readme;

        public ReadmeControl()
        {
            InitializeComponent();
            clear();
        }

        private string formatReadme(ref HmodReadme hReadme)
        {
            

            return CommonMarkConverter.Convert(String.Join("  \n", hReadme.headingLines) + "\n\n" + (hReadme.isMarkdown || hReadme.readme.Length == 0 ? hReadme.readme : $"```\n{hReadme.readme}\n```"));
        }

        private void setReadmeHTML(string name, ref HmodReadme hReadme)
        {
            Color color = this.BackColor;
            string html = String.Format(Properties.Resources.readmeTemplateHTML, Properties.Resources.readmeTemplateCSS, name, formatReadme(ref Readme), $"rgb({color.R},{color.G},{color.B})");
            wbReadme.DocumentText = html;
        }

        public void setReadme(string name, string readme = "", bool markdown = false)
        {
            setReadme(name, new HmodReadme(readme, markdown));
        }

        public void setReadme(string name, HmodReadme hReadme)
        {
            Readme = hReadme;
            setReadmeHTML(name, ref Readme);
        }

        public void clear()
        {
            Readme = new HmodReadme("");
            Color color = this.BackColor;
            string html = String.Format(Properties.Resources.readmeTemplateHTML, Properties.Resources.readmeTemplateCSS, "", "", $"rgb({color.R},{color.G},{color.B})");
            wbReadme.DocumentText = html;
        }

        private void wbReadme_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString() == "about:blank") return;

            //cancel the current event
            e.Cancel = true;

            //this opens the URL in the user's default browser
            Process.Start(e.Url.ToString());
        }
    }
}
