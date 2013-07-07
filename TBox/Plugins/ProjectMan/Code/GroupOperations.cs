using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.ProjectMan;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Plugins.ProjectMan.Code
{
    class GroupOperations
    {
        public void Append(IList<UMenuItem> menu, ProjectContext context, IPluginContext pluginContext, string[] paths)
        {
            var enabled = menu.Count > 0;
            menu.Add(new USeparator());
            menu.Add(new UMenuItem
            {
                IsEnabled = enabled,
                Header = ProjectManLang.UpdateAll,
                Icon = pluginContext.GetIcon(context.SvnProvider.Path, 3),
                OnClick = o => Run(() => Update(context, paths))
            });
            menu.Add(new UMenuItem
            {
                IsEnabled = enabled,
                Header = ProjectManLang.RebuildAllInRelease,
                Icon = pluginContext.GetIcon(context.MsBuildProvider.PathToMsBuild, 0),
                OnClick = o => Run(() => Rebuild(context, "Release", paths))
            });
            menu.Add(new UMenuItem
            {
                IsEnabled = enabled,
                Header = ProjectManLang.RebuildAllInDebug,
                Icon = pluginContext.GetIcon(context.MsBuildProvider.PathToMsBuild, 0),
                OnClick = o => Run(() => Rebuild(context, "Debug", paths))
            });
            menu.Add(new UMenuItem
            {
                IsEnabled = enabled,
                Header = ProjectManLang.UpdateAndRebuildAllInDebug,
                Icon = pluginContext.GetIcon(context.MsBuildProvider.PathToMsBuild, 0),
                OnClick = o => Run(() => UpdateAndBuid(context, paths))
            });
        }

        private void UpdateAndBuid(ProjectContext context, IList<string> paths)
        {
            Update(context, paths);
            Rebuild(context, "Debug", paths);
        }

        private static void Rebuild(ProjectContext context, string mode, IList<string> paths)
        {
            ReBuild(context, mode, paths);
        }

        private static void Update(ProjectContext context, IEnumerable<string> paths)
        {
            context.SvnProvider.Do("update", string.Join("*", paths), true);
        }

        private static void ReBuild(ProjectContext context, string mode, IEnumerable<string> paths)
        {
            var slnFiles = paths.SelectMany(x => Directory.GetFiles(x, "*.sln")).ToArray();
            if (!slnFiles.Any()) return;
            context.MsBuildProvider.BuildSlnFile(mode, string.Join("*", slnFiles), true);
        }

        private static void Run(Action task)
        {
            ThreadPool.QueueUserWorkItem(o => task());
        }
    }
}
