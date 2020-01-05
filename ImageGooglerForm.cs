using com.clusterrr.hakchi_gui.Controls;
using System.Drawing;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ImageGooglerForm : Form
    {
        public Image Result { get; private set; }
        private string query;
        public ImageGooglerForm(NesApplication app)
        {
            InitializeComponent();
            query = app.Name ?? "";
            query += " " + app.Metadata.AppInfo.GoogleSuffix + " (box|cover) art";
        }

        public static string[] GetImageUrls(NesApplication app)
        {
            string query = app.Name ?? "";
            query += " " + app.Metadata.AppInfo.GoogleSuffix + " (box|cover) art";
            return ImageGoogler.GetImageUrls(query);
        }

        private void ImageGooglerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            imageGoogler.Dispose();
        }

        private void imageGoogler_OnImageDoubleClicked(Image image)
        {
            DialogResult = DialogResult.OK;
            Result = image;
            Close();
        }

        private void ImageGooglerForm_Shown(object sender, System.EventArgs e)
        {
            imageGoogler.Queries.Add(new ImageGoogler.SearchQuery()
            {
                Query = query,
                AdditionalVariables = ""
            });
            imageGoogler.RunQuery();
        }
    }
}
