using com.clusterrr.hakchi_gui.data;
using com.clusterrr.hakchi_gui.Properties;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using SpineGen.DrawingBitmaps;
using SpineGen.JSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using TeamShinkansen.Scrapers.Interfaces;

namespace com.clusterrr.hakchi_gui.Tasks
{
    class AddGamesTask
    {
        readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "hakchi2");

        private List<NesApplication> NewGames = new List<NesApplication>();
        private static string selectedFile = null;
        public static DialogResult SelectFile(Tasker tasker, string[] files)
        {
            if (tasker.HostForm.Disposing) return DialogResult.Cancel;
            if (tasker.HostForm.InvokeRequired)
            {
                return (DialogResult)tasker.HostForm.Invoke(new Func<Tasker, string[], DialogResult>(SelectFile), new object[] { tasker, files });
            }
            try
            {
                using (var form = new SelectFileForm(files))
                {
                    tasker.PushState(Tasker.State.Paused);
                    var result = form.ShowDialog();
                    tasker.PopState();
                    selectedFile = form.listBoxFiles.SelectedItem != null ? form.listBoxFiles.SelectedItem.ToString() : null;
                    return result;
                }
            }
            catch (InvalidOperationException) { }
            return DialogResult.Cancel;
        }

        public AddGamesTask(ListView listViewGames, IEnumerable<string> files, bool asIs = false)
        {
            this.listViewGames = listViewGames;
            this.files = files;
            this.addedApps = new List<NesApplication>();
            this.asIs = asIs;
        }

        private ListView listViewGames;
        private IEnumerable<string> files;
        private List<NesApplication> addedApps;
        private bool asIs;

        public Tasker.Conclusion AddGames(Tasker tasker, Object syncObject = null)
        {
            tasker.SetProgress(-1, -1, Tasker.State.Running, Resources.AddingGames);
            tasker.SetTitle(Resources.AddingGames);
            tasker.SetStatusImage(Resources.sign_cogs);

            // static presets
            NesApplication.ParentForm = tasker.HostForm;
            NesApplication.NeedPatch = null;
            NesApplication.Need3rdPartyEmulator = null;
            NesApplication.CachedCoverFiles = null;
            NesGame.IgnoreMapper = null;
            SnesGame.NeedAutoDownloadCover = null;

            int total = files.Count();
            int count = 0;
            var gamesWithMultipleArt = new List<NesApplication>();
            foreach (var sourceFileName in files)
            {
                NesApplication app = null;
                try
                {
                    tasker.SetStatus(string.Format(Resources.AddingGame, Path.GetFileName(sourceFileName)));
                    var fileName = sourceFileName;
                    var ext = Path.GetExtension(sourceFileName).ToLower();
                    byte[] rawData = null;
                    string tmp = null;
                    if (!asIs && (ext == ".7z" || ext == ".zip" || ext == ".rar" || ext == ".clvg"))
                    {
                        if (ext == ".clvg")
                        {
                            tmp = TempHelpers.getUniqueTempPath();
                            Directory.CreateDirectory(tmp);

                            using (var file = File.OpenRead(sourceFileName))
                            using (var reader = ReaderFactory.Open(file))
                            {
                                reader.WriteAllToDirectory(tmp, new ExtractionOptions() { ExtractFullPath = true, PreserveFileTime = true });
                            }

                            var gameFilesInArchive = Directory.GetFiles(tmp, "*.desktop").Select(o => new DirectoryInfo(o)).Cast<DirectoryInfo>().ToArray();

                            switch (gameFilesInArchive.LongLength)
                            {
                                case 0:
                                    // no files found
                                    break;

                                case 1:
                                    // one file found
                                    fileName = gameFilesInArchive[0].FullName;
                                    break;

                                default:
                                    // multiple files found
                                    var r = SelectFile(tasker, gameFilesInArchive.Select(o => o.FullName).ToArray());
                                    if (r == DialogResult.OK)
                                        fileName = selectedFile;
                                    else if (r == DialogResult.Ignore)
                                        fileName = sourceFileName;
                                    else continue;
                                    break;
                            }

                        }
                        else
                        {
                            using (var extractor = ArchiveFactory.Open(sourceFileName))
                            {
                                var filesInArchive = extractor.Entries;
                                var gameFilesInArchive = new List<string>();
                                foreach (var f in extractor.Entries)
                                {
                                    if (!f.IsDirectory)
                                    {
                                        var e = Path.GetExtension(f.Key).ToLower();
                                        if (e == ".desktop")
                                        {
                                            gameFilesInArchive.Clear();
                                            gameFilesInArchive.Add(f.Key);
                                            break;
                                        }
                                        else if (CoreCollection.Extensions.Contains(e))
                                        {
                                            gameFilesInArchive.Add(f.Key);
                                        }
                                    }
                                }
                                if (gameFilesInArchive.Count == 1) // Only one known file (or app)
                                {
                                    fileName = gameFilesInArchive[0];
                                }
                                else if (gameFilesInArchive.Count > 1) // Many known files, need to select
                                {
                                    var r = SelectFile(tasker, gameFilesInArchive.ToArray());
                                    if (r == DialogResult.OK)
                                        fileName = selectedFile;
                                    else if (r == DialogResult.Ignore)
                                        fileName = sourceFileName;
                                    else continue;
                                }
                                else if (filesInArchive.Count() == 1) // No known files but only one another file
                                {
                                    fileName = filesInArchive.First().Key;
                                }
                                else // Need to select
                                {
                                    var r = SelectFile(tasker, filesInArchive.Select(f => f.Key).ToArray());
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
                                        || filesInArchive.Select(f => f.Key).Contains(Path.GetFileNameWithoutExtension(fileName) + ".jpg") // Or it has cover in archive
                                        || filesInArchive.Select(f => f.Key).Contains(Path.GetFileNameWithoutExtension(fileName) + ".png")
                                        || filesInArchive.Select(f => f.Key).Contains(Path.GetFileNameWithoutExtension(fileName) + ".ips") // Or IPS file
                                        )
                                    {
                                        tmp = Path.Combine(tempDirectory, fileName);
                                        Directory.CreateDirectory(tmp);
                                        extractor.WriteToDirectory(tmp, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                                        fileName = Path.Combine(tmp, fileName);
                                    }
                                    else
                                    {
                                        extractor.Entries.Where(f => f.Key == fileName).First().WriteTo(o);
                                        rawData = new byte[o.Length];
                                        o.Seek(0, SeekOrigin.Begin);
                                        o.Read(rawData, 0, (int)o.Length);
                                    }
                                }
                            }
                        }
                        
                    }
                    app = NesApplication.Import(fileName, sourceFileName, rawData, asIs);

                    if (ext == ".clvg")
                    {
                        app.SkipCoreSelect = true;
                    }
                    else
                    {
                        if (app != null && app.CoverArtMatches != null && app.CoverArtMatches.Count() > 1)
                        {
                            gamesWithMultipleArt.Add(app);
                        }
                        NewGames.Add(app);
                    }

                    if (app is ISupportsGameGenie && Path.GetExtension(fileName).ToLower() == ".nes")
                    {
                        var lGameGeniePath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".xml");
                        if (File.Exists(lGameGeniePath))
                        {
                            GameGenieDataBase lGameGenieDataBase = new GameGenieDataBase(app);
                            lGameGenieDataBase.ImportCodes(lGameGeniePath, true);
                            lGameGenieDataBase.Save();
                        }
                    }

                    if (!string.IsNullOrEmpty(tmp) && Directory.Exists(tmp)) Directory.Delete(tmp, true);
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException) return Tasker.Conclusion.Abort;
                    if (ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        Trace.WriteLine(ex.InnerException.Message + ex.InnerException.StackTrace);
                        tasker.ShowError(ex.InnerException, Path.GetFileName(sourceFileName));
                    }
                    else
                    {
                        Trace.WriteLine(ex.Message + ex.StackTrace, Path.GetFileName(sourceFileName));
                        tasker.ShowError(ex);
                    }
                    return Tasker.Conclusion.Error;
                }
                if (app != null)
                {
                    addedApps.Add(app);
                }
                tasker.SetProgress(++count, total);
            }
            if (gamesWithMultipleArt.Count > 0)
            {
                tasker.HostForm.Invoke(new Action(() => {
                    using (SelectCoverDialog selectCoverDialog = new SelectCoverDialog())
                    {
                        selectCoverDialog.Games.AddRange(gamesWithMultipleArt);
                        selectCoverDialog.ShowDialog(tasker.HostForm);
                    }
                }));
            }
            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion ScrapeGames(Tasker tasker, Object syncObject = null)
        {
            if (Program.TheGamesDBAPI == null)
            {
                return Tasker.Conclusion.Success;
            }

            try
            {
                var idList = new Dictionary<UInt32, int>();

                foreach (var app in NewGames)
                {
                    if (app.Metadata.OriginalCrc32 != 0 &&
                        data.GamesDB.HashLookup.ContainsKey(app.Metadata.OriginalCrc32) &&
                        data.GamesDB.HashLookup[app.Metadata.OriginalCrc32].TgdbId.Count > 0)
                    {
                        if (!idList.ContainsKey(app.Metadata.OriginalCrc32))
                        {
                            idList.Add(app.Metadata.OriginalCrc32, data.GamesDB.HashLookup[app.Metadata.OriginalCrc32].TgdbId.First());
                        }
                    }
                }   

                var api = Program.TheGamesDBAPI;
                var infoList = new Dictionary<int, IScraperData>();

                foreach (var chunk in idList.Values.Distinct().ToList().ChunkBy(20)){
                    var apiResponse = api.GetInfoByID(chunk.ToArray());

                    while (true)
                    {
                        apiResponse.Wait();
                        foreach (var result in apiResponse.Result.Items)
                        {
                            infoList.Add(int.Parse(result.ID), result);
                        }

                        if (apiResponse.Result.HasNextPage)
                        {
                            apiResponse = apiResponse.Result.GetNextPage();
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                var counter = 0;
                
                foreach (var app in NewGames)
                {
                    tasker.SetProgress(++counter, NewGames.Count);
                    tasker.SetStatus(string.Format(Resources.Scraping0, app.Name));

                    int tgdbId;
                    if (app.Metadata.OriginalCrc32 != 0 &&
                    data.GamesDB.HashLookup.ContainsKey(app.Metadata.OriginalCrc32) &&
                    data.GamesDB.HashLookup[app.Metadata.OriginalCrc32].TgdbId.Count > 0 &&
                    infoList.ContainsKey(tgdbId = data.GamesDB.HashLookup[app.Metadata.OriginalCrc32].TgdbId.First()))
                    {
                        try
                        {
                            var apiResult = infoList[tgdbId];

                            if (apiResult.Name != null)
                            {
                                var name = GamesDB.HashLookup[app.Metadata.OriginalCrc32].Name;

                                if (name != null)
                                {
                                    name = Shared.CleanName(name, true);
                                }

                                app.Desktop.Name = name ?? apiResult.Name;
                                app.Desktop.SortName = Shared.GetSortName(app.Desktop.Name);
                            }

                            if (apiResult.Publishers != null && apiResult.Publishers.Length > 0)
                            {
                                app.Desktop.Publisher = String.Join(", ", apiResult.Publishers).ToUpper();
                            }
                            else if (apiResult.Developers != null && apiResult.Developers.Length > 0)
                            {
                                if (apiResult.ReleaseDate != null)
                                {
                                    app.Desktop.Copyright = $"© {apiResult.ReleaseDate.Year} {String.Join(", ", apiResult.Developers)}";
                                }
                                else
                                {
                                    app.Desktop.Copyright = $"© {String.Join(", ", apiResult.Developers)}";
                                }
                            }

                            if (apiResult.Description != null)
                                app.Desktop.Description = apiResult.Description;

                            if (apiResult.ReleaseDate != null)
                                app.Desktop.ReleaseDate = apiResult.ReleaseDate.ToString("yyyy-MM-dd");

                            if (apiResult.PlayerCount > 0)
                            {
                                app.Desktop.Players = Convert.ToByte(apiResult.PlayerCount);
                                app.Desktop.Simultaneous = apiResult.PlayerCount == 2;
                            }

                            if (apiResult.Genres != null && apiResult.Genres.Length > 0)
                            {
                                foreach (var genre in apiResult.Genres)
                                {
                                    var match = Genre.GenreList.Where(g => g.GamesDbId.Contains(genre.ID)).Select(g => g);

                                    if (match.Count() > 0)
                                    {
                                        var firstGenre = match.First();

                                        app.Desktop.Genre = firstGenre.DesktopName;
                                        break;
                                    }
                                }
                            }

                            using (var wc = new HakchiWebClient())
                            {

                                try
                                {
                                    var front = apiResult.Images.Where(i => i.Type == TeamShinkansen.Scrapers.Enums.ArtType.Front).ToArray();

                                    if (front.Length > 0 && !app.CoverArtMatchSuccess)
                                    {
                                        tasker.SetStatus(string.Format(Resources.DownloadingFrontArtFor0, apiResult.Name));
                                        var data = wc.DownloadData(front[0].Url);
                                        using (var ms = new MemoryStream(data))
                                        using (var bm = new Bitmap(ms))
                                        {
                                            app.SetImage(bm);
                                        }
                                    }
                                }
                                catch (WebException ex) { }

                                try
                                {
                                    tasker.SetStatus(string.Format(Resources.DownloadingClearLogoFor0, apiResult.Name));
                                    var imageData = wc.DownloadData($"https://cdn.thegamesdb.net/images/original/clearlogo/{apiResult.ID}.png");

                                    using (var ms = new MemoryStream(imageData))
                                    using (var clearLogo = File.OpenWrite(Path.Combine(app.BasePath, $"{app.Code}_logo.png")))
                                    {
                                        ms.Seek(0, SeekOrigin.Begin);
                                        ms.CopyTo(clearLogo);
                                        ms.Seek(0, SeekOrigin.Begin);

                                        tasker.SetStatus(string.Format(Resources.GeneratingSpineFor0, apiResult.Name));
                                        using (var bm = new Bitmap(ms))
                                        using (var cl = new SystemDrawingBitmap(bm))
                                        using (var spineImage = (Program.SpineTemplates.Where(e => e.Value.Name == "Custom (Black Grid)").First().Value as SpineTemplate<Bitmap>).Process(cl).Bitmap)
                                        {
                                            app.SetMdMini(spineImage, NesMenuElementBase.GameImageType.MdSpine);
                                        }
                                    }
                                }
                                catch (WebException ex) { }
                            }
                        }
                        catch (Exception innerEx) { }
                    }
                }
            }
            catch (Exception ex) { }

            return Tasker.Conclusion.Success;
        }

        public Tasker.Conclusion UpdateListView(Tasker tasker, Object syncObject = null)
        {
            if (tasker.HostForm.Disposing) return Tasker.Conclusion.Abort;
            if (tasker.HostForm.InvokeRequired)
            {
                return (Tasker.Conclusion)tasker.HostForm.Invoke(new Func<Tasker, Object, Tasker.Conclusion>(UpdateListView), new object[] { tasker, syncObject });
            }
            if (addedApps != null)
            {
                tasker.SetTitle(Resources.UpdatingList);

                // show select core dialog if applicable
                var unknownApps = new List<NesApplication>();
                foreach (var app in addedApps)
                {
                    if (!app.SkipCoreSelect && app.Metadata.AppInfo.Unknown)
                        unknownApps.Add(app);
                }
                if (unknownApps.Count > 0)
                {
                    using (SelectCoreDialog selectCoreDialog = new SelectCoreDialog())
                    {
                        selectCoreDialog.Games.AddRange(unknownApps);
                        selectCoreDialog.ShowDialog(tasker.HostForm);
                    }
                }

                // show select cover dialog if applicable
                unknownApps.Clear();
                foreach (var app in addedApps)
                {
                    if (!app.CoverArtMatchSuccess && app.CoverArtMatches.Any())
                        unknownApps.Add(app);
                }
                if (unknownApps.Count > 0)
                {
                    using (SelectCoverDialog selectCoverDialog = new SelectCoverDialog())
                    {
                        selectCoverDialog.Games.AddRange(unknownApps);
                        selectCoverDialog.ShowDialog(tasker.HostForm);
                    }
                }

                // update list view
                try
                {
                    listViewGames.BeginUpdate();
                    foreach (ListViewItem item in listViewGames.Items)
                        item.Selected = false;

                    // add games, only new ones
                    var newApps = addedApps.Distinct(new NesApplication.NesAppEqualityComparer());
                    var newCodes = from app in newApps select app.Code;
                    var oldAppsReplaced = from app in listViewGames.Items.Cast<ListViewItem>().ToArray()
                                          where (app.Tag is NesApplication) && newCodes.Contains((app.Tag as NesApplication).Code)
                                          select app;

                    // find "new apps" group
                    ListViewGroup newGroup = null;
                    if (listViewGames.Groups.Count > 0)
                    {
                        newGroup = listViewGames.Groups.OfType<ListViewGroup>().Where(group => group.Header == Resources.ListCategoryNew).FirstOrDefault();
                    }

                    int i = 0, max = newApps.Count() + oldAppsReplaced.Count();
                    foreach (var replaced in oldAppsReplaced)
                    {
                        listViewGames.Items.Remove(replaced);
                        tasker.SetProgress(++i, max);
                    }
                    foreach (var newApp in newApps)
                    {
                        ConfigIni.Instance.AddNewSelectedGame(newApp.Code);
                        var item = new ListViewItem(newApp.Name);
                        item.Group = newGroup;
                        item.Tag = newApp;
                        item.Selected = true;
                        item.Checked = true;
                        listViewGames.Items.Add(item);
                        tasker.SetProgress(++i, max);
                    }
                }
                finally
                {
                    listViewGames.EndUpdate();
                }
            }
            return Tasker.Conclusion.Success;
        }

    }
}
