#pragma warning disable 0618
using com.clusterrr.hakchi_gui.Properties;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
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
        public static readonly string BaseDirectoryInternal = Path.GetDirectoryName(Application.ExecutablePath);
        public static string BaseDirectoryExternal;
        public static bool isPortable = false;
        public static List<Stream> debugStreams = new List<Stream>();
        public static MultiFormContext FormContext = new MultiFormContext();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            try
            {
                AllocConsole();
                IntPtr stdHandle = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                FileStream consoleFileStream = new FileStream(safeFileHandle, FileAccess.Write);
                Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                StreamWriter standardOutput = new StreamWriter(consoleFileStream, encoding);
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                debugStreams.Add(consoleFileStream);
                Debug.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
            }
            catch { }
            try
            {
                Stream logFile = File.Create("debuglog.txt");
                debugStreams.Add(logFile);
                Debug.Listeners.Add(new TextWriterTraceListener(logFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }
            Debug.AutoFlush = true;
#else
            Trace.Listeners.Clear();
#endif
#if TRACE
            try
            {
                MemoryStream inMemoryLog = new MemoryStream();
                debugStreams.Add(inMemoryLog);
                Trace.Listeners.Add(new TextWriterTraceListener(new StreamWriter(inMemoryLog, System.Text.Encoding.GetEncoding(MY_CODE_PAGE))));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
            Trace.AutoFlush = true;
#endif
            isPortable = !args.Contains("/nonportable") || args.Contains("/portable");

            if (File.Exists(Path.Combine(BaseDirectoryInternal, "nonportable.flag")))
                isPortable = false;

            bool isFirstRun = false;
            
            if (!isPortable)
            {
                isFirstRun = Shared.isFirstRun();
            }

            try
            {
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "hakchi2", out createdNew))
                {
                    if (createdNew)
                    {
                        if (!isPortable)
                        {
                            // This is not correct way for Windows 7+...
                            BaseDirectoryExternal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "hakchi2");
                            // So if it's not exists, lets try to get documents library path (Win7+)
                            try
                            {
                                if (!Directory.Exists(BaseDirectoryExternal))
                                {
                                    BaseDirectoryExternal = Path.Combine(GetDocumentsLibraryPath(), "hakchi2");
                                }

                                // There are some folders which should be accessed by user
                                // Moving them to "My documents"
                                if (isFirstRun)
                                {
                                    var externalDirs = new string[]
                                        { "art", "folder_images", "patches", "user_mods", "sfrom_tool" };
                                    foreach (var dir in externalDirs)
                                        Shared.DirectoryCopy(Path.Combine(BaseDirectoryInternal, dir), Path.Combine(BaseDirectoryExternal, dir), true, false, true, false);
                                }
                            }
                            catch (Exception ex)
                            {
                                // TODO: Test it on Windows XP
                                Trace.WriteLine(ex.Message);
                            }
                        }
                        else
                            BaseDirectoryExternal = BaseDirectoryInternal;

                        Trace.WriteLine("Base directory: " + BaseDirectoryExternal + " (" + (isPortable ? "portable" : "non-portable") + " mode)");
                        ConfigIni.Load();
                        try
                        {
                            if (!string.IsNullOrEmpty(ConfigIni.Instance.Language))
                                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Instance.Language);
                        }
                        catch { }

                        string languagesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "languages");
                        const string langFileNames = "hakchi.resources.dll";
                        AppDomain.CurrentDomain.AppendPrivatePath(languagesDirectory);
                        // For updates
                        var oldFiles = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), langFileNames, SearchOption.AllDirectories);
                        foreach (var d in oldFiles)
                            if (!d.Contains(Path.DirectorySeparatorChar + "languages" + Path.DirectorySeparatorChar))
                            {
                                var dir = Path.GetDirectoryName(d);
                                Trace.WriteLine("Removing old directory: " + dir);
                                if (!isPortable)
                                {
                                    var targetDir = Path.Combine(languagesDirectory, Path.GetFileName(dir));
                                    Directory.CreateDirectory(targetDir);
                                    var targetFile = Path.Combine(targetDir, langFileNames);
                                    if (File.Exists(targetFile))
                                        File.Delete(targetFile);
                                    File.Copy(Path.Combine(dir, langFileNames), targetFile);
                                }
                                else
                                    Directory.Delete(dir, true);
                            }

                        Trace.WriteLine("Starting, version: " + Shared.AppDisplayVersion);

                        System.Net.ServicePointManager.SecurityProtocol = (System.Net.SecurityProtocolType)4080; // set default security protocol
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        FormContext.AllFormsClosed += Process.GetCurrentProcess().Kill; // Suicide! Just easy and dirty way to kill all threads.

                        FormContext.AddForm(new MainForm());
                        Application.Run(FormContext);
                        Trace.WriteLine("Done.");
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
                Trace.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetCurrentLogContent()
        {
            MemoryStream stream = debugStreams.OfType<MemoryStream>().FirstOrDefault();
            if (stream != default(MemoryStream))
            {
                return Encoding.GetEncoding(MY_CODE_PAGE).GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
            return "";
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

    }
}
