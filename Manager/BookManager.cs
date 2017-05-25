using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Manager
{
    class BookManager
    {
        private string BookLibraryPath = System.IO.Path.Combine(Program.BaseDirectoryExternal, "Books\\data.dat");
        private static BookManager instance;
        public static BookManager getInstance()
        {
            if(instance == null)
            {
                instance = new BookManager();
            }
            return instance;
        }
        private BookManager()
        {
            LoadSettings();
        }
        private void LoadSettings()
        {
            if (System.IO.File.Exists(BookLibraryPath))
            {
                _BookLibrary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Book>>(System.IO.File.ReadAllText(BookLibraryPath));
            }
        }
        public void SaveSettings()
        {
            string directory = System.IO.Path.GetDirectoryName(BookLibraryPath);
            if(!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            System.IO.File.WriteAllText(BookLibraryPath, Newtonsoft.Json.JsonConvert.SerializeObject(_BookLibrary, Newtonsoft.Json.Formatting.Indented));
        }

        public Book GetBookByName(string name)
        {
            Book ret = null;
            foreach(Book b in _BookLibrary)
            {
                if(b.Name == name)
                {
                    ret = b;
                    break;
                }
            }
            if(ret == null)
            {
                ret = new Book();
                ret.Name = name;
                _BookLibrary.Add(ret);
                SaveSettings();
            }
            return ret;
        }
        public List<Book> GetLibrary()
        {
            List<Book> ret = new List<Book>();
            ret.AddRange(_BookLibrary);
            return ret;
        }
        private List<Book> _BookLibrary = new List<Book>();
        public class Book
        {
            public override string ToString()
            {
                return Name;
            }
            public Book()
            {
                Pages = new List<Page>();
            }
            public List<Page> Pages { get; set; }
            public string Name { get; set; }
            public int RootPageId { get; set; }
            private int GetNextPageId()
            {
                int currentHigh = 0;
                foreach(Page p in Pages)
                {
                    if(p.Id > currentHigh)
                    {
                        currentHigh = p.Id;
                    }
                }
                return currentHigh + 1;
            }
            public void AddPage(string name)
            {
                bool found = false;

                foreach(Page p in Pages)
                {
                    if(p.FriendlyName ==name)
                    {
                        found = true;
                    }
                }
                if(!found)
                {
                    Page p = new Page();
                    p.FriendlyName = name;
                    p.Id = GetNextPageId();
                    Pages.Add(p);
                    BookManager.getInstance().SaveSettings();
                }
            }
        }
        public class Page
        {
            public Page()
            {
                Entries = new List<Entry>();
            }
            public string FriendlyName { get; set; }
            public List<Entry> Entries { get; set; }
            public int Id { get; set; }
        }
        public abstract class Entry
        {
            public string Label { get; set; }
            public CoverManager.Cover Cover { get; set; }
        }
        public class LinkEntry:Entry
        {
            public string PageId;
        }
        public class GameEntry : Entry
        {
            public RomManager.Rom Rom { get; set; }
            public EmulatorManager.Emulator Emulator { get; set; }
        }
    }
}
