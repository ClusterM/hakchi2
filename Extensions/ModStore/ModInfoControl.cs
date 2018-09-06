using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Extensions.ModStore
{
    public partial class ModInfoControl : UserControl
    {
        private Image infoStrips = new Bitmap(ModStoreResources.InfoStrips);
        private Font HeadingFont = new Font("Arial", 12, FontStyle.Bold);
        private Font DataFont = new Font("Arial", 9, FontStyle.Regular);
        private const int padding = 8;

        private string _ModuleName;
        public string ModuleName
        {
            get
            {
                return _ModuleName;
            }
            set
            {
                _ModuleName = value;
                Refresh();
            }
        }

        private string _Author;
        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                _Author = value;
                Refresh();
            }
        }

        private string _LatestVersion;
        public string LatestVersion
        {
            get
            {
                return _LatestVersion;
            }
            set
            {
                _LatestVersion = value;
                Refresh();
            }
        }

        private string _InstalledVersion;
        public string InstalledVersion
        {
            get
            {
                return _InstalledVersion;
            }
            set
            {
                _InstalledVersion = value;
                Refresh();
            }
        }

        public ModInfoControl()
        {
            InitializeComponent();
        }

        public void SetInfo(string Name, string Author, string LatestVersion, string InstalledVersion)
        {
            _ModuleName = Name;
            _Author = Author;
            _LatestVersion = LatestVersion;
            _InstalledVersion = InstalledVersion;
            Refresh();
        }
        public void Clear() => SetInfo(null, null, null, null);

        public bool IsEmpty {
            get
            {
                return _ModuleName == null && _Author == null && _LatestVersion == null && _InstalledVersion == null;
            }
        }

        private int DrawString(string text, Font font, int yOffset, ref PaintEventArgs e)
        {
            e.Graphics.DrawString(text, font, Brushes.White, new RectangleF(padding, yOffset, Width - (padding * 2), Height));
            return (int)(e.Graphics.MeasureString(text, font, Width - (padding * 2)).Height);
        }

        private void ModInfo_Paint(object sender, PaintEventArgs e)
        {
            int CurrentYOffset = padding;
            for(int i = 0; i < Height; i = i + infoStrips.Height)
            {
                e.Graphics.DrawImage(infoStrips, Width - infoStrips.Width, i);
            }

            if(_ModuleName != null && _ModuleName.Trim().Length > 0)
            {
                CurrentYOffset += DrawString("Module Name:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString($"{_ModuleName}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if(_Author != null && _Author.Trim().Length > 0)
            {
                CurrentYOffset += DrawString("Author:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString($"{_Author}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if(_LatestVersion != null && _LatestVersion.Trim().Length > 0)
            {
                CurrentYOffset += DrawString("Latest Version:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString($"{_LatestVersion}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if (_InstalledVersion != null && _InstalledVersion.Trim().Length > 0)
            {
                CurrentYOffset += DrawString("Installed Version:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString($"{_InstalledVersion}\r\n ", DataFont, CurrentYOffset, ref e);
            }
        }
    }
}
