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
        public static void doWithTempFolder(Action<string> func, bool deleteAfter = true, string baseDir = null)
        {
            doWithTempFolder<Object>((tempDir) => {
                func(tempDir);
                return null;
            }, deleteAfter, baseDir);
        }
        public static T doWithTempFolder<T>(Func<string, T> func, bool deleteAfter = true, string baseDir = null)
        {
            var tempFolder = getUniqueTempPath(baseDir);
            Directory.CreateDirectory(tempFolder);
            Exception funcError = null;
            T returnValue = default(T);

            try
            {
                returnValue = func(tempFolder);
            } 
            catch (Exception ex)
            {
                funcError = ex;
            }

            if (deleteAfter)
            {
                try
                {
                    Directory.Delete(tempFolder, true);
                }
                catch (Exception) { }
            }

            if (funcError != null) throw funcError;

            return returnValue;
        }
    }
}
