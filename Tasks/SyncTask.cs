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
        readonly string remoteTempDirectory = "/tmp/upload-test";
        readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "hakchi2");
        public const int UpdateFreq = 100;

        public NesMenuCollection Games
        {
            get; set;
        }

        private class GamesTreeStats
        {
            public List<NesMenuCollection> allMenus = new List<NesMenuCollection>();
            public int TotalGames = 0;
            public long TotalSize = 0;
            public long TransferSize = 0;
        }

        private string exportDirectory;
        private string uploadPath;
        private bool exportLinked;
        private GamesTreeStats stats;
        private HashSet<ApplicationFileInfo> localGameSet;
        private IEnumerable<ApplicationFileInfo> transferGameSet;
        Dictionary<string, string> originalGames;
        NesApplication.CopyMode copyMode;

        public SyncTask()
        {
            Games = new NesMenuCollection();
            exportDirectory = string.Empty;
            uploadPath = string.Empty;
            exportLinked = false;
            stats = new GamesTreeStats();
            localGameSet = new HashSet<ApplicationFileInfo>();
            transferGameSet = null;
            originalGames = new Dictionary<string, string>();
            copyMode = ConfigIni.Instance.SyncLinked ? NesApplication.CopyMode.LinkedSync : NesApplication.CopyMode.Sync;
        }

        public Tasker.Conclusion ShowFoldersManager(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.Disposing) return Tasker.Conclusion.Undefined;
            if (tasker.HostForm.InvokeRequired)
            {
                return (Tasker.Conclusion)tasker.HostForm.Invoke(new Func<Tasker, Object, Tasker.Conclusion>(ShowFoldersManager), new object[] { tasker, syncObject });
            }
            tasker.SetStatus(Resources.RunningFoldersManager);
            try
            {
                using (FoldersManagerForm form = new FoldersManagerForm(Games, MainForm.StaticRef))
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = form.ShowDialog() == DialogResult.OK ? Tasker.Conclusion.Success : Tasker.Conclusion.Abort;
                    tasker.PopState();
                    return result;
                }
            }
            catch (InvalidOperationException) { }
            return Tasker.Conclusion.Abort;
        }

        public Tasker.Conclusion ShowExportDialog(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.Disposing) return Tasker.Conclusion.Undefined;
            if (tasker.HostForm.InvokeRequired)
            {
                return (Tasker.Conclusion)tasker.HostForm.Invoke(new Func<Tasker, Object, Tasker.Conclusion>(ShowExportDialog), new object[] { tasker, syncObject });
            }
            tasker.SetStatus(Resources.SelectDrive);
            try
            {
                using (ExportGamesDialog driveSelectDialog = new ExportGamesDialog())
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = driveSelectDialog.ShowDialog() == DialogResult.OK;
                    tasker.PopState();
                    if (!result)
                        return Tasker.Conclusion.Abort;
                    this.exportLinked = driveSelectDialog.LinkedExport;
                    this.exportDirectory = driveSelectDialog.ExportPath;
                    if (!Directory.Exists(driveSelectDialog.ExportPath))
                        Directory.CreateDirectory(driveSelectDialog.ExportPath);
                }
                copyMode = exportLinked ? NesApplication.CopyMode.LinkedExport : NesApplication.CopyMode.Export;
                return Tasker.Conclusion.Success;
            }
            catch (InvalidOperationException) { }
            return Tasker.Conclusion.Abort;
        }

        public Tasker.Conclusion BuildMenu(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.BuildingMenu);
            if (ConfigIni.Instance.FoldersMode == NesMenuCollection.SplitStyle.Custom)
            {
                if (ShowFoldersManager(tasker) != Tasker.Conclusion.Success)
                    return Tasker.Conclusion.Abort;
                Games.AddBack();
            }
            else
                Games.Split(ConfigIni.Instance.FoldersMode, ConfigIni.Instance.MaxGamesPerFolder);
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion BuildFiles(Tasker tasker, Object syncObject = null)
        {
            tasker.SetStatus(Resources.AddingGames);
            AddMenu(Games, originalGames, copyMode, localGameSet, stats);
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion CheckLocalStorageRequirements(Tasker tasker, Object syncObject = null)
        {
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

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion CalculateLocalDiff(Tasker tasker, Object syncObject = null)
        {
            // list current files on drive
            tasker.SetStatus(Resources.CalculatingDiff);
            var exportDriveGameSet = ApplicationFileInfo.GetApplicationFileInfoForDirectory(exportDirectory);

            // calculating diff
            var exportDriveGamesToDelete = exportDriveGameSet.Except(localGameSet);
            transferGameSet = localGameSet.Except(exportDriveGameSet);

            // delete any files on the device that aren't present in current layout
            DeleteLocalApplicationFilesFromDirectory(exportDriveGamesToDelete, exportDirectory);

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion SyncLocalGames(Tasker tasker, Object syncObject = null)
        {
            // now transfer whatever games are remaining
            Debug.WriteLine("Exporting games: " + Shared.SizeSuffix(stats.TotalSize));
            long max = transferGameSet.Sum(afi => afi.FileSize);
            long value = 0;
            DateTime startTime = DateTime.Now, lastTime = DateTime.Now;
            tasker.SetProgress(0, max, Tasker.State.Running, Resources.CopyingGames);
            foreach (var afi in transferGameSet)
            {
                string path = new Uri(exportDirectory + "/" + afi.FilePath).LocalPath;

                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (DateTime.Now.Subtract(lastTime).TotalMilliseconds > UpdateFreq)
                {
                    transferForm.SetAdvancedProgress(value, max, afi.FilePath);
                    lastTime = DateTime.Now;
                }

                // either read actual file, or file stream
                if (!string.IsNullOrEmpty(afi.LocalFilePath))
                {
                    if (afi.FileSize > NesApplication.MaxCompress)
                    {
                        using (var stream = new TrackableFileStream(afi.LocalFilePath, FileMode.Open))
                        {
                            stream.OnProgress += ((long pos, long len) => {
                                if (DateTime.Now.Subtract(lastTime).TotalMilliseconds > UpdateFreq)
                                {
                                    transferForm.SetAdvancedProgress(value + pos, max, afi.FilePath);
                                    lastTime = DateTime.Now;
                                }
                            });
                            using (var f = File.Open(path, FileMode.Create))
                                stream.CopyTo(f);
                            File.SetLastWriteTimeUtc(path, afi.ModifiedTime);
                        }
                    }
                    else
                    {
                        File.Copy(afi.LocalFilePath, path, true);
                    }
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
                value += afi.FileSize;
            }
            Debug.WriteLine("Uploaded " + (int)(max / 1024) + "kb in " + DateTime.Now.Subtract(startTime).TotalSeconds + " seconds");

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

        public Tasker.Conclusion CheckRemoteStorageRequirements(Tasker tasker, Object syncObject = null)
        {
            // calculating size constraints
            tasker.SetStatus(Resources.CalculatingFreeSpace);
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

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion PrepareRemoteTransfer(Tasker tasker, Object syncObject = null)
        {
            hakchi.ShowSplashScreen();
            hakchi.Shell.ExecuteSimple("hakchi eval 'umount \"$gamepath\"'");
            tasker.AddFinalTask(FinishRemoteTransfer);
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion FinishRemoteTransfer(Tasker tasker, Object syncObject = null)
        {
            try
            {
                if (hakchi.Shell.IsOnline)
                {
                    hakchi.Shell.ExecuteSimple("hakchi overmount_games; uistart", 2000, true);
                    MemoryStats.Refresh();
                }
            }
            catch { }
#if !VERY_DEBUG
            try
            {
                Directory.Delete(tempDirectory, true);
            }
            catch { }
#endif
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion CalculateRemoteDiff(Tasker tasker, Object syncObject = null)
        {
            // clean up previous directories (separate game storage vs not)
            tasker.SetStatus(Resources.CleaningUp);
            hakchi.Shell.ExecuteSimple("find \"" + (ConfigIni.Instance.UploadToTmp ? remoteTempDirectory : hakchi.RemoteGameSyncPath) + "/\" -maxdepth 1 | tail -n +2 " +
                "| grep -" + (ConfigIni.Instance.SeparateGameStorage ? "v" : "") + "Ee '(/snes(-usa|-eur|-jpn)?|/nes(-usa|-jpn)?|/)$' " +
                "| while read f; do rm -rf \"$f\"; done", 0, true);

            // get the remote list of files, timestamps, and sizes
            tasker.SetStatus(Resources.CalculatingDiff);
            string gamesOnDevice = hakchi.Shell.ExecuteSimple($"mkdir -p \"{uploadPath}\"; cd \"{uploadPath}\"; find . -type f -exec sh -c \"stat \\\"{{}}\\\" -c \\\"%n %s %y\\\"\" \\;", 0, true);
            var remoteGameSet = ApplicationFileInfo.GetApplicationFileInfoFromConsoleOutput(gamesOnDevice);

            // delete any remote files that aren't present locally
            var remoteGamesToDelete = remoteGameSet.Except(localGameSet);
            DeleteRemoteApplicationFiles(remoteGamesToDelete, uploadPath);

            // only keep the local files that aren't matching on the mini
            transferGameSet = localGameSet.Except(remoteGameSet);

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion SyncRemoteGamesFTP(Tasker tasker, Object syncObject = null)
        {
            // transfer games
            tasker.SetProgress(0, 100, Tasker.State.Running, Resources.UploadingGames);
            bool uploadSuccessful = false;
            if (!transferGameSet.Any())
            {
                Debug.WriteLine("No file to upload");
                uploadSuccessful = true;
            }
            else
            {
                Debug.WriteLine("Uploading through FTP");
                using (var ftp = new FtpWrapper(transferGameSet))
                {
                    Debug.WriteLine($"Upload size: " + Shared.SizeSuffix(ftp.Length));
                    if (ftp.Length > 0)
                    {
                        DateTime startTime = DateTime.Now, lastTime = DateTime.Now;
                        ftp.OnReadProgress += delegate (long pos, long len, string filename)
                        {
                            if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= UpdateFreq)
                            {
                                transferForm.SetAdvancedProgress(pos, len, filename);
                                lastTime = DateTime.Now;
                            }
                        };
                        if (ftp.Connect((hakchi.Shell as INetworkShell).IPAddress, 21, hakchi.USERNAME, hakchi.PASSWORD))
                        {
                            ftp.Upload(uploadPath);
                            uploadSuccessful = true;
                            Debug.WriteLine("Uploaded " + (int)(ftp.Length / 1024) + "kb in " + DateTime.Now.Subtract(startTime).TotalSeconds + " seconds");
                        }
                    }
                    transferForm.SetAdvancedProgress(ftp.Length, ftp.Length, "");
                }
            }

            // don't continue if upload wasn't successful
            if (!uploadSuccessful)
            {
                Debug.WriteLine("Something happened during transfer, cancelling");
                return Tasker.Conclusion.Error;
            }

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion SyncRemoteGamesShell(Tasker tasker, Object syncObject = null)
        {
            // transfer games
            tasker.SetProgress(0, 100, Tasker.State.Running, Resources.UploadingGames);
            bool uploadSuccessful = false;
            if (!transferGameSet.Any())
            {
                Debug.WriteLine("No file to upload");
                uploadSuccessful = true;
            }
            else
            {
                Debug.WriteLine("Uploading through tar file");
                using (var gamesTar = new TarStream(transferGameSet, "."))
                {
                    Debug.WriteLine($"Upload size: " + Shared.SizeSuffix(gamesTar.Length));
                    if (gamesTar.Length > 0)
                    {
                        DateTime startTime = DateTime.Now, lastTime = DateTime.Now;
                        bool done = false;
                        gamesTar.OnAdvancedReadProgress += delegate (long pos, long len, string filename)
                        {
                            if (done) return;
                            if (DateTime.Now.Subtract(lastTime).TotalMilliseconds >= UpdateFreq)
                            {
                                transferForm.SetAdvancedProgress(pos, len, filename);
                                lastTime = DateTime.Now;
                            }
                        };
                        hakchi.Shell.Execute($"tar -xvC \"{uploadPath}\"", gamesTar, null, null, 0, true);
                        Debug.WriteLine("Uploaded " + (int)(gamesTar.Length / 1024) + "kb in " + DateTime.Now.Subtract(startTime).TotalSeconds + " seconds");

                        uploadSuccessful = true;
                        done = true;
#if VERY_DEBUG
                        File.Delete(Program.BaseDirectoryExternal + "\\DebugSyncOutput.tar");
                        gamesTar.Position = 0;
                        gamesTar.CopyTo(File.OpenWrite(Program.BaseDirectoryExternal + "\\DebugSyncOutput.tar"));
#endif
                    }
                    transferForm.SetAdvancedProgress(gamesTar.Length, gamesTar.Length, "");
                }
            }

            // don't continue if upload wasn't successful
            if (!uploadSuccessful)
            {
                Debug.WriteLine("Something happened during transfer, cancelling");
                return Tasker.Conclusion.Error;
            }

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion RemoteCleanup(Tasker tasker, Object syncObject = null)
        {
            // delete any empty directories we may have left during the differential sync
            tasker.SetStatus(Resources.CleaningUp);
            hakchi.Shell.ExecuteSimple($"for f in $(find \"{uploadPath}\" -type d -mindepth 1 -maxdepth 2); do {{ find \"$f\" -type f -mindepth 1 | grep -v pixelart | grep -v autoplay " +
                "| wc -l | { read wc; test $wc -eq 0 && rm -rf \"$f\"; } } ; done", 0);
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion SyncOriginalGames(Tasker tasker, Object syncObject = null)
        {
            if (originalGames.Any())
            {
                using (MemoryStream commandBuilder = new MemoryStream())
                {
                    tasker.SetStatus(Resources.UploadingOriginalGames);

                    string data = $"#!/bin/sh\ncd \"/tmp\"\n";
                    commandBuilder.Write(Encoding.UTF8.GetBytes(data), 0, data.Length);
                    foreach (var originalCode in originalGames.Keys)
                    {
                        string originalSyncCode = "";
                        switch (ConfigIni.Instance.ConsoleType)
                        {
                            case hakchi.ConsoleType.NES:
                            case hakchi.ConsoleType.Famicom:
                                originalSyncCode =
                                    $"src=\"{hakchi.SquashFsPath}{hakchi.GamesSquashFsPath}/{originalCode}\" && " +
                                    $"dst=\"{uploadPath}/{originalGames[originalCode]}/{originalCode}\" && " +
                                    $"mkdir -p \"$dst\" && " +
                                    $"([ -L \"$dst/autoplay\" ] || ln -s \"$src/autoplay\" \"$dst/\") && " +
                                    $"([ -L \"$dst/pixelart\" ] || ln -s \"$src/pixelart\" \"$dst/\")" +
                                    "\n";
                                break;
                            case hakchi.ConsoleType.SNES_EUR:
                            case hakchi.ConsoleType.SNES_USA:
                            case hakchi.ConsoleType.SuperFamicom:
                                originalSyncCode =
                                    $"src=\"{hakchi.SquashFsPath}{hakchi.GamesSquashFsPath}/{originalCode}\" && " +
                                    $"dst=\"{uploadPath}/{originalGames[originalCode]}/{originalCode}\" && " +
                                    $"mkdir -p \"$dst\" && " +
                                    $"([ -L \"$dst/autoplay\" ] || ln -s \"$src/autoplay\" \"$dst/\")" +
                                    "\n";
                                break;
                        }
                        commandBuilder.Write(Encoding.UTF8.GetBytes(originalSyncCode), 0, originalSyncCode.Length);
                    }
                    tasker.SetProgress(1, 2);

                    hakchi.RunTemporaryScript(commandBuilder, "originalgamessync.sh");
                }

                tasker.SetProgress(2, 2);
            }
            return Tasker.Conclusion.Success;
        }

        private TaskerTransferForm transferForm;

        public Tasker.Conclusion ExportGames(Tasker tasker, Object syncObject = null)
        {
            // get specialized view
            transferForm = tasker.GetSpecificView<TaskerTransferForm>();

            // set up progress bar
            tasker.SetTitle(Resources.ExportGames);
            tasker.SetState(Tasker.State.Starting);
            tasker.SetStatusImage(Resources.sign_up);

            // safeguard
            if (Games == null || Games.Count == 0)
                return Tasker.Conclusion.Error;

            // add sub-tasks
            tasker.AddTask(ShowExportDialog, 0);
            tasker.AddTask(BuildMenu, 0);
            tasker.AddTask(BuildFiles, 1);
            tasker.AddTask(CheckLocalStorageRequirements, 1);
            tasker.AddTask(CalculateLocalDiff, 1);
            tasker.AddTask(SyncLocalGames, 12);

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion UploadGames(Tasker tasker, Object syncObject = null)
        {
            // get specialized view
            transferForm = tasker.GetSpecificView<TaskerTransferForm>();

            // set up progress bar
            tasker.SetTitle(Resources.UploadGames);
            tasker.SetState(Tasker.State.Starting);
            tasker.SetStatusImage(Resources.sign_up);

            // safeguards
            if (!hakchi.Shell.IsOnline || Games == null || Games.Count == 0)
                return Tasker.Conclusion.Error;

            // set up upload path
            if (ConfigIni.Instance.UploadToTmp)
            {
                uploadPath = remoteTempDirectory;
            }
            else
            {
                uploadPath = hakchi.GetRemoteGameSyncPath(ConfigIni.Instance.ConsoleType);
            }

            // add sub-tasks
            tasker.AddTask(BuildMenu, 0);
            tasker.AddTask(BuildFiles, 1);
            tasker.AddTask(CheckRemoteStorageRequirements, 1);
            tasker.AddTask(PrepareRemoteTransfer, 1);
            tasker.AddTask(CalculateRemoteDiff, 1);
            if (ConfigIni.Instance.ForceSSHTransfers || hakchi.Shell is clovershell.ClovershellConnection)
            {
                tasker.AddTask(SyncRemoteGamesShell, 28);
            }
            else if (hakchi.Shell is INetworkShell)
            {
                tasker.AddTask(SyncRemoteGamesFTP, 28);
            }
            tasker.AddTask(RemoteCleanup, 1);
            tasker.AddTask(SyncOriginalGames, 1);
            tasker.AddTask(ShellTasks.SyncConfig, 1);

            return Tasker.Conclusion.Success;
        }

        // internal methods

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
            // deleting files
            foreach (ApplicationFileInfo appInfo in filesToDelete)
            {
                string filepath = rootDirectory + appInfo.FilePath.Substring(1).Replace('/', '\\');
                File.Delete(filepath);
            }
            Shared.DirectoryDeleteEmptyDirectories(rootDirectory);
        }

    }
}
