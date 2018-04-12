using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ProgressBarForm : Form
    {
        public ProgressBarForm()
        {
            InitializeComponent();
        }

        public ProgressBarForm(string title, int progressBarSize)
        {
            InitializeComponent();
            Text = title;
            progressBar1.Maximum = progressBarSize;
        }

        public void UpdateProgress(int increment = 1)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { UpdateProgress(increment); });
            else
                progressBar1.Value += increment;
        }

        public void CloseForm()
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { Close(); });
            else
                Close();

        }

        public void Run(System.Threading.ThreadStart function)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() => { function(); CloseForm(); });
            thread.Start();
            ShowDialog();
        }

        public static bool DownloadFile(string url, string fileName)
        {
            var progressBarForm = new ProgressBarForm("Downloading " + url, 100);
            progressBarForm.downloadFile(url, fileName);
            return progressBarForm.ShowDialog() == DialogResult.OK;
        }

        private void downloadFile(string url, string fileName)
        {
            var wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadFileCompleted);
            wc.DownloadFileAsync(new Uri(url), fileName);
        }

        private void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void downloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DialogResult = (e.Error == null) ? DialogResult.OK : DialogResult.Cancel;
            CloseForm();
        }
    }
}
