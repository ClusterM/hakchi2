using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Manager
{
    public class BitmapManager
    {
        public static BitmapManager getInstance()
        {
            if(instance == null)
            {
                instance = new BitmapManager();
            }
            return instance;
        }
        private static BitmapManager instance;
        private BitmapManager()
        {

        }
        private Dictionary<string, System.Drawing.Bitmap> bitmapCache = new Dictionary<string, System.Drawing.Bitmap>();
        public System.Drawing.Bitmap GetBitmap(string path)
        {
            if(bitmapCache.ContainsKey(path))
            {
                return bitmapCache[path];
            }
            else
            {
                if(System.IO.File.Exists(path))
                {
                    bitmapCache[path] = new System.Drawing.Bitmap(path);
                    return bitmapCache[path];
                }
                else
                {
                    return GetBitmap(".\\images\\blank_app.png");
                }
            }
        }
    }
}
