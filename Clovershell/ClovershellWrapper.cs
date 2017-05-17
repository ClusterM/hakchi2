using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;

namespace com.clusterrr.clovershell
{
    class ClovershellWrapper
    {
      
        public GameFolder GetGameFolder(bool folderOnly = false)
        {
            return GetGameFolder("/var/lib/hakchi/rootfs/usr/share/games/nes/kachikachi/", folderOnly);
        }
        public GameFolder GetGameFolderFromFolderCode(string folderCode, bool folderOnly = false)
        {
            return GetGameFolder("/var/lib/hakchi/rootfs/usr/share/games/nes/kachikachi/" + folderCode + "/", folderOnly);
        }
        public GameFolder GetGameFolder(string physicalPath, bool folderOnly = false)
        {
            GameFolder ret = new GameFolder();
            FolderDetail fd = GetFolderDetail(physicalPath);

            for (int x = 0; x < fd.Folders.Count(); x++)
            {

                string s = fd.Folders[x];
                if (s.ToLower().StartsWith("clv"))
                {

                    EntryInfo ei = new EntryInfo();
                    ei.DesktopFilePath = physicalPath + s + "/" + s + ".desktop";
                    if (!folderOnly || s.Contains("-S-"))
                    {
                        string desktopFile = GetFileAsString(physicalPath + s + "/" + s + ".desktop");
                        ei.DesktopFileContent = desktopFile;
                        string[] splitted = desktopFile.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string ss in splitted)
                        {
                            if (ss.StartsWith("Name="))
                            {
                                ei.FullName = ss.Substring(5);
                            }
                            if (ss.StartsWith("Exec="))
                            {
                                ei.Exec = ss.Substring(5);
                            }
                            if (ss.StartsWith("Icon="))
                            {
                                ei.Icon = ss.Substring(5);
                            }
                        }
                        if (ei.Exec.StartsWith("/bin/chmenu"))
                        {
                            ret.subFolders.Add(ei);
                        }
                        else
                        {
                            ret.games.Add(ei);
                        }
                    }
                }
            }

            return ret;
        }
        public class GameFolder
        {
            public List<EntryInfo> subFolders = new List<EntryInfo>();
            public List<EntryInfo> games = new List<EntryInfo>();
        }
        public class EntryInfo
        {
            public string DesktopFileContent;
            public string DesktopFilePath;
            public string Icon;
            public string code;
            public string FullName;
            public string Exec;
            public string getActualPath()
            {
                return Exec.Substring(Exec.IndexOf(" ")).Trim(); ;

            }
        }
        public class FolderDetail
        {
            public string Path;
            public List<string> Folders = new List<string>();
            public List<string> Files = new List<string>();
        }
        public class DiskSpaceInfo
        {
            public decimal Size;
            public decimal Used;
            public decimal Available;
            public decimal PCT;
        }
        public void Disconnect()
        {
            try
            {
                conn.Disconnect();
            }
            catch (Exception exc)
            {

            }
            conn.Dispose();
        }
        public FolderDetail GetFolderDetail(string path)
        {
            FolderDetail ret = new FolderDetail();
            ret.Path = path;

            string command = "ls -al " + path;
       
            var result = conn.ExecuteSimple(command, 25000, true);
           
            string[] allLogs = result.Split(new string[] { "\n" }, StringSplitOptions.None);


            for (int x = 1; x < allLogs.Length; x++)
            {

                string s = allLogs[x];
                string filename = s.Substring(57);
                if (filename != "." && filename != "..")
                {
                    if (s[0] == 'd')
                    {
                        ret.Folders.Add(filename);
                    }
                    else
                    {
                        ret.Files.Add(filename);
                    }
                }
            }

            return ret;


        }
        public DiskSpaceInfo GetDiskSpaceInfo()
        {
            DiskSpaceInfo ret = new DiskSpaceInfo();
            string result = conn.ExecuteSimple("df -h", 5000);
            string[] splitted = result.Split(new string[] { "\n" }, StringSplitOptions.None);
            foreach (string s in splitted)
            {
                if (s.StartsWith("/dev/nandc"))
                {
                    string fs = s.Substring(0, 24).Trim();
                    string size = s.Substring(24, 6).Trim();
                    string used = s.Substring(30, 10).Trim();
                    string available = s.Substring(40, 10).Trim();
                    string pct = s.Substring(50, 6).Trim();
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

                    ret.Size = decimal.Parse(size.Replace("M", ""));
                    ret.Used = decimal.Parse(used.Replace("M", ""));
                    ret.Available = decimal.Parse(available.Replace("M", ""));
                    ret.PCT = decimal.Parse(pct.Replace("%", ""));

                    break;
                }
            }
            return ret;
        }
        public System.Drawing.Bitmap Screenshot()
        {
            var rawStream = new MemoryStream();
            conn.Execute("cat /dev/fb0", null, rawStream, null, 1000, true);

            //  bmp.LockBits(new Rectangle(new Point(0,0),new Size(1280,720)),ImageLockMode.ReadWrite,PixelFormat.Format32bppRgb,BitmapData dt =)
            Bitmap bmp = new Bitmap(1280, 720, 1280 * 4, PixelFormat.Format32bppRgb, GCHandle.Alloc(rawStream.ToArray(), GCHandleType.Pinned).AddrOfPinnedObject());
            rawStream.Dispose();
            return bmp;
        }
        public string[] ExecuteConsoleCommand(string command)
        {
            var result = conn.ExecuteSimple(command, 5000, true);
            string[] allLogs = result.Split(new string[] { "\n" }, StringSplitOptions.None);
            return allLogs;
        }
        public void GetFileInStream(string path, Stream s)
        {
            conn.Execute("cat " + path, null, s, null, 30000, false);

        }
        public void DeleteFile(string path)
        {
            conn.Execute("rm " + path, null, null, null, 10000, false);
        }
        public void UploadFile(string path, Stream s)
        {

            string command = "cat > " + path;
            conn.Execute(command, s, null, null, 30000, true);
        }
        public void UploadFile(string path, string text)
        {
            System.IO.MemoryStream ms2 = new MemoryStream();
            System.IO.TextWriter wri = new System.IO.StreamWriter(ms2);
            wri.Write(text);
            wri.Flush();
            ms2.Seek(0, SeekOrigin.Begin);
            UploadFile(path, ms2);
            ms2.Close();
        }
        public void ShowSplashScreen()
        {
   
            var splashScreenPath = Path.Combine(Path.Combine(com.clusterrr.hakchi_gui.Program.BaseDirectoryInternal, "data"), "splash.gz");
            conn.ExecuteSimple("pkill -KILL clover-mcp");
            conn.ExecuteSimple("pkill -KILL ReedPlayer-Clover");
            conn.ExecuteSimple("pkill -KILL kachikachi");
            if (File.Exists(splashScreenPath))
            {
                using (var splash = new FileStream(splashScreenPath, FileMode.Open))
                {
                    conn.Execute("gunzip -c - > /dev/fb0", splash, null, null, 3000);
                }
            }
        }
        public void UploadFile(string path, byte[] data)
        {
            System.IO.MemoryStream mstr = new MemoryStream();
            mstr.Write(data, 0, data.Length);
            mstr.Seek(0, SeekOrigin.Begin);
            UploadFile(path, mstr);
            mstr.Close();
        }
        public byte[] GetFile(string path)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            conn.Execute("cat " + path, null, ms, null, 30000, false);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] ret = ms.ToArray();
            ms.Close();
            return ret;

        }
        public string GetFileAsString(string path)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            conn.Execute("cat " + path, null, ms, null, 30000, false);
            ms.Seek(0, SeekOrigin.Begin);
            System.IO.TextReader rdr = new System.IO.StreamReader(ms);
            string file = rdr.ReadToEnd();




            rdr.Close();
            return file;
        }
        public bool IsOnline()
        {
            return conn.IsOnline;
        }
        public void Reboot()
        {
            try
            {
                conn.ExecuteSimple("reboot", 100);
            }
            catch(Exception exc)
            {

            }
        }
        private ClovershellConnection conn;
       
        public ClovershellWrapper(ClovershellConnection theConnection)
        {
            conn = theConnection;
            conn.OnConnected += Conn_OnConnected;
        }
        public event ClovershellConnection.OnClovershellConnected OnConnected;
        private void Conn_OnConnected()
        {
            if (OnConnected != null)
            {
                OnConnected();
            }
        }
    }
}
