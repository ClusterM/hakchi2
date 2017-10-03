using System;
using System.Collections.Generic;

namespace mooftpserv
{
    public class FileSystemHelper
    {
        /// Handles TVFS path resolution, similar to Path.GetFullPath(Path.Combine(basePath, path))
        public static string ResolvePath(string basePath, string path)
        {
            // CF is missing String.IsNullOrWhiteSpace
            if (path == null || path.Trim() == "")
                return null;

            // first, make a complete unix path
            string fullPath;
            if (path[0] == '/') {
                fullPath = path;
            } else {
                fullPath = basePath;
                if (!fullPath.EndsWith("/"))
                    fullPath += "/";
                fullPath += path;
            }

            // then, remove ".." and "."
            List<string> tokens = new List<string>(fullPath.Split('/'));
            for (int i = 0; i < tokens.Count; ++i) {
                if (tokens[i] == "") {
                    if (i == 0 || i == tokens.Count - 1) {
                        continue; // ignore, start and end should be empty tokens
                    } else {
                        tokens.RemoveAt(i);
                        --i;
                    }
                } else if (tokens[i] == "..") {
                    if (i < 2) {
                        // cannot go higher than root, just remove the token
                        tokens.RemoveAt(i);
                        --i;
                    } else {
                        tokens.RemoveRange(i - 1, 2);
                        i -= 2;
                    }
                } else if (i < tokens.Count - 1 && tokens[i].EndsWith(@"\")) {
                    int slashes = 0;
                    for (int c = tokens[i].Length - 1; c >= 0 && tokens[i][c] == '\\'; --c)
                        ++slashes;

                    if (slashes % 2 != 0) {
                        // the slash was actually escaped, merge tokens
                        tokens[i] += ("/" + tokens[i + 1]);
                        ++i;
                    }
                }
            }

            if (tokens.Count > 1)
                return String.Join("/", tokens.ToArray());
            else
                return "/";
        }

    }
}

