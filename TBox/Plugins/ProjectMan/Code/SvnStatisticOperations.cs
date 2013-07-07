using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.ProjectMan;
using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Plugins.ProjectMan.Code
{
    class SvnStatisticOperations
    {
        public void Append(IList<UMenuItem> menu, ProjectContext context, IPluginContext pluginContext, string userName)
        {
            if (!Directory.Exists(pluginContext.DataProvider.ReadOnlyDataPath)) return;
            var dirs = Directory.EnumerateDirectories(pluginContext.DataProvider.ReadOnlyDataPath).ToArray();
            if (dirs.Length == 0) return;
            menu.Add(new USeparator());
            foreach (var dir in dirs)
            {
                menu.Add(new UMenuItem
                {
                    Header = ProjectManLang.ShowMyChanges + ": " + Path.GetFileName(dir),
                    Icon = pluginContext.GetIcon(context.SvnProvider.Path, 0),
                    OnClick = o => context.SvnProvider.Do("log", dir, "/findtype:260 /findstring:" + userName)
                });
            }
        }
    }
}
