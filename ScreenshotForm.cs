using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ScreenshotForm : Form
    {
        private readonly string unattendedPath = Path.Combine(Program.BaseDirectoryExternal, "screenshots");
        private bool liveView;
        private string formatTitle;
        int counter = 0;

        public ScreenshotForm(bool liveView = false)
        {
            this.liveView = liveView;
            InitializeComponent();
            formatTitle = Text;
            Directory.CreateDirectory(unattendedPath);
            saveImageFileDialog.InitialDirectory = unattendedPath;

            new Thread(() =>
            {
                try
                {
                    var action = new Action(LoadScreenshot);
                    while (true)
                    {
                        if(this.liveView)
                            Invoke(action);

                        Thread.Sleep(1000);
                        
                    }
                }
                catch(ThreadAbortException)
                {
                    if (hakchi.Shell.IsOnline)
                        hakchi.Shell.Execute("hakchi uiresume");
                }
                catch { }
            }).Start();
        }

        private void ScreenshotForm_Load(object sender = null, EventArgs e = null)
        {
            if (liveView)
            {
                startStopLiveViewToolStripMenuItem.Text = Resources.DisableAutoRefresh;
            }
            else
            {
                LoadScreenshot();
            }
        }

        async private void LoadScreenshot()
        {
            DateTime currentTime = DateTime.Now;
            Text = String.Format(formatTitle, currentTime.ToShortDateString(), currentTime.ToLongTimeString());
            screenshotPictureBox.Image = await TakeScreenshot();
        }

        async public static Task<Image> TakeScreenshot()
        {
            return await Task<Image>.Factory.StartNew(() =>
            {
                if (!hakchi.Shell.IsOnline)
                    return null;

                try
                {
                    var screenshot = new Bitmap(1280, 720, PixelFormat.Format24bppRgb);
                    var rawStream = new MemoryStream();
                    hakchi.Shell.ExecuteSimple("hakchi uipause");
                    hakchi.Shell.Execute("cat /dev/fb0", null, rawStream, null, 1000, true);
                    hakchi.Shell.ExecuteSimple("hakchi uiresume");
                    var raw = rawStream.ToArray();
                    BitmapData data = screenshot.LockBits(
                        new Rectangle(0, 0, screenshot.Width, screenshot.Height),
                        ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    int rawOffset = 0;
                    unsafe
                    {
                        for (int y = 0; y < screenshot.Height; ++y)
                        {
                            byte* row = (byte*)data.Scan0 + (y * data.Stride);
                            int columnOffset = 0;
                            for (int x = 0; x < screenshot.Width; ++x)
                            {
                                row[columnOffset] = raw[rawOffset];
                                row[columnOffset + 1] = raw[rawOffset + 1];
                                row[columnOffset + 2] = raw[rawOffset + 2];

                                columnOffset += 3;
                                rawOffset += 4;
                            }
                        }
                    }
                    screenshot.UnlockBits(data);
                    return screenshot;
                }
                catch
                {
                    return null;
                }
            });
        }

        public void SaveScreenshot(string screenshotPath)
        {
            if (screenshotPictureBox.Image is null)
                return;

            screenshotPictureBox.Image.Save(screenshotPath, ImageFormat.Png);
        }
        public void OpenInDefaultViewer()
        {
            OpenInDefaultViewer(null, null);
        }
        private void OpenInDefaultViewer(object sender, EventArgs e)
        {
            var screenshotPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
            SaveScreenshot(screenshotPath);
            var showProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = screenshotPath
                }
            };
            showProcess.Start();
            new Thread(delegate ()
            {
                try
                {
                    Thread.Sleep(5000);
                    showProcess.WaitForExit();
                }
                catch { }
                try
                {
                    File.Delete(screenshotPath);
                }
                catch { }
            }).Start();
        }

        private void saveImageToFile(object sender = null, EventArgs e = null)
        {
            saveImageFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + string.Format("_{0:0000}", counter++) + ".png";
            if (saveImageFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                SaveScreenshot(saveImageFileDialog.FileName);
            }
        }

        private void screenshotPictureBox_DoubleClick(object sender, EventArgs e)
        {
            OpenInDefaultViewer(sender, e);
        }

        private void LiveView(object sender = null, EventArgs e = null)
        {
            if (liveView)
            {
                startStopLiveViewToolStripMenuItem.Text = Resources.EnableAutoRefresh;
            }
            else
            {
                startStopLiveViewToolStripMenuItem.Text = Resources.DisableAutoRefresh;
            }

            liveView = !liveView;
        }

        private void copyImageToClipboard(object sender = null, EventArgs e = null)
        {
            if (screenshotPictureBox.Image is null)
                return;

            Clipboard.SetImage(screenshotPictureBox.Image);
        }

        private void updateScreenshot(object sender = null, EventArgs e = null)
        {
            LoadScreenshot();
        }

        private void unattendedScreenshot()
        {
            LoadScreenshot();
            try
            {
                if (!Directory.Exists(unattendedPath))
                {
                    Directory.CreateDirectory(unattendedPath);
                }
                string fileName = Path.Combine(unattendedPath, DateTime.Now.ToString("yyyyMMdd_HHmmss") + string.Format("_{0:0000}", counter++) + ".png");
                if (counter > 9999) counter = 0;
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                SaveScreenshot(fileName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error saving unattended screenshot: " + ex.Message + ex.StackTrace);
            }
        }

        private void ScreenshotForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                OpenInDefaultViewer();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                copyImageToClipboard();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                saveImageToFile();
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F5)
            {
                LiveView();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F5)
            {
                LoadScreenshot();
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.F11)
            {
                unattendedScreenshot();
            }
        }
    }
}
