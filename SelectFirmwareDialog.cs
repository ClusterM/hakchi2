using com.clusterrr.hakchi_gui;
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectFirmwareDialog : Form
    {
        public SelectFirmwareDialog()
        {
            InitializeComponent();

            try
            {
                new Thread(loadFirmwares).Start();
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
                Close();
            }
        }

        private List<string> firmwarePaths;

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (!WaitingShellForm.WaitForDevice(this))
            {
                Close();
                return;
            }

            var selected = listBoxFirmwares.SelectedIndex;
            if (selected == -1 || selected > firmwarePaths.Count)
            {
                Close();
                return;
            }

            try
            {
                var firmwareCurrent = hakchi.Shell.ExecuteSimple("hakchi currentFirmware", 1000, true);
                var firmwareFullPath = selected == 0 ? "_nand_" : firmwarePaths[selected - 1];

                if (firmwareCurrent == firmwareFullPath)
                {
                    Close();
                    return;
                }

                try
                {
                    hakchi.Shell.ExecuteSimple($"/bin/hsqs \"{firmwareFullPath}\"", 500, false);
                }
                catch { } // no-op
                Tasks.MessageForm.Show(this, Resources.Firmware, string.Format(Resources.FirmwareSwitched, (string)listBoxFirmwares.SelectedItem), Resources.sign_check, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.OK });
            }
            catch (Exception ex)
            {
                Tasks.ErrorForm.Show(this, ex);
            }
            Close();
        }

        private void loadFirmwares()
        {
            try
            {
                while (!IsHandleCreated) Thread.Sleep(100);
                Invoke(new Action(delegate {
                    if (!WaitingShellForm.WaitForDevice(this))
                    {
                        Close();
                        return;
                    }
                }));

                var firmwareCurrent = hakchi.Shell.ExecuteSimple("hakchi currentFirmware", 1000, true);
                var firmwareList = hakchi.Shell.ExecuteSimple("find \"/var/lib/hakchi/\" -iname \"*.hsqs\"", 10000, true);
                firmwarePaths = new List<string>();
                foreach (var firmware in firmwareList.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    firmwarePaths.Add(firmware);
                }

                Invoke(new Action(delegate
                {
                    listBoxFirmwares.Items.Clear();
                    listBoxFirmwares.Items.Add(Resources.FirmwareDefault + (firmwareCurrent == "_nand_" ? $" ({Resources.FirmwareCurrent})" : ""));
                    listBoxFirmwares.Items.AddRange(firmwarePaths.Select(
                        f => Path.GetFileNameWithoutExtension(f) + (f == firmwareCurrent ? $" ({Resources.FirmwareCurrent})" : "")).ToArray());
                    buttonOk.Enabled = true;
                }));

            }
            catch (ThreadAbortException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                try
                {
                    Tasks.ErrorForm.Show(this, ex);
                }
                catch { }
            }
        }
    }
}
