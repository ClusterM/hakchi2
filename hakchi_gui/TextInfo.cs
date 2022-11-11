using SharpCompress.Archives;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class TextInfo : Form
    {
        public TextInfo()
        {
            InitializeComponent();
        }

        private void TextInfo_Shown(object sender, System.EventArgs e)
        {
            textBoxInfo.DeselectAll();
        }
    }
}
