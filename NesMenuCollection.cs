using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuCollection : List<INesMenuElement>
    {
        const int FoldersEqualLetters = 3;
        const int PagesEqualLetters = 3;
        public enum SplitStyle
        {
            NoSplit = 0,
            Original_NoSplit = 1,
            Auto = 2,
            Original_Auto = 3,
            PagesEqual = 4,
            Original_PagesEqual = 5,
            FoldersEqual = 6,
            Original_FoldersEqual = 7,
            FoldersAlphabetic_FoldersEqual = 8,
            Original_FoldersAlphabetic_FoldersEqual = 9,
            FoldersAlphabetic_PagesEqual = 10,
            Original_FoldersAlphabetic_PagesEqual = 11,
            FoldersGroupByApp = 12,
            Custom = 99
        }

        public void Split(SplitStyle style, int maxElements = 35)
        {
            bool originalToRoot = false;
            int originalCount = 0;
            switch (style)
            {
                case SplitStyle.Original_NoSplit:
                case SplitStyle.Original_Auto:
                case SplitStyle.Original_FoldersAlphabetic_FoldersEqual:
                case SplitStyle.Original_FoldersAlphabetic_PagesEqual:
                case SplitStyle.Original_FoldersEqual:
                case SplitStyle.Original_PagesEqual:
                    style--;
                    originalCount = this.Where(o => (o is NesApplication) && (o as NesApplication).IsOriginalGame).Count();
                    if (originalCount > 0)
                        originalToRoot = true;
                    break;
            }
            if (style == SplitStyle.NoSplit && !originalToRoot) return;
            if (((style == SplitStyle.Auto && !originalToRoot) ||
                (style == SplitStyle.FoldersEqual && !originalToRoot) ||
                (style == SplitStyle.PagesEqual) && !originalToRoot) &&
                (Count <= maxElements)) return;
            var total = Count - originalCount;
            var partsCount = (int)Math.Ceiling((float)total / (float)maxElements);
            var perPart = (int)Math.Ceiling((float)total / (float)partsCount);
            var alphaNum = new Regex("[^a-zA-Z0-9]");

            NesMenuCollection root;
            if (!originalToRoot)
                root = this;
            else
            {
                root = new NesMenuCollection();
                root.AddRange(this.Where(o => (o is NesApplication) && !(o as NesApplication).IsOriginalGame));
                if (root.Count == 0)
                    return;
                this.RemoveAll(o => (o is NesApplication) && !(o as NesApplication).IsOriginalGame);
                this.Add(new NesMenuFolder()
                {
                    Name = Resources.FolderNameMoreGames,
                    Position = NesMenuFolder.Priority.Rightmost,
                    ChildMenuCollection = root
                });
            }

            var sorted = root.OrderBy(o => o.SortName);
            var collections = new List<NesMenuCollection>();
            int i = 0;
            if (style == SplitStyle.Auto || style == SplitStyle.FoldersEqual || style == SplitStyle.PagesEqual)
            {
                var collection = new NesMenuCollection();
                foreach (var game in sorted)
                {
                    collection.Add(game);
                    i++;
                    if (((i % perPart) == 0) || (i == sorted.Count()))
                    {
                        collections.Add(collection);
                        collection = new NesMenuCollection();
                    }
                }
            }

            if (style == SplitStyle.Auto)
            {
                if (collections.Count >= 12)
                    style = SplitStyle.FoldersEqual;
                else
                    style = SplitStyle.PagesEqual;
            }

            // Folders, equal
            if (style == SplitStyle.FoldersEqual) // minimum amount of games/folders on screen without glitches
            {
                root.Clear();
                foreach (var coll in collections)
                {
                    var fname = alphaNum.Replace(coll.Where(o => o is NesApplication).First().SortName.ToUpper(), "");
                    var lname = alphaNum.Replace(coll.Where(o => o is NesApplication).Last().SortName.ToUpper(), "");

                    var folder = new NesMenuFolder() { ChildMenuCollection = coll, NameParts = new string[] { fname, lname }, Position = NesMenuFolder.Priority.Right };
                    coll.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                    root.Add(folder);
                }
                TrimFolderNames(root);
            }
            else if (style == SplitStyle.PagesEqual)
            // Pages, equal
            {
                root.Clear();
                root.AddRange(collections[0]);
                collections[0] = root;
                for (i = 0; i < collections.Count; i++)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var fname = alphaNum.Replace(collections[j].Where(o => o is NesApplication).First().SortName.ToUpper(), "");
                        var lname = alphaNum.Replace(collections[j].Where(o => o is NesApplication).Last().SortName.ToUpper(), "");
                        var folder = new NesMenuFolder()
                        {
                            ChildMenuCollection = collections[j],
                            NameParts = new string[] { fname, lname },
                            Position = NesMenuFolder.Priority.Left
                        };
                        collections[i].Insert(0, folder);
                    }
                    for (int j = i + 1; j < collections.Count; j++)
                    {
                        var fname = alphaNum.Replace(collections[j].Where(o => o is NesApplication).First().SortName.ToUpper(), "");
                        var lname = alphaNum.Replace(collections[j].Where(o => o is NesApplication).Last().SortName.ToUpper(), "");
                        var folder = new NesMenuFolder()
                        {
                            ChildMenuCollection = collections[j],
                            NameParts = new string[] { fname, lname },
                            Position = NesMenuFolder.Priority.Right
                        };
                        collections[i].Insert(collections[i].Count, folder);
                    }
                    TrimFolderNames(collections[i]);
                }
            }
            else if (style == SplitStyle.FoldersAlphabetic_PagesEqual || style == SplitStyle.FoldersAlphabetic_FoldersEqual)
            {
                var letters = new Dictionary<char, NesMenuCollection>();
                for (char ch = 'A'; ch <= 'Z'; ch++)
                    letters[ch] = new NesMenuCollection();
                letters['#'] = new NesMenuCollection();
                foreach (var game in root)
                {
                    if (!(game is NesApplication)) continue;
                    var letter = game.SortName.Substring(0, 1).ToUpper()[0];
                    if (letter < 'A' || letter > 'Z')
                        letter = '#';
                    letters[letter].Add(game);
                }

                root.Clear();
                foreach (var letter in letters.Keys)
                    if (letters[letter].Count > 0)
                    {
                        string folderImageId = "folder_" + letter.ToString().ToLower();
                        if (letter < 'A' || letter > 'Z') folderImageId = "folder_number";
                        var folder = new NesMenuFolder() { ChildMenuCollection = letters[letter], Name = letter.ToString(), Position = NesMenuFolder.Priority.Right, ImageId = folderImageId };
                        if (style == SplitStyle.FoldersAlphabetic_PagesEqual)
                        {
                            folder.ChildMenuCollection.Split(SplitStyle.PagesEqual, maxElements);
                            folder.ChildMenuCollection.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                            foreach (NesMenuFolder f in folder.ChildMenuCollection.Where(o => o is NesMenuFolder))
                                if (f.ChildMenuCollection != root)
                                    f.ChildMenuCollection.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                        }
                        else if (style == SplitStyle.FoldersAlphabetic_FoldersEqual)
                        {
                            folder.ChildMenuCollection.Split(SplitStyle.FoldersEqual, maxElements);
                            folder.ChildMenuCollection.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                        }
                        root.Add(folder);
                    }
            }
            else if (style == SplitStyle.FoldersGroupByApp)
            {
                var apps = new Dictionary<string, NesMenuCollection>();
                var customApps = new Dictionary<string, NesMenuCollection>();
                foreach(var appInfo in AppTypeCollection.Apps)
                    apps[appInfo.Name] = new NesMenuCollection();

                foreach (var game in root)
                {
                    if (!(game is NesApplication)) continue;
                    NesApplication app = game as NesApplication;

                    AppTypeCollection.AppInfo ai = app.Metadata.AppInfo;
                    if (!ai.Unknown)
                        apps[ai.Name].Add(game);
                    else
                    {
                        if(!string.IsNullOrEmpty(app.Desktop.Bin))
                        {
                            if (!customApps.ContainsKey(app.Desktop.Bin))
                                customApps.Add(app.Desktop.Bin, new NesMenuCollection());
                            customApps[app.Desktop.Bin].Add(game);
                        }
                        else
                            apps[AppTypeCollection.UnknownApp.Name].Add(game);
                    }
                }

                root.Clear();
                foreach(var app in apps)
                    if(app.Value.Count > 0)
                    {
                        string folderImageId = "folder";
                        var folder = new NesMenuFolder() { ChildMenuCollection = app.Value, Name = app.Key, Position = NesMenuFolder.Priority.Right, ImageId = folderImageId };
                        //folder.ChildMenuCollection.Split(SplitStyle.FoldersEqual, maxElements);
                        folder.ChildMenuCollection.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                        root.Add(folder);
                    }
                foreach (var app in customApps)
                    if (app.Value.Count > 0)
                    {
                        string folderImageId = "folder";
                        var folder = new NesMenuFolder() { ChildMenuCollection = app.Value, Name = app.Key, Position = NesMenuFolder.Priority.Right, ImageId = folderImageId };
                        //folder.ChildMenuCollection.Split(SplitStyle.FoldersEqual, maxElements);
                        folder.ChildMenuCollection.Add(new NesMenuFolder() { Name = Resources.FolderNameBack, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = root });
                        root.Add(folder);
                    }
            }
            if (originalToRoot)
            {
                if (style != SplitStyle.PagesEqual)
                    root.Add(new NesMenuFolder() { Name = Resources.FolderNameOriginalGames, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = this });
                else
                {
                    foreach (var collection in collections)
                        collection.Add(new NesMenuFolder() { Name = Resources.FolderNameOriginalGames, ImageId = "folder_back", Position = NesMenuFolder.Priority.Back, ChildMenuCollection = this });
                }
            }
        }

        public void Unsplit(List<NesMenuCollection> ignore = null)
        {
            if (ignore == null)
                ignore = new List<NesMenuCollection>();
            ignore.Add(this);
            var newElements = new List<INesMenuElement>();
            var oldElements = new List<INesMenuElement>();
            foreach (NesMenuFolder item in from i in this where i is NesMenuFolder select i)
            {
                if (ignore.Contains(item.ChildMenuCollection))
                    continue;
                item.ChildMenuCollection.Unsplit(ignore);
                newElements.AddRange(item.ChildMenuCollection);
                item.ChildMenuCollection.Clear();
                oldElements.Add(item);
            }
            this.AddRange(newElements);
            this.RemoveAll(o => oldElements.Contains(o));
        }

        public void AddBack(List<NesMenuCollection> ignore = null)
        {
            if (ignore == null)
                ignore = new List<NesMenuCollection>();
            ignore.Add(this);
            foreach (NesMenuFolder item in from i in this where i is NesMenuFolder select i)
            {
                if (ignore.Contains(item.ChildMenuCollection))
                    continue;
                var back = new NesMenuFolder(Resources.FolderNameBack, "folder_back");
                back.Position = NesMenuFolder.Priority.Back;
                back.ChildMenuCollection = this;
                item.ChildMenuCollection.AddBack(ignore);
                item.ChildMenuCollection.Add(back);
            }
        }

        void TrimFolderNames(NesMenuCollection nesMenuCollection)
        {
            const int minChars = 3;
            const int maxChars = 8;
            var folders = nesMenuCollection.Where(o => o is NesMenuFolder).OrderBy(o => o.Name).ToArray();
            for (int i = 1; i < folders.Length; i++)
            {
                var prevFolder = i > 0 ? (folders[i - 1] as NesMenuFolder) : null;
                var currentFolder = folders[i] as NesMenuFolder;
                var nameA = prevFolder.NameParts[1];
                var nameB = currentFolder.NameParts[0];
                int l = Math.Min(maxChars - 1, Math.Max(nameA.Length, nameB.Length));
                while ((nameA.Substring(0, Math.Min(l, nameA.Length)) !=
                    nameB.Substring(0, Math.Min(l, nameB.Length))) && l >= minChars)
                    l--;
                nameA = nameA.Substring(0, Math.Min(l + 1, nameA.Length));
                nameB = nameB.Substring(0, Math.Min(l + 1, nameB.Length));
                if (nameA == nameB) // There is no point to make long name
                    nameA = nameB = nameA.Substring(0, Math.Min(minChars, nameA.Length));
                prevFolder.NameParts = new string[] { prevFolder.NameParts[0], nameA };
                currentFolder.NameParts = new string[] { nameB, currentFolder.NameParts[1] };
            }
            if (folders.Length > 0)
            {
                var firstFolder = folders[0] as NesMenuFolder;
                firstFolder.NameParts = new string[] { firstFolder.NameParts[0].Substring(0, Math.Min(firstFolder.NameParts[0].Length, minChars)), firstFolder.NameParts[1] };

                var lastFolder = folders[folders.Length - 1] as NesMenuFolder;
                lastFolder.NameParts = new string[] { lastFolder.NameParts[0], lastFolder.NameParts[1].Substring(0, Math.Min(lastFolder.NameParts[1].Length, minChars)), };
            }
        }
    }
}
