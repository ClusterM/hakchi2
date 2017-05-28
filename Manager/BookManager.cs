using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Manager
{
    public class BookManager
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
            public Page GetPageById(int pageId)
            {
                Page ret = null;
                foreach(Page p in Pages)
                {
                    if(p.Id == pageId)
                    {
                        ret = p;
                        break;
                    }
                }
                return ret;
            }
            public Page AddPage(string name, bool forceNew)
            {
                Page ret = null;
                bool found = false;

                if (!forceNew)
                {
                    foreach (Page p in Pages)
                    {
                        if (p.FriendlyName == name)
                        {
                            found = true;
                            ret = p;
                        }
                    }
                }
                if (!found)
                {
                    Page p = new Page();
                    p.FriendlyName = name;
                    p.Id = GetNextPageId();
                    Pages.Add(p);
                    ret = p;
                    BookManager.getInstance().SaveSettings();
                }
                return ret;
            }
            public Page AddPage(string name)
            {
                return AddPage(name, true);
            }
        }
        public class Page
        {
            public override string ToString()
            {
                return FriendlyName;
            }
            public Page()
            {
                Entries = new List<Entry>();
            }
            public string FriendlyName { get; set; }
            public List<Entry> Entries { get; set; }
            public int Id { get; set; }
        }
        public  class Entry
        {
            public Entry()
            {
                IsLink = false;
            }
            public string Label { get; set; }
            public CoverManager.Cover Cover { get; set; }
            public int PageId;

            public RomManager.Rom Rom { get; set; }
            public EmulatorManager.Emulator Emulator { get; set; }
            public bool IsLink { get; set; }
        }
       
    }
}
