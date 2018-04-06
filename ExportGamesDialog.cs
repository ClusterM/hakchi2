using com.clusterrr.hakchi_gui.Properties;
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
                if (drive.IsReady == false || drive.AvailableFreeSpace == 0) continue;
                comboDriveLetters.Items.Add(new DriveLetterItem(drive));
            }

            if (comboDriveLetters.Items.Count > 0) comboDriveLetters.SelectedIndex = 0;

            if (ConfigIni.Instance.ConsoleType == hakchi.ConsoleType.SNES_EUR)
                radioEUR.Checked = true;
            if (ConfigIni.Instance.ConsoleType == hakchi.ConsoleType.SNES_USA)
                radioUSA.Checked = true;

            if (!string.IsNullOrEmpty(ConfigIni.Instance.ExportDrive))
            {
                foreach (DriveLetterItem drive in comboDriveLetters.Items)
                {
                    if (ConfigIni.Instance.ExportDrive == Path.GetPathRoot(drive.info.RootDirectory.FullName).ToLower())
                    {
                        comboDriveLetters.SelectedItem = drive;
                        break;
                    }
                }
            }
            else if (Program.isPortable)
            {
                foreach (DriveLetterItem drive in comboDriveLetters.Items)
                {
                    if (baseDrive == Path.GetPathRoot(drive.info.RootDirectory.FullName).ToLower())
                    {
                        comboDriveLetters.SelectedItem = drive;
                        break;
                    }
                }
            }
            checkLinked.Checked = ConfigIni.Instance.ExportLinked;
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
                Tasks.MessageForm.Show(Resources.ExportGames, Resources.NoDriveSelected, Resources.sign_error);
            }
            else
            {
                SelectedDrive = ((DriveLetterItem)comboDriveLetters.SelectedItem).info;
                string systemCode = "";
                if (ConfigIni.Instance.SeparateGameStorage)
                {
                    switch (ConfigIni.Instance.ConsoleType)
                    {
                        case hakchi.ConsoleType.Famicom:
                            systemCode = "nes-jpn";
                            break;
                        case hakchi.ConsoleType.NES:
                            systemCode = "nes-usa";
                            break;
                        case hakchi.ConsoleType.SNES_EUR:
                            systemCode = "snes-eur";
                            break;
                        case hakchi.ConsoleType.SNES_USA:
                            systemCode = "snes-usa";
                            break;
                        case hakchi.ConsoleType.SuperFamicom:
                            systemCode = "snes-jpn";
                            break;

                        default:
                            DialogResult = DialogResult.Abort;
                            this.Close();
                            return;
                    }
                }

                if (ConfigIni.Instance.SeparateGameStorage)
                {
                    ExportPath = Shared.PathCombine(SelectedDrive.RootDirectory.FullName, "hakchi", "games", systemCode);
                }
                else
                {
                    ExportPath = Shared.PathCombine(SelectedDrive.RootDirectory.FullName, "hakchi", "games");
                }

                ConfigIni.Instance.ExportDrive = Path.GetPathRoot(SelectedDrive.RootDirectory.FullName).ToLower();
                LinkedExport = checkLinked.Enabled && checkLinked.Checked;

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
                checkLinked.Enabled = false;
            }
        }

        private void Region_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void checkLinked_CheckedChanged(object sender, EventArgs e)
        {
            ConfigIni.Instance.ExportLinked = ((CheckBox)sender).Checked;
        }
    }
}
