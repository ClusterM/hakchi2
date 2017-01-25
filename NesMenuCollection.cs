using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui
{
    public class NesMenuCollection : List<INesMenuElement>
    {
        const int Letters = 10;

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
            for (i = 0; i < collections.Count; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    var folder = new NesMenuFolder();
                    var fname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).First().Name/*.ToUpper().Replace(" ", "")*/;
                    var lname = collections[j].Where(o => (o is NesGame) || (o is NesDefaultGame)).Last().Name/*.ToUpper().Replace(" ", "")*/;
                    folder.Child = collections[j];
                    folder.Name = fname.Substring(0, Math.Min(Letters, fname.Length)) + "... - " + lname.Substring(0, Math.Min(Letters, lname.Length)) + "...";
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
                    folder.Name = fname.Substring(0, Math.Min(Letters, fname.Length)) + "... - " + lname.Substring(0, Math.Min(Letters, lname.Length)) + "...";
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
