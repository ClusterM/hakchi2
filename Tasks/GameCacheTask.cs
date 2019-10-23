using com.clusterrr.hakchi_gui.Properties;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace com.clusterrr.hakchi_gui.Tasks
{
    public class GameCacheTask
    {
        public int LoadedGames
        {
            get;
            private set;
        }

        public GameCacheTask()
        {
            LoadedGames = 0;
        }

        public Tasker.Conclusion UpdateLocal(Tasker tasker, Object SyncObject = null)
        {
            if (hakchi.IsMd()) return Tasker.Conclusion.Success;

            tasker.SetTitle(Resources.UpdatingLocalCache);
            tasker.SetStatusImage(Resources.sign_sync);
            tasker.SetProgress(-1, -1, Tasker.State.Running, Resources.UpdatingLocalCache);

            var shell = hakchi.Shell;
            if (!shell.IsOnline) return Tasker.Conclusion.Abort;

            string gamesCloverPath = shell.ExecuteSimple("hakchi eval 'echo \"$squashfs$gamepath\"'", 2000, true);
            string cachePath = Path.Combine(Program.BaseDirectoryExternal, "games_cache");

            try
            {
                var reply = shell.ExecuteSimple($"[ -d \"{gamesCloverPath}\" ] && echo YES || echo NO");
                if (reply == "NO")
                {
                    gamesCloverPath = hakchi.GamesSquashFsPath;
                    reply = shell.ExecuteSimple($"[ -d \"{gamesCloverPath}\" ] && echo YES || echo NO");
                    if (reply == "NO")
                        throw new Exception("Unable to update local cache. games directory not accessible");
                }

                var list = shell.ExecuteSimple($"ls \"{gamesCloverPath}\" -p -1 | grep '/'").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int i = 0;
                foreach (var folder in list)
                {
                    string gameCode = folder.Substring(0, folder.Length - 1);
                    string gamePath = Path.Combine(cachePath, gameCode);
                    if (!Directory.Exists(gamePath))
                    {
                        try
                        {
                            Directory.CreateDirectory(gamePath);
                            using (var tar = new MemoryStream())
                            {
                                string cmd = $"cd {gamesCloverPath}/{gameCode} && tar -c *";
                                shell.Execute(cmd, null, tar, null, 10000, true);
                                tar.Seek(0, SeekOrigin.Begin);
                                using (var extractorTar = SharpCompress.Archives.Tar.TarArchive.Open(tar))
                                    extractorTar.WriteToDirectory(gamePath, new SharpCompress.Common.ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            }
                            ++LoadedGames;
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message + ex.StackTrace);
                            if (Directory.Exists(gamePath))
                            {
                                Trace.WriteLine($"Exception occured while loading data from NES/SNES mini, deleting \"{gamePath}\".");
                                Directory.Delete(gamePath, true);
                            }
                        }
                    }

                    tasker.SetProgress(++i, list.Length);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
                return Tasker.Conclusion.Error;
            }

            return Tasker.Conclusion.Success;
        }

    }
}
