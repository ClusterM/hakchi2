using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    static class SfromToolWrapper
    {
        static private string toolPath = null;
        static public bool IsInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(toolPath))
                {
                    string[] files = Directory.GetFiles(Program.BaseDirectoryExternal, "SFROM Tool.exe", SearchOption.AllDirectories);
                    if (files.Length == 0)
                        return false;
                    toolPath = files[0];
                }
                return true;
            }
        }

        static public bool ConvertROMtoSFROM(ref byte[] rawRomData)
        {
            if (!IsInstalled) return false;

#if VERY_DEBUG
            var tempPath = Path.Combine(Program.BaseDirectoryExternal, "temp");
#else
            var tempPath = Path.Combine(Path.GetTempPath(), "hakchi-temp");
#endif

            var tempFile = Path.Combine(tempPath, "temp.sfc");
            var outTempFile = Path.Combine(tempPath, "temp.sfrom");
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            if (File.Exists(tempFile)) File.Delete(tempFile);
            if (File.Exists(outTempFile)) File.Delete(outTempFile);

            File.WriteAllBytes(tempFile, rawRomData);
            string sfromargs = string.Empty;
            if (ConfigIni.UsePCMPatch)
                sfromargs += "p";
            if (!string.IsNullOrEmpty(sfromargs))
                sfromargs = "-" + sfromargs + " ";

            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = toolPath;
            process.StartInfo.Arguments = $"{sfromargs}\"{tempFile}\" \"{outTempFile}\"";
            process.Start();
            process.WaitForExit();

            if (!File.Exists(outTempFile))
                return false;

            rawRomData = File.ReadAllBytes(outTempFile);
            return true;
        }

        static public void EditSFROM(string fileName)
        {
            if (!IsInstalled) return;

            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = toolPath;
            process.StartInfo.Arguments = $"-a \"{fileName}\" \"{fileName}\"";
            process.Start();
            process.WaitForExit();
        }

        static public void ResetSFROM(string fileName)
        {
            if (!IsInstalled) return;

            string sfromargs = "-d";
            if (ConfigIni.UsePCMPatch)
                sfromargs += "p";

            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = toolPath;
            process.StartInfo.Arguments = $"{sfromargs} \"{fileName}\" \"{fileName}\"";
            process.Start();
            process.WaitForExit();
        }

    }
}
