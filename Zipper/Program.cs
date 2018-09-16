using System.IO;
using System.IO.Compression;

namespace Zipper
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
                return 1;

            if (File.Exists(args[1]))
                File.Delete(args[1]);

            ZipFile.CreateFromDirectory(args[0], args[1]);
            return 0;
        }
    }
}
