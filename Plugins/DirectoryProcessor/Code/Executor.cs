using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Menu;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.DirectoryProcessor.Code
{
    class Executor
    {
        private readonly IPathResolver pathResolver;
        private static readonly ILog Log = LogManager.GetLogger<Executor>();

        public Executor(IPathResolver pathResolver)
        {
            this.pathResolver = pathResolver;
        }

        public IList<UMenuItem> Process(DirInfo info)
        {
            return ProcessSubdirectory(info.Key, info, info.Deep);
        }

        private UMenuItem FillMenu(FileSystemInfo d, DirInfo info, int deep)
        {
            var menu = new UMenuItem { Header = d.Name };
            if (deep > 0)
            {
                menu.Items = ProcessSubdirectory(d.FullName, info, deep - 1);
            }
            if (deep == 0 || menu.Items.Count == 0)
            {
                var exec = info.Executable;
                var args = info.ExtendedArguments;
                menu.OnClick = o => Cmd.Start(exec, Log, args, false, directory: d.FullName);
            }
            return menu;
        }

        private IList<UMenuItem> ProcessSubdirectory(string path, DirInfo info, int deep)
        {
            try
            {
                return new DirectoryInfo(pathResolver.Resolve(path)).GetDirectories()
                    .Where(x => !(x.Attributes.HasFlag(FileAttributes.Hidden) || x.Attributes.HasFlag(FileAttributes.System)))
                    .Select(p => FillMenu(p, info, deep)).ToArray();
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't access to directory: '{0}'", path);
                return new UMenuItem[0];
            }
        }
    }
}
