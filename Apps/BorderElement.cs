using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.Apps
{
    class BorderElement
    {
        public static string BackgroundsDirectory
        {
            get
            {
                return System.IO.Path.Combine(Program.BaseDirectoryExternal, "backgrounds");
            }
        }

        public BorderElement(string path)
        {
            String str = path;
            Path = path;
            Name = str.Substring(1+str.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
            IconPath = Path + System.IO.Path.DirectorySeparatorChar + Name + "_thumbnail.png";
        }

        private string iconPath;
        public string IconPath { get => iconPath; set => iconPath = value; }

        private string path;
        public string Path { get => path; set => path = value; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public static Bitmap LoadBitmap(string path)
        {
            //Open file in read only mode
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            //Get a binary reader for the file stream
            using (BinaryReader reader = new BinaryReader(stream))
            {
                //copy the content of the file into a memory stream
                var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                //make a new Bitmap object the owner of the MemoryStream
                return new Bitmap(memoryStream);
            }
        }

        internal static object FromDirectory(string borderDir)
        {
            BorderElement border = new BorderElement(borderDir);

            return border;
        }
    }
}
