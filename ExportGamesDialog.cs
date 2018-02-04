using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ExportGamesDialog : Form
    {
        private readonly string baseDrive = Path.GetPathRoot(Program.BaseDirectoryExternal).ToLower();
        public string ExportPath;
        public bool LinkedExport = false;
        public DriveInfo SelectedDrive;
        public struct DriveLetterItem
        {
            public readonly DriveInfo info;
            public string title
            {
                get
                {
                    if (info.VolumeLabel.Length > 0)
                    {
                        return $"{info.RootDirectory.FullName} ({info.VolumeLabel})";
                    }
                    else
                    {
                        return info.RootDirectory.FullName;
                    }
                }
            }

            public DriveLetterItem(DriveInfo info)
            {
                this.info = info;
            }
        }

        public ExportGamesDialog()
        {
            InitializeComponent();
            DialogResult = DialogResult.Abort;

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.IsReady == false || drive.DriveType == DriveType.Network || drive.AvailableFreeSpace == 0) continue;
                comboDriveLetters.Items.Add(new DriveLetterItem(drive));
            }

            if(comboDriveLetters.Items.Count > 0) comboDriveLetters.SelectedIndex = 0;

            if (ConfigIni.ConsoleType == MainForm.ConsoleType.SNES && ConfigIni.SeparateGameStorage)
            {
                radioEUR.Enabled = true;
                radioUSA.Enabled = true;
                switch (ConfigIni.ExportRegion)
                {
                    case "EUR":
                        radioEUR.Checked = true;
                        break;

                    case "USA":
                        radioUSA.Checked = true;
                        break;
                }
            }
            if (Program.isPortable)
            {
                foreach (DriveLetterItem drive in comboDriveLetters.Items)
                {
                    if (baseDrive == Path.GetPathRoot(drive.info.RootDirectory.FullName).ToLower())
                    {
                        comboDriveLetters.SelectedItem = drive;
                    }
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (comboDriveLetters.SelectedItem == null)
            {
                MessageBox.Show(this, Properties.Resources.NoDriveSelected, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SelectedDrive = ((DriveLetterItem)comboDriveLetters.SelectedItem).info;
                string systemCode = "";
                if (ConfigIni.SeparateGameStorage)
                {
                    switch (ConfigIni.ConsoleType)
                    {
                        case MainForm.ConsoleType.Famicom:
                            systemCode = "nes-jpn";
                            break;

                        case MainForm.ConsoleType.NES:
                            systemCode = "nes-usa";
                            break;

                        case MainForm.ConsoleType.SNES:
                            systemCode = "snes";
                            if (radioEUR.Checked) systemCode += "-eur";
                            if (radioUSA.Checked) systemCode += "-usa";
                            if (radioEUR.Checked == false && radioUSA.Checked == false)
                            {
                                MessageBox.Show(this, Properties.Resources.SelectRegion, Properties.Resources.SelectRegion, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            break;

                        case MainForm.ConsoleType.SuperFamicom:
                            systemCode = "snes-jpn";
                            break;

                        default:
                            DialogResult = DialogResult.Abort;
                            this.Close();
                            return;
                    }
                }

                if (ConfigIni.SeparateGameStorage)
                {
                    ExportPath = Shared.PathCombine(SelectedDrive.RootDirectory.FullName, "hakchi", "games", systemCode);
                }
                else
                {
                    ExportPath = Shared.PathCombine(SelectedDrive.RootDirectory.FullName, "hakchi", "games");
                }

                LinkedExport = checkLinked.Checked;

                DialogResult = DialogResult.OK;
                this.Close();
            }


        }

        private void comboDriveLetters_SelectedIndexChanged(object sender, EventArgs e)
        {
            DriveInfo drive = ((DriveLetterItem)comboDriveLetters.SelectedItem).info;
            if (baseDrive == Path.GetPathRoot(drive.RootDirectory.FullName).ToLower())
            {
                checkLinked.Enabled = true;
            }
            else
            {
                checkLinked.Checked = false;
                checkLinked.Enabled = false;
            }
        }

        private void Region_CheckedChanged(object sender, EventArgs e)
        {
            if(((RadioButton)sender).Checked) ConfigIni.ExportRegion = ((RadioButton)sender).Text;
        }
    }
}
