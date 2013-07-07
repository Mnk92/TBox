using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Ionic.Zip;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;

namespace Solution.Scripts
{
    public class PutResources : IScript
    {
        [DirectoryList()]
        public string[] TargetPathes { get; set; }

        [Directory()]
        public string PathToDirectoryWithPackage { get; set; }

        public bool RemoveAfterUnpack { get; set; }

        [String("Package*.zip")]
        public string PackageMask { get; set; }

        [StringDictionary("js", "Scripts", "css", "Content")]
        public IDictionary<string, string> Aliases { get; set; }

        public void Run(IScriptContext context)
        {
            bool exist;
            var package = GetPackage(context, out exist);
            if (!exist) return;
            if (package == null)
            {
                MessageBox.Show("Can't find any package in folder: " + PathToDirectoryWithPackage, PackageMask);
                return;
            }
            if (UnpackPackage(package, context.PathResolver))
            {
                if (RemoveAfterUnpack) package.Delete();
            }
            else
            {
                MessageBox.Show("Can't find any files, according aliaces.", "Put resources", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private FileInfo GetPackage(IScriptContext context, out bool exist)
        {
            exist = false;
            var packages = new DirectoryInfo(context.PathResolver.Resolve(PathToDirectoryWithPackage))
                .EnumerateFiles(PackageMask)
                .OrderBy(x => x.CreationTime)
                .Reverse()
                .ToArray();
            FileInfo package = null;
            if (packages.Length > 0)
            {
                var result = new KeyValuePair<bool, string>();
                context.Sync(() =>
                    result = DialogsCache.ShowInputSelect("Select file to extract", "Script", packages.First().Name, x => true,
                                                          packages.Select(x => x.Name + "\t" + x.CreationTime).ToArray(), null, true)
                    );
                exist = result.Key;
                var value = result.Value.Split('\t').First();
                package = packages.FirstOrDefault(x => string.Equals(x.Name, value));
            }
            return package;
        }

        private bool UnpackPackage(FileInfo package, IPathResolver pathResolver)
        {
            var count = 0;
            using (var zf = ZipFile.Read(package.FullName))
            {
                foreach (var entry in zf)
                {
                    var directory = Path.GetDirectoryName(entry.FileName);
                    if (string.IsNullOrEmpty(directory)) continue;
                    var alias = Aliases.FirstOrDefault(x => directory.StartsWith(x.Key));
                    if (string.IsNullOrEmpty(alias.Key)) continue;
                    ++count;
                    Save(entry, alias.Value + entry.FileName.Substring(alias.Key.Length), pathResolver);
                }
            }
            return count > 0;
        }

        private void Save(ZipEntry entry, string name, IPathResolver pathResolver)
        {
            foreach (var path in TargetPathes.Select(pathResolver.Resolve))
            {
                var dir = Path.GetDirectoryName(name);
                var targetDir = Path.GetFullPath(Path.Combine(path, dir));
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                var targetPath = Path.GetFullPath(Path.Combine(targetDir, Path.GetFileName(name)));
                if (File.Exists(targetPath)) File.Delete(targetPath);
                entry.Extract(targetDir);
            }
        }
    }
}
