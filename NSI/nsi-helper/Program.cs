using System;
using System.IO;
using System.Linq;

namespace nsi_helper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sr = new StreamWriter(args.Length > 1 ? File.Create(args[1]) : Console.OpenStandardOutput()))
            {
                var files = Directory.EnumerateFiles(args[0], "*", SearchOption.AllDirectories);
                var directories = Directory.EnumerateDirectories(args[0], "*", SearchOption.AllDirectories).Reverse();

                foreach (var file in files)
                {
                    sr.WriteLine($"Delete \"$INSTDIR\\{file.Substring(args[0].Length).TrimStart("\\"[0])}\"");
                }
                foreach (var directory in directories)
                {
                    sr.WriteLine($"RMDir \"$INSTDIR\\{directory.Substring(args[0].Length).TrimStart("\\"[0])}\"");
                }
            }
        }
    }
}
