using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.util;
using com.clusterrr.util.arxoneftp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class SyncTask
    {
        readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "hakchi2");

        public NesMenuCollection Games
        {
            get; set;
        }

        public bool ShowFoldersManager(Tasker tasker, NesMenuCollection collection)
        {
            if (tasker.HostForm.Disposing) return false;
            if (tasker.HostForm.InvokeRequired)
            {
                return (bool)tasker.HostForm.Invoke(new Func<Tasker, NesMenuCollection, bool>(ShowFoldersManager), new object[] { tasker, collection });
            }
            try
            {
                using (FoldersManagerForm form = new FoldersManagerForm(collection, MainForm.StaticRef))
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = form.ShowDialog() == DialogResult.OK;
                    tasker.PopState();
                    return result;
                }
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool ShowExportDialog(Tasker tasker)
        {
            if (tasker.HostForm.Disposing) return false;
            if (tasker.HostForm.InvokeRequired)
            {
                return (bool)tasker.HostForm.Invoke(new Func<Tasker, bool>(ShowExportDialog), new object[] { tasker });
            }
            try
            {
                using (ExportGamesDialog driveSelectDialog = new ExportGamesDialog())
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = driveSelectDialog.ShowDialog() == DialogResult.OK;
                    tasker.PopState();
                    if (!result)
                        return false;
                    this.exportLinked = driveSelectDialog.LinkedExport;
                    this.exportDirectory = driveSelectDialog.ExportPath;
                    if (!Directory.Exists(driveSelectDialog.ExportPath))
                        Directory.CreateDirectory(driveSelectDialog.ExportPath);
                }
                return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        public bool ShowUploadDialog(Tasker tasker)
        {
            if (tasker.HostForm.Disposing) return false;
            if (tasker.HostForm.InvokeRequired)
            {
                return (bool)tasker.HostForm.Invoke(new Func<Tasker, bool>(ShowUploadDialog), new object[] { tasker });
            }
            try
            {
                using (UploadGamesDialog uploadGamesDialog = new UploadGamesDialog())
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = uploadGamesDialog.ShowDialog() == DialogResult.OK;
                    tasker.PopState();
                    if (!result)
                        return false;
                    // this.uploadPath = hakchi.GetRemoteGameSyncPath(ConfigIni.Instance.ConsoleType);
                }
                return true;
            }
            catch (InvalidOperationException) { }
            return false;
        }

        private class GamesTreeStats
        {
            public List<NesMenuCollection> allMenus = new List<NesMenuCollection>();
            public int TotalGames = 0;
            public long TotalSize = 0;
            public long TransferSize = 0;
        }

        private string exportDirectory;
        private bool exportLinked;

        public SyncTask()
        {
            Games = new NesMenuCollection();
        }

        public Tasker.Conclusion ExportGames(Tasker tasker, Object syncObject = null)
        {
            int maxProgress = 100;
            tasker.SetTitle(Resources.ExportGames);
            tasker.SetStatusImage(Resources.sign_up);
            tasker.SetProgress(0, maxProgress, Tasker.State.Starting, Resources.SelectDrive);
            if (Games == null || Games.Count == 0)
                throw new Exception("No games to upload");

            // select drive
            exportLinked = false;
            exportDirectory = string.Empty;
            if (!ShowExportDialog(tasker))
                return Tasker.Conclusion.Abort;

            // building folders
            tasker.SetProgress(5, maxProgress, Tasker.State.Starting, Resources.BuildingMenu);
            if (ConfigIni.Instance.FoldersMode == NesMenuCollection.SplitStyle.Custom)
            {
                if (!ShowFoldersManager(tasker, Games))
                    return Tasker.Conclusion.Abort;
                Games.AddBack();
            }
            else
                Games.Split(ConfigIni.Instance.FoldersMode, ConfigIni.Instance.MaxGamesPerFolder);

            // generate menus and game files ready to be uploaded
            tasker.SetStatus(Resources.AddingGames);
            Dictionary<string, string> originalGames = new Dictionary<string, string>();
            var localGameSet = new HashSet<ApplicationFileInfo>();
            var stats = new GamesTreeStats();
            AddMenu(
                Games,
                originalGames,
                exportLinked ? NesApplication.CopyMode.LinkedExport : NesApplication.CopyMode.Export,
                localGameSet,
                stats);
            tasker.SetProgress(15, maxProgress);

            // check free space
            tasker.SetStatus(Resources.CalculatingDiff);
            var drive = new DriveInfo(Path.GetPathRoot(exportDirectory));
            long storageTotal = drive.TotalSize;
            long storageUsed = Shared.DirectorySize(exportDirectory);
            long storageFree = drive.AvailableFreeSpace;
            long maxGamesSize = storageUsed + storageFree;
            Debug.WriteLine($"Exporting to folder: {exportDirectory}");
            Debug.WriteLine($"Drive: {drive.Name} ({drive.DriveFormat})");
            Debug.WriteLine(string.Format("Storage size: {0:F1}MB, used by games: {1:F1}MB, free: {2:F1}MB", storageTotal / 1024.0 / 1024.0, storageUsed / 1024.0 / 1024.0, storageFree / 1024.0 / 1024.0));
            Debug.WriteLine(string.Format("Available for games: {0:F1}MB", maxGamesSize / 1024.0 / 1024.0));
            if (stats.TotalSize > maxGamesSize)
            {
                throw new Exception(
                    string.Format(Resources.MemoryFull, Shared.SizeSuffix(stats.TotalSize)) + "\r\n" +
                    string.Format(Resources.MemoryStatsExport, Shared.SizeSuffix(maxGamesSize)));
            }

            // list current files on drive
            var exportDriveGameSet = ApplicationFileInfo.GetApplicationFileInfoForDirectory(exportDirectory);

            // calculating diff
            var exportDriveGamesToDelete = exportDriveGameSet.Except(localGameSet);
            var localGamesToTransfer = localGameSet.Except(exportDriveGameSet);

            // delete any files on the device that aren't present in current layout
            tasker.SetStatus(Resources.CleaningUp);
            DeleteLocalApplicationFilesFromDirectory(exportDriveGamesToDelete, exportDirectory);

            // now transfer whatever games are remaining
            Debug.WriteLine("Exporting games: " + Shared.SizeSuffix(stats.TotalSize));
            int i = 25;
            maxProgress = 25 + localGamesToTransfer.Count();
            tasker.SetProgress(25, maxProgress, Tasker.State.Running, Resources.CopyingGames);
            foreach (var afi in localGamesToTransfer)
            {
                string path = new Uri(exportDirectory + "/" + afi.FilePath).LocalPath;
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!string.IsNullOrEmpty(afi.LocalFilePath))
                {
                    File.Copy(afi.LocalFilePath, path, true);
                }
                else
                {
                    if (afi.FileStream == null || !afi.FileStream.CanRead)
                    {
                        Debug.WriteLine($"\"{afi.FilePath}\": no source data or stream or unreadable");
                    }
                    else
                    {
                        afi.FileStream.Position = 0;
                        using (var f = File.Open(path, FileMode.Create))
                            afi.FileStream.CopyTo(f);
                        File.SetLastWriteTimeUtc(path, afi.ModifiedTime);
                    }
                }
                tasker.SetProgress(++i, maxProgress);
            }

#if DEBUG
            using (var gamesTar = new TarStream(localGamesToTransfer, "."))
            {
                if (gamesTar.Length > 0)
                {
                    Debug.WriteLine("Creating debug tar archive: " + Shared.SizeSuffix(gamesTar.Length));
                    File.Delete(Path.Combine(Program.BaseDirectoryExternal, "DebugSyncOutput.tar"));
                    gamesTar.CopyTo(File.OpenWrite(Path.Combine(Program.BaseDirectoryExternal, "DebugSyncOutput.tar")));
                }
            }
#endif

            // show resulting games directory
            tasker.SetStatus(Resources.PleaseWait);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = exportDirectory
                }
            };
            process.Start();

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion UploadGames(Tasker tasker, Object syncObject = null)
        {
            int maxProgress = 135;
            tasker.SetTitle(Resources.UploadGames);
            tasker.SetState(Tasker.State.Starting);
            tasker.SetStatusImage(Resources.sign_up);

            if (!hakchi.Shell.IsOnline)
            {
                return Tasker.Conclusion.Error;
            }

            if (Games == null || Games.Count == 0)
                throw new Exception("No games to upload");

            // building folders
            tasker.SetStatus(Resources.BuildingMenu);
            if (ConfigIni.Instance.FoldersMode == NesMenuCollection.SplitStyle.Custom)
            {
                if (!ShowFoldersManager(tasker, Games))
                    return Tasker.Conclusion.Abort;
                Games.AddBack();
            }
            else
                Games.Split(ConfigIni.Instance.FoldersMode, ConfigIni.Instance.MaxGamesPerFolder);

            // prepare transfer
            var shell = hakchi.Shell;
            try
            {
                hakchi.ShowSplashScreen();
                shell.ExecuteSimple("hakchi eval 'umount \"$gamepath\"'");

                // clean up previous directories (separate game storage vs not)
                string uploadPath = "";
                if (ConfigIni.Instance.UploadToTmp)
                {
                    uploadPath = "/tmp/upload-test";
                }
                else
                {
                    uploadPath = hakchi.GetRemoteGameSyncPath(ConfigIni.Instance.ConsoleType);
                    tasker.SetStatus(Resources.CleaningUp);
                    shell.ExecuteSimple("find \"" + hakchi.RemoteGameSyncPath + "/\" -maxdepth 1 | tail -n +2 " + 
                        "| grep -" + (ConfigIni.Instance.SeparateGameStorage ? "v" : "") + "Ee '(/snes(-usa|-eur|-jpn)?|/nes(-usa|-jpn)?|/)$' " + 
                        "| while read f; do rm -rf \"$f\"; done", 0, true);
                }
                tasker.SetProgress(5, maxProgress);

                // generate menus and game files ready to be uploaded
                tasker.SetStatus(Resources.AddingGames);
                Dictionary<string, string> originalGames = new Dictionary<string, string>();
                var localGameSet = new HashSet<ApplicationFileInfo>();
                var stats = new GamesTreeStats();
                AddMenu(
                    Games,
                    originalGames,
                    ConfigIni.Instance.SyncLinked ? NesApplication.CopyMode.LinkedSync : NesApplication.CopyMode.Sync,
                    localGameSet,
                    stats);
                tasker.SetProgress(15, maxProgress);

                // calculating size constraints
                tasker.SetStatus(Resources.CalculatingDiff);
                MemoryStats.Refresh();
                if (stats.TotalSize > MemoryStats.AvailableForGames())
                {
                    throw new OutOfMemoryException(string.Format(Resources.MemoryFull, Shared.SizeSuffix(stats.TotalSize)) + "\r\n" +
                        string.Format(Resources.MemoryStats.Replace("|", "\r\n"),
                        MemoryStats.StorageTotal / 1024.0 / 1024.0,
                        MemoryStats.AvailableForGames() / 1024.0 / 1024.0,
                        MemoryStats.SaveStatesSize / 1024.0 / 1024.0,
                        (MemoryStats.StorageUsed - MemoryStats.AllGamesSize - MemoryStats.SaveStatesSize) / 1024.0 / 1024.0));
                }

                // get the remote list of files, timestamps, and sizes
                string gamesOnDevice = shell.ExecuteSimple($"mkdir -p \"{uploadPath}\"; cd \"{uploadPath}\"; find . -type f -exec sh -c \"stat \\\"{{}}\\\" -c \\\"%n %s %y\\\"\" \\;", 0, true);
                var remoteGameSet = ApplicationFileInfo.GetApplicationFileInfoFromConsoleOutput(gamesOnDevice);

                // delete any remote files that aren't present locally
                tasker.SetStatus(Resources.CleaningUp);
                var remoteGamesToDelete = remoteGameSet.Except(localGameSet);
                DeleteRemoteApplicationFiles(remoteGamesToDelete, uploadPath);

                // only keep the local files that aren't matching on the mini
                var localGamesToUpload = localGameSet.Except(remoteGameSet);

                // now transfer whatever games are remaining
                tasker.SetProgress(20, maxProgress, Tasker.State.Running, Resources.UploadingGames);
                bool uploadSuccessful = false;
                if (!localGamesToUpload.Any())
                {
                    Debug.WriteLine("No file to upload");
                    uploadSuccessful = true;
                }
                else if (ConfigIni.Instance.ForceSSHTransfers || hakchi.Shell is clovershell.ClovershellConnection) // use tar stream when detecting clovershell
                {
                    Debug.WriteLine("Uploading through tar file");
                    using (var gamesTar = new TarStream(localGamesToUpload, "."))
                    {
                        Debug.WriteLine($"Upload size: " + Shared.SizeSuffix(gamesTar.Length));
                        if (gamesTar.Length > 0)
                        {
                            DateTime
                                startTime = DateTime.Now,
                                lastTime = DateTime.Now;
                            bool done = false;
                            gamesTar.OnReadProgress += delegate (long pos, long len)
                            {
                                if (done) return;
                                if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 250)
                                {
                                    tasker.SetProgress(20 + (int)((double)pos / len * 100), maxProgress);
                                    lastTime = DateTime.Now;
                                }
                            };
                            shell.Execute($"tar -xvC \"{uploadPath}\"", gamesTar, null, null, 0, true);
                            Debug.WriteLine("Uploaded " + (int)(gamesTar.Length / 1024) + "kb in " + DateTime.Now.Subtract(startTime).TotalSeconds + " seconds");

                            tasker.SetState(Tasker.State.Finishing);
                            uploadSuccessful = true;
                            done = true;
#if DEBUG
                            File.Delete(Program.BaseDirectoryExternal + "\\DebugSyncOutput.tar");
                            gamesTar.Position = 0;
                            gamesTar.CopyTo(File.OpenWrite(Program.BaseDirectoryExternal + "\\DebugSyncOutput.tar"));
#endif
                        }
                    }
                }
                else if (hakchi.Shell is INetworkShell) // using ftp when detecting network
                {
                    Debug.WriteLine("Uploading through FTP");
                    tasker.SetProgress(20, maxProgress, Tasker.State.Running, Resources.UploadingGames);
                    using (var ftp = new FtpWrapper(localGamesToUpload))
                    {
                        Debug.WriteLine($"Upload size: " + Shared.SizeSuffix(ftp.Length));
                        if (ftp.Length > 0)
                        {
                            DateTime startTime = DateTime.Now,
                                lastTime = DateTime.Now;
                            ftp.OnReadProgress += delegate (long pos, long len, string filename)
                            {
                                if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= 100)
                                {
                                    tasker.SetProgress(20 + (int)((double)pos / len * 100), maxProgress);
                                    lastTime = DateTime.Now;
                                }
                            };
                            if (ftp.Connect(hakchi.STATIC_IP, 21, hakchi.USERNAME, hakchi.PASSWORD))
                            {
                                ftp.Upload(uploadPath);
                                uploadSuccessful = true;
                                Debug.WriteLine("Uploaded " + (int)(ftp.Length / 1024) + "kb in " + DateTime.Now.Subtract(startTime).TotalSeconds + " seconds");
                            }
                            tasker.SetState(Tasker.State.Finishing);
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("Unknown shell detected, aborting");
                }

                // don't continue if upload wasn't successful
                if (!uploadSuccessful)
                {
                    Debug.WriteLine("Something happened during transfer, cancelling");
                    return Tasker.Conclusion.Error;
                }

                // Finally, delete any empty directories we may have left during the differential sync
                tasker.SetStatus(Resources.CleaningUp);
                shell.ExecuteSimple($"for f in $(find \"{uploadPath}\" -type d -mindepth 1 -maxdepth 2); do {{ ls -1 \"$f\" | grep -v pixelart | grep -v autoplay " +
                    "| wc -l | { read wc; test $wc -eq 0 && rm -rf \"$f\"; } } ; done", 0);
                tasker.SetProgress(125, maxProgress);

                tasker.SetStatus(Resources.UploadingOriginalGames);
                int i = 0;
                foreach (var originalCode in originalGames.Keys)
                {
                    string originalSyncCode = "";
                    switch (ConfigIni.Instance.ConsoleType)
                    {
                        case hakchi.ConsoleType.NES:
                        case hakchi.ConsoleType.Famicom:
                            originalSyncCode =
                                $"src=\"{hakchi.SquashFsPath}{hakchi.GamesPath}/{originalCode}\" && " +
                                $"dst=\"{uploadPath}/{originalGames[originalCode]}/{originalCode}\" && " +
                                $"mkdir -p \"$dst\" && " +
                                $"([ -L \"$dst/autoplay\" ] || ln -s \"$src/autoplay\" \"$dst/\") && " +
                                $"([ -L \"$dst/pixelart\" ] || ln -s \"$src/pixelart\" \"$dst/\")";
                            break;
                        case hakchi.ConsoleType.SNES_EUR:
                        case hakchi.ConsoleType.SNES_USA:
                        case hakchi.ConsoleType.SuperFamicom:
                            originalSyncCode =
                                $"src=\"{hakchi.SquashFsPath}{hakchi.GamesPath}/{originalCode}\" && " +
                                $"dst=\"{uploadPath}/{originalGames[originalCode]}/{originalCode}\" && " +
                                $"mkdir -p \"$dst\" && " +
                                $"([ -L \"$dst/autoplay\" ] || ln -s \"$src/autoplay\" \"$dst/\")";
                            break;
                    }
                    shell.ExecuteSimple(originalSyncCode, 5000, true);
                    tasker.SetProgress(125 + (int)((double)++i / originalGames.Count * 10), maxProgress);
                };

                tasker.SetStatus(Resources.UploadingConfig);
                hakchi.SyncConfig(ConfigIni.GetConfigDictionary());
            }
            finally
            {
                try
                {
                    if (shell.IsOnline)
                    {
                        shell.ExecuteSimple("hakchi overmount_games; uistart", 2000, true);
                        MemoryStats.Refresh();
                    }
                }
                catch { }
            }

#if !DEBUG
            try
            {
                Directory.Delete(tempDirectory, true);
            }
            catch { }
#endif

            return Tasker.Conclusion.Success;
        }

        private void AddMenu(NesMenuCollection menuCollection, Dictionary<string, string> originalGames, NesApplication.CopyMode copyMode, HashSet<ApplicationFileInfo> localGameSet = null, GamesTreeStats stats = null)
        {
            if (stats == null)
                stats = new GamesTreeStats();
            if (!stats.allMenus.Contains(menuCollection))
                stats.allMenus.Add(menuCollection);
            int menuIndex = stats.allMenus.IndexOf(menuCollection);
            string targetDirectory = string.Format("{0:D3}", menuIndex);

            foreach (var element in menuCollection)
            {
                if (element is NesApplication)
                {
                    var game = element as NesApplication;
                    
                    // still use temp directory for game genie games
                    try
                    {
                        if (game is ISupportsGameGenie && File.Exists(game.GameGeniePath))
                        {
                            string tempPath = Path.Combine(tempDirectory, game.Desktop.Code);
                            Shared.EnsureEmptyDirectory(tempPath);
                            NesApplication gameCopy = game.CopyTo(tempDirectory);
                            (gameCopy as ISupportsGameGenie).ApplyGameGenie();
                            game = gameCopy;
                        }
                    }
                    catch (GameGenieFormatException ex)
                    {
                        Debug.WriteLine(string.Format(Resources.GameGenieFormatError, ex.Code, game.Name));
                    }
                    catch (GameGenieNotFoundException ex)
                    {
                        Debug.WriteLine(string.Format(Resources.GameGenieNotFound, ex.Code, game.Name));
                    }

                    long gameSize = game.Size();
                    Debug.WriteLine(string.Format("Processing {0} ('{1}'), size: {2}KB", game.Code, game.Name, gameSize / 1024));
                    gameSize = game.CopyTo(targetDirectory, localGameSet, copyMode);
                    stats.TotalSize += gameSize;
                    stats.TransferSize += gameSize;
                    stats.TotalGames++;

                    // legacy
                    if (game.IsOriginalGame)
                        originalGames[game.Code] = $"{menuIndex:D3}";
                }
                if (element is NesMenuFolder)
                {
                    var folder = element as NesMenuFolder;
                    if (folder.Name == Resources.FolderNameTrashBin)
                        continue; // skip recycle bin!

                    if (!stats.allMenus.Contains(folder.ChildMenuCollection))
                    {
                        stats.allMenus.Add(folder.ChildMenuCollection);
                        AddMenu(folder.ChildMenuCollection, originalGames, copyMode, localGameSet, stats);
                    }
                    folder.ChildIndex = stats.allMenus.IndexOf(folder.ChildMenuCollection);

                    long folderSize = folder.CopyTo(targetDirectory, localGameSet);
                    stats.TotalSize += folderSize;
                    stats.TransferSize += folderSize;
                    Debug.WriteLine(string.Format("Processed folder {0} ('{1}'), size: {2}KB", folder.Code, folder.Name, folderSize / 1024));
                }
            }
        }

        private static void DeleteRemoteApplicationFiles(IEnumerable<ApplicationFileInfo> filesToDelete, string remoteDirectory)
        {
            using (MemoryStream commandBuilder = new MemoryStream())
            {
                string data = $"#!/bin/sh\ncd \"{remoteDirectory}\"\n";
                commandBuilder.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);

                foreach (ApplicationFileInfo appInfo in filesToDelete)
                {
                    data = $"rm \"{appInfo.FilePath}\"\n";
                    commandBuilder.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
                }
                hakchi.RunTemporaryScript(commandBuilder, "cleanup.sh");
            }
        }

        private static void DeleteLocalApplicationFilesFromDirectory(IEnumerable<ApplicationFileInfo> filesToDelete, string rootDirectory)
        {
            foreach (ApplicationFileInfo appInfo in filesToDelete)
            {
                string filepath = rootDirectory + appInfo.FilePath.Substring(1).Replace('/', '\\');
                File.Delete(filepath);

                // determine if the folder is empty now -- if so, delete the folder also
                string directory = Path.GetDirectoryName(filepath);
                var dirInfo = new DirectoryInfo(directory);
                if (dirInfo.GetFiles().Length == 0 && dirInfo.GetDirectories().Length == 0)
                {
                    Directory.Delete(directory);
                }
            }
        }

    }
}
