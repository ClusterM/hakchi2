using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Tooling.Tasks
{
    public class SaveCurrentConfigAsBook : TaskableTool
    {
        Dictionary<NesMiniApplication, Manager.RomManager.Rom> _AppToRom = new Dictionary<NesMiniApplication, Manager.RomManager.Rom>();
        Dictionary<NesMiniApplication, Manager.CoverManager.Cover> _AppToCover = new Dictionary<NesMiniApplication, Manager.CoverManager.Cover>();
        public string TempRomFolder = System.IO.Path.Combine(Program.BaseDirectoryExternal, "tempRom");
        public SaveCurrentConfigAsBook() : base("Saving current config as book")
        {
        }
        public override void Execute()
        {
            ReportStatus("Importing all selected games in library");
            IOrderedEnumerable<NesMiniApplication> selectedGames = Manager.GameManager.GetInstance().getSelectedGames();
            int processed = 0;
            NesMenuCollection nmc = new NesMenuCollection();
            foreach (NesMiniApplication mn in selectedGames)
            {
                nmc.Add(mn);
                string localRom = System.IO.Path.Combine(mn.GamePath, mn.RomFile);
                if(!System.IO.Directory.Exists(TempRomFolder))
                {
                    System.IO.Directory.CreateDirectory(TempRomFolder);
                }
                string gameName = mn.Name;
                string fileName = System.IO.Path.GetFileNameWithoutExtension(mn.RomFile);
                string ext = System.IO.Path.GetExtension(mn.RomFile);
                string destRom = System.IO.Path.Combine(TempRomFolder, MakeValidFileName( gameName + ext));
                if (mn.RomFile.Trim() != "")
                {
                    System.IO.File.Copy(localRom, destRom);
                    _AppToRom[mn]= Manager.RomManager.getInstance().AddRom(destRom);


                    System.IO.File.Delete(destRom);
                }
                string coverExt = System.IO.Path.GetExtension(mn.IconPath);
                string destIcon = System.IO.Path.Combine(TempRomFolder, MakeValidFileName( gameName + coverExt));
                if (System.IO.File.Exists(mn.IconPath))
                {
                    if(System.IO.File.Exists(destIcon))
                    {
                        System.IO.File.Delete(destIcon);
                    }
                    System.IO.File.Copy(mn.IconPath, destIcon);
                    _AppToCover[mn]=Manager.CoverManager.getInstance().AddCover(destIcon);
                    System.IO.File.Delete(destIcon);
                }
                else
                {
                  
                }
                processed++;
                ReportProgress(processed * 100 / selectedGames.Count());

                Console.Write(mn.RomFile);
            }

            ReportStatus("Loading folder structure");
            ReportProgress(0);
            if (ConfigIni.FoldersMode != NesMenuCollection.SplitStyle.Custom)
            {
                nmc.Split(ConfigIni.FoldersMode, ConfigIni.MaxGamesPerFolder);
            }
            else
            {
                FoldersManagerForm fm = new FoldersManagerForm(nmc,true);

                nmc.AddBack();
            }
            Manager.BookManager.Book theBook = Manager.BookManager.getInstance().GetBookByName("HakchiImport");
            theBook.Pages.Clear();
            Manager.BookManager.Page rootPage = theBook.AddPage("0");
            ReportStatus("Creating pages");
            AddMenu(nmc, rootPage,theBook);
            ReportCompleted();
            
        }
        Dictionary<NesMenuCollection, Manager.BookManager.Page> pagePerCollection = new Dictionary<NesMenuCollection, Manager.BookManager.Page>();
        List<NesMenuCollection> processedPages = new List<NesMenuCollection>();
        private void AddMenu(NesMenuCollection menuCollection,Manager.BookManager.Page currentPage, Manager.BookManager.Book theBook)
        {
            pagePerCollection[menuCollection]= currentPage;
            processedPages.Add(menuCollection);
            foreach (var element in menuCollection)
            {
                if (element is NesMiniApplication)
                {
                    var app = element as NesMiniApplication;
                    Manager.BookManager.Entry entr = new Manager.BookManager.Entry();
                    entr.Emulator = app.GetEmulator();
                    if (_AppToRom.ContainsKey(app))
                    {
                        entr.Rom = _AppToRom[app];
                    }
                    if (_AppToCover.ContainsKey(app))
                    {
                        entr.Cover = _AppToCover[app];
                    }
                    entr.Label = app.Name;
                    currentPage.Entries.Add(entr);
                }
                if (element is NesMenuFolder)
                {
                    
                    var folder = element as NesMenuFolder;
                    if(!pagePerCollection.ContainsKey(folder.ChildMenuCollection))
                    {
                        Manager.BookManager.Page p = theBook.AddPage(folder.Name);
                        Manager.BookManager.Entry entr = new Manager.BookManager.Entry();
                        entr.IsLink = true;
                        entr.Label = folder.Name;
                        entr.PageId = p.Id;
                        currentPage.Entries.Add(entr);
                        AddMenu(folder.ChildMenuCollection, p, theBook);
                    }
                    else
                    {
                        Manager.BookManager.Entry entr = new Manager.BookManager.Entry();
                        entr.IsLink = true;
                        entr.Label = folder.Name;
                        entr.PageId = pagePerCollection[folder.ChildMenuCollection].Id;
                        currentPage.Entries.Add(entr);
                    }
                    /*
                    
                    if (folder.ChildMenuCollection.Count() > 0)
                    {
                        if (!processedPages.Contains(folder.ChildMenuCollection))
                        {
                            Manager.BookManager.Page p = theBook.AddPage(folder.Name);
                            Manager.BookManager.Entry entr = new Manager.BookManager.Entry();
                            entr.IsLink = true;
                            entr.Label = folder.Name;
                            entr.PageId = p.Id;
                            currentPage.Entries.Add(entr);
                            AddMenu(folder.ChildMenuCollection, p,currentPage, theBook);
                        }
                    }
                    else
                    {
                        //Most likely a back, but not quite sure...
                        if(parent != null)
                        {
                            Manager.BookManager.Entry entr = new Manager.BookManager.Entry();
                            entr.IsLink = true;
                            entr.Label = folder.Name;
                            entr.PageId = parent.Id;
                            currentPage.Entries.Add(entr);
                        }
                    }*/
                   

                }

            }
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
