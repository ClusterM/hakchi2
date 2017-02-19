using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;

namespace com.clusterrr.hakchi_gui
{
    public class N64Game : NesMiniApplication
    {
        public const char Prefix = '6';
        public static Image DefaultCover { get { return Resources.blank_snes_us; } } // TODO: need N64 icons
        public const string DefaultApp = "/bin/n64";

        public N64Game(string path, bool ignoreEmptyConfig)
            : base(path, ignoreEmptyConfig)
        {
        }
    }
}

