using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
    public class ClearFolders : IScript
    {
        private const int MaxFilesInError = 32;

        [DirectoryList]
        public IList<string> RootDirectories { get; set; }

        [StringList("bin", "obj", "log", "logs")]
        public IList<string> DirectoryNamesToClean { get; set; }

        [StringList("Libs", "Lib")]
        public IList<string> StopOnFolders { get; set; }

        public void Run(IScriptContext s)
        {
            var errors = new List<string>();
            var i = 0;
            foreach (var dir in RootDirectories.Select(x => new DirectoryInfo(s.PathResolver.Resolve(x))))
            {
                s.Updater.Update(dir.FullName, i++/(float)RootDirectories.Count);
                foreach (var subdir in FindSubDir(dir))
                {
                    Clear(subdir, errors);
                }
            }
            if (errors.Count <= 0) return;
            if (errors.Count > MaxFilesInError)
            {
                errors.RemoveRange(MaxFilesInError, errors.Count - MaxFilesInError);
                errors.Add("...");
            }
            MessageBox.Show("Can't remove next files: " + Environment.NewLine + string.Join(Environment.NewLine, errors));
        }

        private IEnumerable<DirectoryInfo> FindSubDir(DirectoryInfo dir)
        {
            if (dir.Attributes.HasFlag(FileAttributes.System) || dir.Attributes.HasFlag(FileAttributes.Hidden))
            {
                return new DirectoryInfo[0];
            }
            foreach (var folder in StopOnFolders)
            {
                if (Equals(folder, dir.Name))
                {
                    return new DirectoryInfo[0];
                }
            }
            if (DirectoryNamesToClean.Any(x => Equals(dir, x)))
            {
                return new[] { dir };
            }
            var toSearch = dir.GetDirectories()
                              .Where(x => !StopOnFolders.Any(y => Equals(x, y)));
            return toSearch
                .SelectMany(FindSubDir)
                .Where(x => x != null);
        }

        private static bool Equals(DirectoryInfo dir, string name)
        {
            return string.Equals(dir.Name, name, StringComparison.OrdinalIgnoreCase);
        }

        private static void Clear(DirectoryInfo dir, IList<string> errors)
        {
            foreach (var file in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    file.Delete();
                }
                catch { errors.Add(file.FullName); }
            }
            foreach (var subdir in dir.GetDirectories())
            {
                try
                {
                    subdir.Delete(true);
                }
                catch { errors.Add(subdir.FullName); }
            }
        }
    }
}
