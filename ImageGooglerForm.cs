using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ImageGooglerForm : Form
    {
        Thread searchThread;
        Image result;
        public Image Result
        {
            get { return result; }
        }

        public ImageGooglerForm(string query)
        {
            InitializeComponent();
            Text = "Google Images - " + query;
            searchThread = new Thread(SearchThread);
            searchThread.Start(query);
        }

        string[] GetImageUrls(string query)
        {
            var url = string.Format("https://www.google.com/search?q={0}&source=lnms&tbm=isch", HttpUtility.UrlEncode(query));
            var request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            (request as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            request.Timeout = 10000;
            var response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            var pageData = new StringBuilder();
            int b;
            var result = new List<byte>();
            while ((b = dataStream.ReadByte()) >= 0)
                result.Add((byte)b);
            dataStream.Close();
            response.Close();
            var page = Encoding.UTF8.GetString(result.ToArray());

            var r = new Regex(@"\""ou\""\:\""(?<url>.+?)\""");
            var mc = r.Matches(page);
            var urls = new List<string>();
            for (int i = 0; i < mc.Count && i < 20; i++)
            {
                var gr = mc[i].Groups;
                urls.Add(gr["url"].Value);
            }
            return urls.ToArray();
        }

        void SearchThread(object o)
        {
            try
            {
                var urls = GetImageUrls(o as string);
                foreach (var url in urls)
                {
                    //new Thread(DownloadImageThread).Start(url);
                    try
                    {
                        var image = DownloadImage(url);
                        ShowImage(image);
                    }
                    catch { }
                }

            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searchThread = null;
            }
        }

        void DownloadImageThread(object o)
        {
            try
            {
                var image = DownloadImage(o as string);
                ShowImage(image);
            }
            catch { }
        }

        protected void ShowImage(Image image)
        {
            try
            {
                if (this.Disposing) return;
                if (InvokeRequired)
                {
                    Invoke(new Action<Image>(ShowImage), new object[] { image });
                    return;
                }
                int i = imageList.Images.Count;
                const int side = 204;
                var imageRect = new Bitmap(side, side, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var gr = Graphics.FromImage(imageRect);
                gr.Clear(Color.White);
                if (image.Height > image.Width)
                    gr.DrawImage(image, new Rectangle((side - side * image.Width / image.Height) / 2, 0, side * image.Width / image.Height, side),
                        new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                else
                    gr.DrawImage(image, new Rectangle(0, (side - side * image.Height / image.Width) / 2, side, side * image.Height / image.Width),
                        new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
                gr.Flush();
                imageList.Images.Add(imageRect);
                var item = new ListViewItem(image.Width + "x" + image.Height);
                item.ImageIndex = i;
                item.Tag = image;
                listView.Items.Add(item);
            }
            catch { }
        }


        Image DownloadImage(string url)
        {
            var request = HttpWebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 5000;
            ((HttpWebRequest)request).UserAgent =
                "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";
            ((HttpWebRequest)request).KeepAlive = false;
            var response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            var image = Image.FromStream(dataStream);
            dataStream.Dispose();
            response.Close();
            return image;
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0) return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            result = listView.SelectedItems[0].Tag as Image;
            Close();
        }

        private void ImageGooglerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (searchThread != null) searchThread.Abort();
        }

    }
}
