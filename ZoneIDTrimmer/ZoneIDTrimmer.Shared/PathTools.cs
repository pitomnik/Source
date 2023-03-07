using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ZoneIDTrimmer.Shared
{
    public class PathTools
    {
        private readonly List<string> _ignoreFoldersList;
        private readonly List<string> _ignoreFilesList;

        public PathTools(string ignoreFolders, string ignoreFiles)
        {
            _ignoreFoldersList = CreateIgnoreList(ignoreFolders);
            _ignoreFilesList = CreateIgnoreList(ignoreFiles);
        }

        public IEnumerable<string> IgnoreFoldersList
        {
            [DebuggerNonUserCode]
            get { return _ignoreFoldersList.AsReadOnly(); }
        }

        public IEnumerable<string> IgnoreFilesList
        {
            [DebuggerNonUserCode]
            get { return _ignoreFilesList.AsReadOnly(); }
        }

        public static string Normalize(string path, bool safe = false)
        {
            path = path.Trim('"');

            if (path.LastOrDefault() == Path.VolumeSeparatorChar)
            {
                path = String.Concat(path, Path.DirectorySeparatorChar);
            }

            return safe ? String.Concat("\"", path, "\"") : path;
        }

        public bool IsPathIgnored(string path)
        {
            return IsFileIgnored(path) || IsFolderIgnored(path);
        }

        public bool IsFileIgnored(string path)
        {
            var file = Path.GetFileName(path);

            return _ignoreFilesList.Any(x => x.Equals(file, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsFolderIgnored(string path)
        {
            var folder = Path.GetFileName(path);

            return _ignoreFoldersList.Any(x => x.Equals(folder, StringComparison.OrdinalIgnoreCase));
        }

        private List<string> CreateIgnoreList(string s)
        {
            var tokens = s.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

            return new List<string>(tokens);
        }
    }
}
