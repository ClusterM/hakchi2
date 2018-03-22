using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.FelLib;
using com.clusterrr.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class KernelTask
    {
        readonly Dictionary<MainForm.ConsoleType, string[]> correctKernels = new Dictionary<MainForm.ConsoleType, string[]>();
        readonly Dictionary<MainForm.ConsoleType, string[]> correctKeys = new Dictionary<MainForm.ConsoleType, string[]>();
        const long maxCompressedsRamfsSize = 30 * 1024 * 1024;
        const long reservedMemory = 30 * 1024 * 1024;
        const UInt16 vid = 0x1F3A;
        const UInt16 pid = 0xEFE8;
        Fel fel = null;

        // path contstants
#if VERY_DEBUG
        readonly string tempDir = Path.Combine(externalDir, "temp");
#else
        readonly string tempDir = Path.GetTempPath();
#endif
        readonly string internalDir = Program.BaseDirectoryInternal;
        readonly string externalDir = Program.BaseDirectoryExternal;
        readonly string dataDir = Path.Combine(Program.BaseDirectoryInternal, "data");
        readonly string toolsDir = Path.Combine(Program.BaseDirectoryInternal, "tools");
        readonly string fes1File = Path.Combine(Program.BaseDirectoryInternal, "data", "fes1.bin");
        readonly string ubootFile = Path.Combine(Program.BaseDirectoryInternal, "data", "uboot.bin");
        readonly string ubootSDFile = Path.Combine(Program.BaseDirectoryInternal, "data", "ubootSD.bin");
        readonly string zImageFile = Path.Combine(Program.BaseDirectoryInternal, "data", "zImage");

        public KernelTask()
        {
            correctKernels[MainForm.ConsoleType.NES] = new string[] {
                "5cfdca351484e7025648abc3b20032ff",
                "07bfb800beba6ef619c29990d14b5158",
            };
            correctKernels[MainForm.ConsoleType.Famicom] = new string[] {
                "ac8144c3ea4ab32e017648ee80bdc230",  // Famicom Mini
            };
            correctKernels[MainForm.ConsoleType.SNES] = new string[] {
                "d76c2a091ebe7b4614589fc6954653a5", // SNES Mini (EUR)
                "c2b57b550f35d64d1c6ce66f9b5180ce", // SNES Mini (EUR)
                "0f890bc78cbd9ede43b83b015ba4c022", // SNES Mini (EUR)
                "449b711238575763c6701f5958323d48", // SNES Mini (USA)
                "5296e64818bf2d1dbdc6b594f3eefd17", // SNES Mini (USA)
                "228967ab1035a347caa9c880419df487", // SNES Mini (USA)
            };
            correctKernels[MainForm.ConsoleType.SuperFamicom] = new string[]
            {
                "632e179db63d9bcd42281f776a030c14", // Super Famicom Mini (JAP)
                "c3378edfc1b96a5268a066d5fbe12d89", // Super Famicom Mini (JAP)
            };
            correctKeys[MainForm.ConsoleType.NES] =
                correctKeys[MainForm.ConsoleType.Famicom] =
                new string[] { "bb8f49e0ae5acc8d5f9b7fa40efbd3e7" };
            correctKeys[MainForm.ConsoleType.SNES] =
                correctKeys[MainForm.ConsoleType.SuperFamicom] =
                new string[] { "c5dbb6e29ea57046579cfd50b124c9e1" };
        }

        bool WaitForFel(TaskerForm tasker)
        {
            if (tasker.InvokeRequired)
            {
                return (bool)tasker.Invoke(new Func<TaskerForm, bool>(WaitForFel), new object[]{ tasker });
            }
            var prevState = tasker.SetState(TaskerForm.State.Paused);
            tasker.SetStatus(Resources.WaitingForDevice);

            if (fel != null) fel.Close();
            var result = WaitingFelForm.WaitForDevice(vid, pid, tasker);

            tasker.SetState(prevState);
            if (result)
            {
                fel = new Fel();
                if (!File.Exists(fes1File)) throw new FileNotFoundException(fes1File + " not found");
                if (!File.Exists(ubootFile)) throw new FileNotFoundException(ubootFile + " not found");
                fel.Fes1Bin = File.ReadAllBytes(fes1File);
                fel.UBootBin = File.ReadAllBytes(ubootFile);
                if (!fel.Open(vid, pid)) throw new FelException("Can't open device");
                tasker.SetStatus(Resources.UploadingFes1);
                fel.InitDram(true);
                return true;
            }
            return false;
        }

        bool WaitForShell(TaskerForm tasker)
        {
            if (tasker.InvokeRequired)
            {
                return (bool)tasker.Invoke(new Func<TaskerForm, bool>(WaitForShell), new object[] { tasker });
            }
            TaskerForm.State prevState = tasker.SetState(TaskerForm.State.Paused);
            tasker.SetStatus(Resources.WaitingForDevice);

            bool result = WaitingClovershellForm.WaitForDevice(tasker);
            tasker.SetState(prevState);
            return result;
        }
        
        public TaskerForm.Conclusion DoDump(TaskerForm tasker, Object syncObject = null)
        {
            return TaskerForm.Conclusion.Success;
        }
    }
}
