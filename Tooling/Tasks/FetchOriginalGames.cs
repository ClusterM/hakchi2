using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
    class FetchOriginalGames:TaskableTool
    {
        clovershell.ClovershellWrapper wrapper;
        public FetchOriginalGames(clovershell.ClovershellConnection conn):base("Fetching orignal games")
        {
            wrapper = new clovershell.ClovershellWrapper(conn);
        }
        public override void Execute()
        {
           
            ReportProgress(0);
            try
            {
                ReportStatus("Unmounting current filesystem");
                try
                {
                    wrapper.ExecuteConsoleCommand("umount /usr/share/games/nes/kachikachi/");
                }
                catch(Exception exc)
                {
                    //Will fail if already unmounted so not much issue here.
                }
                ReportStatus("Fetching game listing from NES Classic");
                clovershell.ClovershellWrapper.FolderDetail folders= wrapper.GetFolderDetail("/usr/share/games/nes/kachikachi/");
                for(int x = 0;x<folders.Folders.Count;x++)
                {
                    ReportStatus("Fetching " + folders.Folders[x]);
                    clovershell.ClovershellWrapper.FolderDetail gamefolder = wrapper.GetFolderDetail("/usr/share/games/nes/kachikachi/" + folders.Folders[x] +"/");
                    foreach(string f in gamefolder.Files)
                    {
                        string localPath = System.IO.Path.Combine(NesMiniApplication.GamesDirectory, folders.Folders[x]+"\\"+ f);
                        if(localPath.Contains("\\PRODUCTION-TESTS\\"))
                        {
                            localPath = localPath.Replace("\\PRODUCTION-TESTS\\", "\\CLV-P-AAAAA\\");
                        }
                        if(localPath.Contains("\\PRODUCTION-TESTS.desktop"))
                        {
                            localPath = localPath.Replace("\\PRODUCTION-TESTS.desktop", "\\CLV-P-AAAAA.desktop");
                        }
                        if(System.IO.File.Exists(localPath))
                        {
                            System.IO.File.Delete(localPath);
                        }
                        if(!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(localPath)))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(localPath));
                        }
                        System.IO.File.WriteAllBytes(localPath, wrapper.GetFile("/usr/share/games/nes/kachikachi/" + folders.Folders[x] + "/" + f));
                    }
                    ReportProgress(((x + 1) * 100) / folders.Folders.Count);
                    Console.Write("");
                }
                ReportStatus("Rebooting");
                wrapper.Reboot();
                ReportStatus("Reloading games");
                Manager.GameManager.GetInstance().LoadLibrary();
                ReportCompleted();

            }
            catch(Exception exc)
            {
                ReportStatus("Rebooting");
                wrapper.Reboot();
                ReportError(exc.Message, true);
            }

        
        }
    }
}
