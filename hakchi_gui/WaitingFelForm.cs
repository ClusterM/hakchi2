using com.clusterrr.hakchi_gui.Properties;
using FelLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class WaitingFelForm : Form
    {
        bool deviceFound = false;
        public static bool DriverInstalled()
        {
            if (!Shared.isWindows)
                return true;

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "pnputil.exe",
                    Arguments = "-e",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            return proc.StandardOutput.ReadToEnd().Contains("USB\\VID_1F3A&PID_EFE8");
        }

        public static int InstallDriver()
        {
            try
            {
                int exitCode = 0;
                TempHelpers.doWithTempFolder((string temp) =>
                {
                    var fileName = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "driver"), "classic_driver.exe");
                    var process = new Process();
                    process.StartInfo.FileName = fileName;
                    process.StartInfo.WorkingDirectory = temp;
                    process.StartInfo.Verb = "runas";
                    process.Start();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                });
                return exitCode;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return -1;
            }
        }

        public WaitingFelForm()
        {
            InitializeComponent();

            if (DriverInstalled() || InstallDriver() == 0)
            {
                tableLayoutPanelDriver.Visible = false;
            }

            timer.Enabled = true;
        }

        public static bool WaitForDevice(IWin32Window owner)
        {
            if (Fel.DeviceExists()) return true;
            var form = new WaitingFelForm();
            form.ShowDialog(owner);
            return form.DialogResult == DialogResult.OK;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Fel.DeviceExists())
            {
                DialogResult = DialogResult.OK;
                timer.Enabled = false;
                deviceFound = true;
                Close();
            }
        }

        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!deviceFound && !Fel.DeviceExists())
            {
                if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.DoYouWantCancel, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button2) == Tasks.MessageForm.Button.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    DialogResult = DialogResult.Abort;
                }
            }
        }

        private void buttonDriver_Click(object sender, EventArgs e)
        {
            try
            {
                buttonDriver.Enabled = InstallDriver() != 0;
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
        }

        private void WaitingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Enabled = false;
        }

        private void WaitingFelForm_Load(object sender, EventArgs e)
        {

        }
    }
}


