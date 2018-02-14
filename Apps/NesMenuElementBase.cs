using com.clusterrr.hakchi_gui.Properties;
using SevenZip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    class NesMenuElementBase : INesMenuElement
    {
        protected DesktopFile desktop = new DesktopFile();

        public string Code
        {
            get { return desktop.Code; }
        }
        public string Name
        {
            get { return desktop.Name; }
            set { desktop.Name = value; }
        }
        public string SortName
        {
            get { return desktop.SortName; }
        }


    }
}

