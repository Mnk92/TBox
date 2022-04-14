using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Menu;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.Tools;
using Mnk.TBox.Locales.Localization.Plugins.ProjectMan;
using Mnk.TBox.Plugins.ProjectMan.Code;
using Mnk.TBox.Plugins.ProjectMan.Code.Settings;

namespace Mnk.TBox.Plugins.ProjectMan
{
    [PluginInfo(typeof(ProjectManLang), 43, PluginGroup.Development)]
    public class ProjectMan : ConfigurablePlugin<Settings, Config>
    {
        private static readonly ILog Log = LogManager.GetLogger<ProjectMan>();
        private readonly GroupOperations groupOperations = new GroupOperations();
        private readonly SvnStatisticOperations svnStatisticOperations = new SvnStatisticOperations();
        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            var projectContext = CreateContext();
            var items = Config.Dirs.CheckedItems.ToArray();
            var list = new List<KeyValuePair<string, UMenuItem>>();
            foreach (var dir in items)
            {
                CreateNewProject(list, dir, projectContext);
            }
            var paths = list.Select(x => x.Key).ToArray();
            var menu = list.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            groupOperations.Append(menu, projectContext, Context, paths);
            svnStatisticOperations.Append(menu, projectContext, Context, Config.SvnUserName);
            Menu = menu.ToArray();
        }

        private ProjectContext CreateContext()
        {
            return new ProjectContext(
                new SvnProvider(Config.PathToSvn),
                new MsBuildProvider(Config.PathToMsBuild, Context.DataProvider.ToolsPath)
                );
        }

        private void CreateNewProject(ICollection<KeyValuePair<string, UMenuItem>> list, ProjectInfo info, ProjectContext context)
        {
            var key = Context.PathResolver.Resolve(info.Key);
            if (!Directory.Exists(key))
            {
                Log.Write("Path not exist: '{0}'", key);
                return;
            }
            list.Add(new KeyValuePair<string, UMenuItem>(
                             key,
                             (new Project(info, context, Context)).MenuItem
                             ));
        }
    }
}
