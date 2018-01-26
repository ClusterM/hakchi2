#pragma warning disable 0618
using com.clusterrr.hakchi_gui.Properties;
using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        private const int MY_CODE_PAGE = 437;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint OPEN_EXISTING = 0x3;
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
                IntPtr stdHandle = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
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
                        {
                            // This is not correct way for Windows 7+...
                            BaseDirectoryExternal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "hakchi2");
                            // So if it's not exists, lets try to get documents library path (Win7+)
                            try
                            {
                                if (!Directory.Exists(BaseDirectoryExternal))
                                    BaseDirectoryExternal = Path.Combine(GetDocumentsLibraryPath(), "hakchi2");
                            }
                            catch (Exception ex)
                            {
                                // TODO: Test it on Windows XP
                                Debug.WriteLine(ex.Message);
                            }
                        }
                        else
                            BaseDirectoryExternal = BaseDirectoryInternal;
                        Debug.WriteLine("Base directory: " + BaseDirectoryExternal);
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
                            if (!d.Contains(Path.DirectorySeparatorChar + "languages" + Path.DirectorySeparatorChar))
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
                        if (Resources.gitCommit.Length > 0)
                        {
                            Debug.WriteLine("git commit: " + Resources.gitCommit);
                        }
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

        [DllImport("Shell32.dll")]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags,
            IntPtr hToken, out IntPtr ppszPath);
        private static string GetDocumentsLibraryPath()
        {
            IntPtr outPath;
            var documentsLibraryGuid = new Guid("7B0DB17D-9CD2-4A93-9733-46CC89022E7C");
            int result = SHGetKnownFolderPath(documentsLibraryGuid, 0, WindowsIdentity.GetCurrent().Token, out outPath);
            if (result >= 0)
            {
                var libConfigPath = Marshal.PtrToStringUni(outPath);
                var libConfig = new XmlDocument();
                libConfig.LoadXml(File.ReadAllText(libConfigPath));
                var nsmgr = new XmlNamespaceManager(libConfig.NameTable);
                nsmgr.AddNamespace("ns", libConfig.LastChild.NamespaceURI);
                var docs = libConfig.SelectSingleNode("//ns:searchConnectorDescription[ns:isDefaultSaveLocation='true']/ns:simpleLocation/ns:url/text()", nsmgr);
                if (Directory.Exists(docs.Value))
                    return docs.Value;
                else
                    throw new Exception("Invalid Documents directory: " + docs.Value);
            }
            else
            {
                throw new ExternalException("Cannot get the known folder path. It may not be available on this system.",
                    result);
            }
        }

        // concatenate an arbitrary number of arrays
        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        // TODO: needs work to show a dialog when directory can't be deleted
        public static void PersistentDeleteDirectory(string dir, bool excludeSelf = false, uint retries = 10, bool recursing = false) // recurse is internal parameter
        {
            if (!Directory.Exists(dir))
                return;

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            string[] dirs = Directory.GetDirectories(dir);
            foreach (string d in dirs)
                PersistentDeleteDirectory(d, false, retries, true);

            if (!excludeSelf)
            {
                bool deleted = false;
                uint r = retries;
                while (r > 0)
                {
                    try
                    {
                        Directory.Delete(dir, false);
                        deleted = true;
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(0);
                        --r;
                    }
                }
                if (!deleted)
                    throw new IOException($"Could not delete directory \"{dir}\".");

                if (!recursing)
                {
                    while (retries > 0)
                    {
                        if (!Directory.Exists(dir)) // success
                            return;
                        Thread.Sleep(0);
                        --retries;
                    }
                    throw new IOException($"Could not confirm directory \"{dir}\" was deleted.");
                }
            }
        }

        // workaround to prevent flickering with ListView controls
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

    }
}
