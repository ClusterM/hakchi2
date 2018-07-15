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
        public NesMenuCollection Games
        {
            get; set;
        }

        public GameCacheTask()
        {
            Games = new NesMenuCollection();
        }

        public Tasker.Conclusion UpdateLocal(Tasker tasker, Object SyncObject = null)
        {
            tasker.SetTitle(Resources.UpdatingLocalCache);
            tasker.SetStatusImage(Resources.sign_sync);
            tasker.SetProgress(-1, -1, Tasker.State.Running, Resources.UpdatingLocalCache);

            var shell = hakchi.Shell;
            if (!shell.IsOnline) return Tasker.Conclusion.Abort;

            string gamesCloverPath = shell.ExecuteSimple("hakchi eval 'echo \"$squashfs$gamepath\"'", 2000, true);
            string cachePath = Path.Combine(Program.BaseDirectoryExternal, "games_cache");

            try
            {
                var reply = shell.ExecuteSimple($"[ -d {gamesCloverPath} ] && echo YES || echo NO");
                if (reply == "NO")
                {
                    gamesCloverPath = hakchi.GamesPath;
                    reply = shell.ExecuteSimple($"[ -d {gamesCloverPath} ] && echo YES || echo NO");
                    if (reply == "NO")
                        throw new Exception("Unable to update local cache. games directory not accessible");
                }

                int i = 0;
                foreach (NesDefaultGame game in Games)
                {
                    string gamePath = Path.Combine(cachePath, game.Code);

                    if (!Directory.Exists(gamePath))
                    {
                        try
                        {
                            Directory.CreateDirectory(gamePath);
                            using (var tar = new MemoryStream())
                            {
                                string cmd = $"cd {gamesCloverPath}/{game.Code} && tar -c *";
                                shell.Execute(cmd, null, tar, null, 10000, true);
                                tar.Seek(0, SeekOrigin.Begin);
                                using (var extractorTar = SharpCompress.Archives.Tar.TarArchive.Open(tar))
                                    extractorTar.WriteToDirectory(gamePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex.Message + ex.StackTrace);
                            if (Directory.Exists(gamePath))
                            {
                                Trace.WriteLine($"Exception, erasing \"{gamePath}\".");
                                Directory.Delete(gamePath, true);
                            }
                        }
                    }

                    tasker.SetProgress(++i, Games.Count);
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
