using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
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

        public ImageGooglerForm(NesApplication app)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(app.Name))
                Text += " - " + app.Name;
            searchThread = new Thread(SearchThread);
            searchThread.Start(app);
        }

        public static string[] GetImageUrls(NesApplication app)
        {
            string query = app.Name ?? "";
            query += " " + app.Metadata.AppInfo.GoogleSuffix + " (box|cover) art";
            var url = string.Format("https://www.google.com/search?q={0}&source=lnms&tbm=isch", HttpUtility.UrlEncode(query));
            Trace.WriteLine("Web request: " + url);
            var request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            (request as HttpWebRequest).UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            request.Timeout = 10000;
            var response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            //Trace.WriteLine("Web response: " + responseFromServer);

            var urls = new List<string>();
            string search = @"\""ou\""\:\""(?<url>.+?)\""";
            MatchCollection matches = Regex.Matches(responseFromServer, search);
            foreach (Match match in matches)
            {
                urls.Add(HttpUtility.UrlDecode(match.Groups[1].Value.Replace("\\u00", "%")));
            }

            // For some reason Google returns different data for dirrefent users (IPs?)
            // There is alternative method
            search = @"imgurl=(.*?)&";
            matches = Regex.Matches(responseFromServer, search);
            foreach (Match match in matches)
            {
                // Not sure about it.
                urls.Add(HttpUtility.UrlDecode(match.Groups[1].Value.Replace("\\u00", "%")));
            }

            return urls.ToArray();
        }

        void SearchThread(object o)
        {
            try
            {
                var urls = GetImageUrls(o as NesApplication);
                foreach (var url in urls)
                {
                    //new Thread(DownloadImageThread).Start(url);
                    try
                    {
                        Trace.WriteLine("Downloading image: " + url);
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
                Tasks.ErrorForm.Show(this, ex);
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

        public static Image DownloadImage(string url)
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
