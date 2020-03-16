using SharpCompress.Archives;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class LicenseInfo : Form
    {
        public LicenseInfo()
        {
            InitializeComponent();

            var licenses = new List<string>();

            using (var licenseMs = new MemoryStream(Properties.Resources.LicensesTar))
            using (var extractor = ArchiveFactory.Open(licenseMs))
            using (var reader = extractor.ExtractAllEntries())
            {
                while (reader.MoveToNextEntry())
                {
                    using (var entryStream = reader.OpenEntryStream())
                    using (var sr = new StreamReader(entryStream))
                    {
                        var license = sr.ReadToEnd();
                        if (license.Length > 0)
                        {
                            licenses.Add(license);
                        }
                    }
                }
            }

            licenses.Sort();

            textBoxLicenses.Text = string.Join("\n--------------------------------------------------------------------------------\n", licenses.ToArray()).Replace("\r", "").Replace("\n", "\r\n");
        }

        private void LicenseInfo_Shown(object sender, System.EventArgs e)
        {
            textBoxLicenses.DeselectAll();
        }
    }
}
