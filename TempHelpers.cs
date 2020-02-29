using System;
using System.IO;

namespace com.clusterrr.hakchi_gui
{
    class TempHelpers
    {
        public static string getUniqueTempPath(string baseDir = null)
        {
            baseDir = baseDir ?? Path.GetTempPath();

            int counter = 0;
            string path = Path.Combine(baseDir, $"hakchi2-ce-temp");

            while (Directory.Exists(path))
            {
                counter++;
                path = Path.Combine(baseDir, $"hakchi2-ce-temp{counter}");
            }

            return path;
        }
        public static void doWithTempFolder(Action<string> func, bool deleteAfter = true)
        {
            var tempFolder = getUniqueTempPath();
            Directory.CreateDirectory(tempFolder);
            func(tempFolder);
            if (deleteAfter)
            {
                try
                {
                    Directory.Delete(tempFolder, true);
                }
                catch (Exception) { }
            }
        }
    }
}
