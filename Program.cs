#pragma warning disable 0618
using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    static class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public static string BaseDirectoryInternal, BaseDirectoryExternal;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            try
            {
                AllocConsole();
                Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            }
            catch { }
            try
            {
                Stream logFile = File.Create("debuglog.txt");
                Debug.Listeners.Add(new TextWriterTraceListener(logFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
            Debug.AutoFlush = true;
#endif
            try
            {
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "hakchi2", out createdNew))
                {
                    if (createdNew)
                    {
                        BaseDirectoryInternal = Path.GetDirectoryName(Application.ExecutablePath);
                        if (ApplicationDeployment.IsNetworkDeployed)
                            BaseDirectoryExternal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "hakchi2");
                        else
                            BaseDirectoryExternal = BaseDirectoryInternal;
                        ConfigIni.Load();
                        try
                        {
                            if (!string.IsNullOrEmpty(ConfigIni.Language))
                                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Language);
                        }
                        catch { }

                        // There are some folders which should be accessed by user
                        // Moving them to "My documents"
                        if (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun)
                        {
                            var externalDirs = new string[]
                        {
                            "art", "folder_images", "patches", "user_mods"
                        };
                            foreach (var dir in externalDirs)
                                DirectoryCopy(Path.Combine(BaseDirectoryInternal, dir), Path.Combine(BaseDirectoryExternal, dir), true);
                        }

                        string languagesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "languages");
                        const string langFileNames = "hakchi.resources.dll";
                        AppDomain.CurrentDomain.AppendPrivatePath(languagesDirectory);
                        // For updates
                        var oldFiles = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), langFileNames, SearchOption.AllDirectories);
                        foreach (var d in oldFiles)
                            if (!d.Contains(@"\languages\"))
                            {
                                var dir = Path.GetDirectoryName(d);
                                Debug.WriteLine("Removing old directory: " + dir);
                                if (ApplicationDeployment.IsNetworkDeployed)
                                {
                                    var targetDir = Path.Combine(languagesDirectory, Path.GetFileName(dir));
                                    Directory.CreateDirectory(targetDir);
                                    var targetFile = Path.Combine(targetDir, langFileNames);
                                    if (File.Exists(targetFile))
                                        File.Delete(targetFile);
                                    File.Move(Path.Combine(dir, langFileNames), targetFile);
                                }
                                else
                                    Directory.Delete(dir, true);
                            }

                        Debug.WriteLine("Starting, version: " + Assembly.GetExecutingAssembly().GetName().Version);
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainForm());
                        Debug.WriteLine("Done.");
                    }
                    else
                    {
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName("hakchi"))
                        {
                            if (process.Id != current.Id)
                            {
                                ShowWindow(process.MainWindowHandle, 9); // Restore
                                SetForegroundWindow(process.MainWindowHandle); // Foreground
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
