using CommonMark;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Hmod.Controls
{
    public partial class TextReadmeControl : UserControl, IReadmeControl
    {
        private HmodReadme Readme;

        public TextReadmeControl()
        {
            InitializeComponent();

            clear();
        }

        private void setReadmeHTML(string name, ref HmodReadme hReadme)
        {
            tbReadme.Text = hReadme.rawReadme.Replace("\r", "").Replace("\n", "\r\n").Trim();
            return;
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

            tbReadme.Text = string.Empty;
            return;
        }
    }
}
