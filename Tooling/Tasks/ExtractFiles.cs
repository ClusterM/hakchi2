using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
  
    class ExtractFiles : TaskableTool
    {

        Dictionary<string, string> _filesToDir;
        public ExtractFiles(Dictionary<string,string> filesToDirectory) : base("Extracting roms")
        {
            _filesToDir = filesToDirectory;
        }
        public override void Execute()
        {
            SevenZip.SevenZipExtractor.SetLibraryPath(System.IO.Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
            int done = 0;
            foreach (string f in _filesToDir.Keys)
            {
                using (var szExtractor = new SevenZip.SevenZipExtractor(f))
                {

                    ReportStatus("Extracting " + f);
                    try
                    {
                        System.IO.Directory.CreateDirectory(_filesToDir[f]);
                        szExtractor.ExtractArchive(_filesToDir[f]);
                        done++;
                    }
                    catch(Exception exc)
                    {
                        ReportError("Cannot extract " + f + "\r\n" + exc.Message, false);
                    }
                    ReportProgress(done * 100 / _filesToDir.Count);

                }
            }
            ReportCompleted();
        }
           
    }
}
