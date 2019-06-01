using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static com.clusterrr.hakchi_gui.Tasks.Tasker;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public partial class SDFormat : Form
    {
        public class SDFormatResult
        {
            public enum CopyTypes { None, Saves, Everything }
            public bool MakeBootable;
            public bool StoreSaves;
            public CopyTypes CopyType;

            public SDFormatResult()
            {
                MakeBootable = false;
                StoreSaves = false;
                CopyType = CopyTypes.None;
            }
        }
        public SDFormatResult DialogOptions = new SDFormatResult();
        public SDFormat()
        {
            InitializeComponent();
            DialogOptions.StoreSaves = chkStoreSaves.Checked;
            DialogOptions.MakeBootable = chkBootable.Checked;
        }

        private void SetStoreSavesState()
        {
            if (chkBootable.Checked || chkCopySaves.Checked)
            {
                chkStoreSaves.Checked = true;
                chkStoreSaves.Enabled = false;
            }
            else
            {
                chkStoreSaves.Enabled = true;
            }
        }

        private void ChkBootable_CheckedChanged(object sender, EventArgs e)
        {
            SetStoreSavesState();
            lblBootable.Visible = chkBootable.Checked;
            DialogOptions.MakeBootable = chkBootable.Checked;

            chkCopyNand.Enabled = chkBootable.Checked;

            if (!chkBootable.Checked)
            {
                chkCopyNand.Checked = false;
            }
        }

        private void ChkCopyNand_CheckedChanged(object sender, EventArgs e)
        {
            chkCopySaves.Enabled = !chkCopyNand.Checked;
            if (chkCopyNand.Checked)
            {
                chkCopySaves.Checked = true;
            }
            DialogOptions.CopyType = (chkCopyNand.Checked ? SDFormatResult.CopyTypes.Everything : chkCopySaves.Checked ? SDFormatResult.CopyTypes.Saves : SDFormatResult.CopyTypes.None);
        }

        private void ChkCopySaves_CheckedChanged(object sender, EventArgs e)
        {
            SetStoreSavesState();
            DialogOptions.CopyType = (chkCopyNand.Checked ? SDFormatResult.CopyTypes.Everything : chkCopySaves.Checked ? SDFormatResult.CopyTypes.Saves : SDFormatResult.CopyTypes.None);
        }

        private void ChkStoreSaves_CheckedChanged(object sender, EventArgs e)
        {
            DialogOptions.StoreSaves = chkStoreSaves.Checked;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (Tasks.MessageForm.Show(Resources.AreYouSure, Resources.FormatSDQ, Resources.sign_warning, new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No }, Tasks.MessageForm.DefaultButton.Button1) == Tasks.MessageForm.Button.Yes)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        

        private Tasker.Conclusion InstallSDPart1(Tasker tasker, Object sync)
        {
            tasker.SetStatus(Resources.InstallingHakchi);

            if (hakchi.Shell.Execute("[ -b /dev/mmcblk0 ]") != 0)
            {
                throw new Exception(Resources.NoSDCard);
            }

            var splitStream = new SplitterStream(Program.debugStreams);
            hakchi.Shell.Execute("mkdir -p /squashtools /tmp", null, null, null, 0, true);
            hakchi.Shell.Execute("umount /newroot");
            hakchi.Shell.Execute("losetup -d /dev/loop2");
            hakchi.Shell.Execute("umount /firmware");
            hakchi.Shell.Execute("mkdir -p /sd-temp/", null, null, null, 0, true);
            tasker.SetStatus(Resources.ExtractingHakchiToTemporaryFolder);
            hakchi.Shell.Execute("tar -xzvf - -C /sd-temp/", hakchi.GetHakchiHmodStream(), null, null, 0, true); // 16
            tasker.SetProgress(16, 161);
            tasker.SetStatus(Resources.ClearingTheFirst32MBOfSDCard);
            hakchi.Shell.Execute("dd if=/dev/zero of=/dev/mmcblk0 bs=1M count=32", null, null, null, 0, true); // 32
            tasker.SetProgress(48, 161);
            if (DialogOptions.MakeBootable)
            {
                tasker.SetStatus(Resources.AddingHakchiMBR);
                hakchi.Shell.Execute("printf \"hakchi\\n%s\\n\" \"$(cat \"/sd-temp/var/version\")\" | dd \"of=/dev/mmcblk0\"", null, null, null, 0, true);
                tasker.SetProgress(49, 161);
                tasker.SetStatus(Resources.WritingFATFilesystem);
                hakchi.Shell.Execute("gunzip -c /sd-temp/sd/data.vfat.gz | dd of=/dev/mmcblk0 bs=1M seek=32", null, null, null, 0, true); // 96
                tasker.SetProgress(145, 161);
                tasker.SetStatus(Resources.WritingBoot0);
                hakchi.Shell.Execute("dd if=/sd-temp/sd/boot0.bin of=/dev/mmcblk0 bs=1K seek=8", null, null, null, 0, true); // 1
                tasker.SetProgress(146, 161);
                tasker.SetStatus(Resources.WritingUboot);
                hakchi.Shell.Execute("dd if=/sd-temp/sd/uboot.bin of=/dev/mmcblk0 bs=1K seek=19096", null, null, null, 0, true); // 1
                tasker.SetProgress(147, 161);
                tasker.SetStatus(Resources.WritingKernel);
                hakchi.Shell.Execute("dd if=/sd-temp/sd/kernel.img of=/dev/mmcblk0 bs=1K seek=20480", null, null, null, 0, true); // 4
                tasker.SetProgress(151, 161);

                tasker.SetStatus(Resources.WritingSquashFS);
                hakchi.Shell.Execute("dd \"if=/sd-temp/sd/squash.hsqs\" of=/dev/mmcblk0 bs=1K seek=40", null, null, null, 0, true); // 10
                tasker.SetProgress(161, 161);

                tasker.SetStatus(Resources.MountingSquashFS);
                
                hakchi.Shell.Execute($"losetup -o {1024 * 40} /dev/loop1 /dev/mmcblk0", null, null, null, 0, true);
                hakchi.Shell.Execute("mount /dev/loop1 /squashtools", null, null, null, 0, true);
                hakchi.Shell.Execute("cp /squashtools/init /tmp/init", null, null, null, 0, true);

                hakchi.Shell.Execute("sh /tmp/init partition", null, splitStream, splitStream, 0, true);
            }
            else
            {
                hakchi.Shell.Execute("echo sdprep | dd \"of=/dev/mmcblk0\"", null, null, null, 0, true);
                hakchi.Shell.Execute("mount /sd-temp/sd/squash.hsqs /squashtools", null, null, null, 0, true);
                hakchi.Shell.Execute("/squashtools/sfdisk /dev/mmcblk0", new MemoryStream(Encoding.ASCII.GetBytes("128M,,L\n")), splitStream, splitStream, 0, true);
            }

            return Tasker.Conclusion.Success;
        }

        private Tasker.Conclusion InstallSDPart2(Tasker tasker, Object sync)
        {
            var splitStream = new SplitterStream(Program.debugStreams);

            tasker.SetStatus(Resources.MountingSDCard);
            if (DialogOptions.MakeBootable)
            {
                hakchi.Shell.Execute("sh /tmp/init mount", null, splitStream, splitStream, 0, true);
            }
            else
            {
                hakchi.Shell.Execute("mkdir -p /data/", null, null, null, 0, true);
                hakchi.Shell.Execute("mount /dev/mmcblk0p1 /data/", null, null, null, 0, true);
            }

            hakchi.Shell.Execute("mkdir -p /data/hakchi/games/", null, null, null, 0, true);

            if (!DialogOptions.MakeBootable && DialogOptions.StoreSaves)
            {
                hakchi.Shell.Execute("mkdir -p /data/hakchi/saves/", null, null, null, 0, true);
            }

            if (DialogOptions.CopyType != SDFormatResult.CopyTypes.None)
            {
                using (EventStream copyDataProgress = new EventStream())
                {
                    splitStream.AddStreams(copyDataProgress);
                    copyDataProgress.OnData += (byte[] buffer) =>
                    {
                        tasker.SetStatus(System.Text.Encoding.ASCII.GetString(buffer));
                    };
                    tasker.SetStatus(Resources.CopyingNandDataToSDCard);
                    hakchi.Shell.Execute("mkdir -p /nandc && mount /dev/nandc /nandc", null, null, null, 0, true);

                    if (DialogOptions.CopyType == SDFormatResult.CopyTypes.Everything) {
                        hakchi.Shell.Execute("rsync -avc /nandc/ /data/", null, splitStream, splitStream, 0, true);
                    }
                    else
                    {
                        var nandPath = DialogOptions.MakeBootable ? "/nandc/clover" : "/nandc/clover/profiles/0";
                        var dataPath = DialogOptions.MakeBootable ? "/data/clover" : "/data/hakchi/saves";

                        if (hakchi.Shell.Execute($"[ -d {nandPath} ]") == 0)
                        {
                            hakchi.Shell.Execute($"rsync -avc {nandPath} {dataPath}", null, splitStream, splitStream, 0, true);
                        }
                    }

                    hakchi.Shell.Execute("umount /nandc/ && rmdir /nandc/", null, null, null, 0, true);
                    splitStream.RemoveStream(copyDataProgress);
                    copyDataProgress.Dispose();
                }
            }

            if (DialogOptions.MakeBootable)
            {
                tasker.SetStatus(Resources.CopyingHakchiToSDCard);
                hakchi.Shell.Execute("sh /tmp/init copy", null, splitStream, splitStream, 0, true);
                tasker.SetStatus(Resources.UnmountingSDCard);
                hakchi.Shell.Execute("sh /tmp/init unmount", null, splitStream, splitStream, 0, true);
            }

            return Tasker.Conclusion.Success;
        }

        public Tasker.TaskFunc[] GetTasks()
        {
            var tasks = new List<Tasker.TaskFunc>();
            tasks.AddRange(new MembootTasks(
                type: MembootTasks.MembootTaskType.MembootRecovery,
                requireSD: true
            ).Tasks);
            tasks.AddRange(new TaskFunc[] {
                MembootTasks.FlashUboot(ConfigIni.UbootType.SD),
                InstallSDPart1,
                ShellTasks.FormatDevice(DialogOptions.MakeBootable ? "/dev/mmcblk0p2" : "/dev/mmcblk0p1"),
                InstallSDPart2
            });
            tasks.Add(ShellTasks.Reboot);
            tasks.Add(MembootTasks.WaitForShellCycle());
            return tasks.ToArray();
        }
    }
}
