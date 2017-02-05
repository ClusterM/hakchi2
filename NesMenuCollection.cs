using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuCollection : List<INesMenuElement>
    {
        const int Letters = 25;

        public void Split(int maxElements)
        {
            if (Count <= maxElements) return;
            var total = Count;
            var partsCount = (int)Math.Ceiling((float)total / (float)maxElements);
            var perPart = (int)Math.Ceiling((float)total / (float)partsCount);

            var sorted = this.OrderBy(o => o.Name);

            var collections = new List<NesMenuCollection>();
            var collection = new NesMenuCollection();
            int i = 0;
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

            // Method A
            if (collections.Count >= 12) // minimum amount of games/folders on screen without glitches
            {
                var root = this;
                root.Clear();
                foreach (var coll in collections)
                {
                    var fname = coll.Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Name;
                    var lname = coll.Where(o => (o is NesGame) || (o is NesDefaultGame)).Last().Name;
                    var folder = new NesMenuFolder();
                    folder.Child = coll;

                    if (ConfigIni.FoldersAZ == true)
                    {
                        folder.Name = fname.Substring(0, 2).ToUpper() + " - " + lname.Substring(0, 2).ToUpper();
                    }
                    else
                    {
                        folder.Name = fname.Substring(0, Math.Min(Letters, fname.Length)) + (fname.Length > Letters ? "..." : "") + " - " + lname.Substring(0, Math.Min(Letters, lname.Length)) + (lname.Length > Letters ? "..." : "");
                    }

                    root.Add(folder);
                    coll.Add(new NesMenuFolder() { Name = "<- Back", Image = Resources.back, Child = root });
                }
            }
            else
            // Method B
            {
                for (i = 0; i < collections.Count; i++)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        var folder = new NesMenuFolder();
                        var fname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Name/*.ToUpper().Replace(" ", "")*/;
                        var lname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).Last().Name/*.ToUpper().Replace(" ", "")*/;
                        folder.Child = collections[j];

                        if (ConfigIni.FoldersAZ == true)
                        {
                            folder.Name = fname.Substring(0, 2).ToUpper() + " - " + lname.Substring(0, 2).ToUpper();
                        }
                        else
                        {
                            folder.Name = fname.Substring(0, Math.Min(Letters, fname.Length)) + (fname.Length > Letters ? "..." : "") + " - " + lname.Substring(0, Math.Min(Letters, lname.Length)) + (lname.Length > Letters ? "..." : "");
                        }

                        folder.Initial = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Code;
                        folder.First = true;
                        collections[i].Insert(0, folder);
                    }
                    for (int j = i + 1; j < collections.Count; j++)
                    {
                        var folder = new NesMenuFolder();
                        var fname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Name/*.ToUpper().Replace(" ", "")*/;
                        var lname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).Last().Name/*.ToUpper().Replace(" ", "")*/;
                        folder.Child = collections[j];

                        if (ConfigIni.FoldersAZ == true)
                        {
                            folder.Name = fname.Substring(0, 2).ToUpper() + " - " + lname.Substring(0, 2).ToUpper();
                        }
                        else
                        {
                            folder.Name = fname.Substring(0, Math.Min(Letters, fname.Length)) + (fname.Length > Letters ? "..." : "") + " - " + lname.Substring(0, Math.Min(Letters, lname.Length)) + (lname.Length > Letters ? "..." : "");
                        }

                        folder.Initial = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Code;
                        folder.First = false;
                        collections[i].Insert(collections[i].Count, folder);
                    }
                }
                Clear();
                AddRange(collections[0]);
            }
        }
    }
}
