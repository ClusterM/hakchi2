using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SevenZip;
using System.Windows.Forms;
using System.Diagnostics;
namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
    public class ImportGames : TaskableTool
    {
        public List<NesMiniApplication> addedApplications;
        private IEnumerable<string> _GamesToAdd;
        public ImportGames(IEnumerable<string> GamesToAdd) : base("Importing games")
        {
            _GamesToAdd = GamesToAdd;
        }
        bool YesForAllPatches;
        public override void Execute()
        {

            ReportProgress(0);
            try
            {

                var apps = new List<NesMiniApplication>();
                 addedApplications = null;
                //bool NoForAllUnsupportedMappers = false;
                bool YesForAllUnsupportedMappers = false;
                 YesForAllPatches = false;
                int count = 0;
                ReportStatus(Properties.Resources.AddingGames);
                foreach (var sourceFileName in _GamesToAdd)
                {
                    NesMiniApplication app = null;
                    try
                    {
                        var fileName = sourceFileName;
                        var ext = Path.GetExtension(sourceFileName).ToLower();
                        bool? needPatch = YesForAllPatches ? (bool?)true : null;
                        byte[] rawData = null;
                        string tmp = null;
                        if (ext == ".7z" || ext == ".zip" || ext == ".rar")
                        {
                            SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                            using (var szExtractor = new SevenZipExtractor(sourceFileName))
                            {
                                var filesInArchive = new List<string>();
                                var gameFilesInArchive = new List<string>();
                                foreach (var f in szExtractor.ArchiveFileNames)
                                {
                                    var e = Path.GetExtension(f).ToLower();
                                    if (e == ".desktop" || Manager.EmulatorManager.getInstance().isFileValidRom(e))
                                        gameFilesInArchive.Add(f);
                                    filesInArchive.Add(f);
                                }
                                if (gameFilesInArchive.Count == 1) // Only one NES file (or app)
                                {
                                    fileName = gameFilesInArchive[0];
                                }
                                else if (gameFilesInArchive.Count > 1) // Many NES files, need to select
                                {
                                    var r = SelectFile(gameFilesInArchive.ToArray());
                                    if (r == DialogResult.OK)
                                        fileName = selectedFile;
                                    else if (r == DialogResult.Ignore)
                                        fileName = sourceFileName;
                                    else continue;
                                }
                                else if (filesInArchive.Count == 1) // No NES files but only one another file
                                {
                                    fileName = filesInArchive[0];
                                }
                                else // Need to select
                                {
                                    var r = SelectFile(filesInArchive.ToArray());
                                    if (r == DialogResult.OK)
                                        fileName = selectedFile;
                                    else if (r == DialogResult.Ignore)
                                        fileName = sourceFileName;
                                    else continue;
                                }
                                if (fileName != sourceFileName)
                                {
                                    var o = new MemoryStream();
                                    if (Path.GetExtension(fileName).ToLower() == ".desktop" // App in archive, need the whole directory
                                        || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".jpg") // Or it has cover in archive
                                        || szExtractor.ArchiveFileNames.Contains(Path.GetFileNameWithoutExtension(fileName) + ".png"))
                                    {
                                        tmp = Path.Combine(Path.GetTempPath(), fileName);
                                        Directory.CreateDirectory(tmp);
                                        szExtractor.ExtractArchive(tmp);
                                        fileName = Path.Combine(tmp, fileName);
                                    }
                                    else
                                    {
                                        szExtractor.ExtractFile(fileName, o);
                                        rawData = new byte[o.Length];
                                        o.Seek(0, SeekOrigin.Begin);
                                        o.Read(rawData, 0, (int)o.Length);
                                    }
                                }
                            }
                        }
                        if (Path.GetExtension(fileName).ToLower() == ".nes")
                        {
                            try
                            {
                                app = NesGame.Import(fileName, sourceFileName, YesForAllUnsupportedMappers ? (bool?)true : null, ref needPatch, needPatchCallback, null, rawData);

                                // Trying to import Game Genie codes
                                var lGameGeniePath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".xml");
                                if (File.Exists(lGameGeniePath))
                                {
                                    GameGenieDataBase lGameGenieDataBase = new GameGenieDataBase(app);
                                    lGameGenieDataBase.ImportCodes(lGameGeniePath, true);
                                    lGameGenieDataBase.Save();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex is UnsupportedMapperException || ex is UnsupportedFourScreenException)
                                {
                                    var r = MessageBoxFromThread(
                                        (ex is UnsupportedMapperException)
                                           ? string.Format(Properties.Resources.MapperNotSupported, Path.GetFileName(fileName), (ex as UnsupportedMapperException).ROM.Mapper)
                                           : string.Format(Properties.Resources.FourScreenNotSupported, Path.GetFileName(fileName)),
                                        Properties.Resources.AreYouSure,
                                        _GamesToAdd.Count() <= 1 ? MessageBoxButtons.YesNo : MessageBoxButtons.AbortRetryIgnore,
                                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, true);
                                    if (r == DialogResult.Abort)
                                        YesForAllUnsupportedMappers = true;
                                    if (r == DialogResult.Yes || r == DialogResult.Abort || r == DialogResult.Retry)
                                        app = NesGame.Import(fileName, sourceFileName, true, ref needPatch, needPatchCallback, null, rawData);
                                    else
                                        continue;
                                }
                                else throw ex;
                            }
                        }
                        else
                        {
                            app = NesMiniApplication.Import(fileName, sourceFileName, rawData);
                        }
                        if (!string.IsNullOrEmpty(tmp) && Directory.Exists(tmp)) Directory.Delete(tmp, true);
                        ConfigIni.SelectedGames += ";" + app.Code;
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.Threading.ThreadAbortException)
                        {
                            ReportCompleted();
                          
                        }
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        ReportError(ex.Message,false);
                    }
                    if (app != null)
                        apps.Add(app);
                    ReportProgress((++count*100/ _GamesToAdd.Count()));
                }
                addedApplications = apps;
              
                ReportCompleted();

            }
            catch (Exception exc)
            {

                ReportError(exc.Message, true);
            }


        }
        private delegate DialogResult MessageBoxFromThreadDelegate( string text, string caption, MessageBoxButtons buttons,
         MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak);
        DialogResult MessageBoxFromThread(string text, string caption, MessageBoxButtons buttons,
            MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, bool tweak)
        {
          
            if (tweak) MessageBoxManager.Register(); // Tweak button names
            var result = MessageBox.Show( text, caption, buttons, icon, defaultButton);
            if (tweak) MessageBoxManager.Unregister();
           
            return result;
        }
        private bool needPatchCallback(Form parentForm, string nesFileName)
        {
            if (_GamesToAdd == null || _GamesToAdd.Count() <= 1)
            {
                return MessageBoxFromThread(
                    string.Format(Properties.Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Properties.Resources.PatchAvailable,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1, false) == DialogResult.Yes;
            }
            else
            {
                var r = MessageBoxFromThread(
                    string.Format(Properties.Resources.PatchQ, Path.GetFileName(nesFileName)),
                    Properties.Resources.PatchAvailable,
                    MessageBoxButtons.AbortRetryIgnore,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2, true);
                if (r == DialogResult.Abort)
                    YesForAllPatches = true;
                return r != DialogResult.Ignore;
            }
        }
        string selectedFile = null;
        DialogResult SelectFile(string[] files)
        {
          
            var form = new SelectFileForm(files);
       
            var result = form.ShowDialog();
        
            if (form.listBoxFiles.SelectedItem != null)
                selectedFile = form.listBoxFiles.SelectedItem.ToString();
            else
                selectedFile = null;
           
            return result;
        }
    }
}
