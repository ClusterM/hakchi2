using System.Drawing;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Extensions.ModStore
{
    public partial class ModInfoControl : UserControl
    {
        private Bitmap _infoStrips = ModStoreResources.InfoStrips;
        public Bitmap infoStrips {
            get
            {
                return _infoStrips;
            }
            set
            {
                _infoStrips = value;
                Refresh();
            }
        }

        private Color _textColor = Color.White;
        public Color textColor
        {
            get
            {
                return _textColor;
                
            }
            set
            {
                _textColor = value;
                Refresh();
            }
        }

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

        private int DrawString(Brush brush, string text, Font font, int yOffset, ref PaintEventArgs e)
        {
            e.Graphics.DrawString(text, font, brush, new RectangleF(padding, yOffset, Width - (padding * 2), Height));
            return (int)(e.Graphics.MeasureString(text, font, Width - (padding * 2)).Height);
        }

        private void ModInfo_Paint(object sender, PaintEventArgs e)
        {
            var brush = new SolidBrush(_textColor);

            int CurrentYOffset = padding;
            if (_infoStrips != null)
            {
                for (int i = 0; i < Height; i = i + _infoStrips.Height)
                {
                    e.Graphics.DrawImage(_infoStrips, Width - _infoStrips.Width, i);
                }
            }

            if(_ModuleName != null && _ModuleName.Trim().Length > 0)
            {
                CurrentYOffset += DrawString(brush, "Module Name:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString(brush, $"{_ModuleName}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if(_Author != null && _Author.Trim().Length > 0)
            {
                CurrentYOffset += DrawString(brush, "Author:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString(brush, $"{_Author}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if(_LatestVersion != null && _LatestVersion.Trim().Length > 0)
            {
                CurrentYOffset += DrawString(brush, "Latest Version:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString(brush, $"{_LatestVersion}\r\n ", DataFont, CurrentYOffset, ref e);
            }

            if (_InstalledVersion != null && _InstalledVersion.Trim().Length > 0)
            {
                CurrentYOffset += DrawString(brush, "Installed Version:", HeadingFont, CurrentYOffset, ref e);
                CurrentYOffset += DrawString(brush, $"{_InstalledVersion}\r\n ", DataFont, CurrentYOffset, ref e);
            }
        }
    }
}
