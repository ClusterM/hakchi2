using com.clusterrr.hakchi_gui.Properties;
using SpineGen.DrawingBitmaps;
using SpineGen.JSON;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SpineForm : Form
    {
        public Image Spine { get => pictureBoxSpine.Image; }
        public Image ClearLogo { get; private set; }
        private NesApplication App { get; set; }
        public SpineForm(NesApplication app)
        {
            InitializeComponent();
            this.App = app;
        }

        private void SpineForm_Shown(object sender, EventArgs e)
        {
            imageGoogler1.Query = $"{App.Name} {App.Metadata.AppInfo.GoogleSuffix} Clear Logo -\"project lunar\"";
            imageGoogler1.AdditionalVariables = "tbs=ift:png,ic:trans"; // Transparent PNG files
            imageGoogler1.RunQuery();
        }

        private void SpineForm_Load(object sender, EventArgs e)
        {
            foreach (var template in Program.SpineTemplates.Values)
            {
                listBoxTemplates.Items.Add(template);
            }
            listBoxTemplates.SelectedIndex = 0;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void GenerateSpine()
        {
            if (listBoxTemplates.SelectedItem is SpineTemplate<Bitmap> && ClearLogo != null)
            {
                using (var clearLogo = new SystemDrawingBitmap(new Bitmap(ClearLogo) as Bitmap))
                {
                    if (pictureBoxSpine.Image != null)
                    {
                        pictureBoxSpine.Image.Dispose();
                        pictureBoxSpine.Image = null;
                    }

                    pictureBoxSpine.Image = (listBoxTemplates.SelectedItem as SpineTemplate<Bitmap>).Process(clearLogo).Bitmap;
                }
                buttonOk.Enabled = true;
            }
            else
            {
                buttonOk.Enabled = false;
            }
        }

        private void imageGoogler1_OnImageSelected(Image image)
        {
            ClearLogo?.Dispose();
            ClearLogo = new Bitmap(image);
            GenerateSpine();
        }

        private void imageGoogler1_OnImageDeselected()
        {
            ClearLogo?.Dispose();
            ClearLogo = null;
        }

        private void listBoxTemplates_SelectedIndexChanged(object sender, EventArgs e) => GenerateSpine();

        private void buttonLoadLogo_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog() { Filter = Resources.Images + "|*.bmp;*.png;*.jpg;*.jpeg;*.gif;*.tif;*.tiff|" + Resources.AllFiles + "|*.*"})
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imageGoogler1.Deselect();
                    using (var file = File.OpenRead(ofd.FileName))
                    {
                        ClearLogo?.Dispose();
                        ClearLogo = new Bitmap(file);
                    }
                    GenerateSpine();
                }
            }
        }
    }
}
