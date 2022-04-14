using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Models;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.ProjectMan;
using Mnk.TBox.Plugins.ProjectMan.Code.Settings;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Plugins.ProjectMan.Code
{
    sealed class Project
    {
        private static readonly ILog Log = LogManager.GetLogger<Project>();
        private static readonly List<string> KnownArgs = new List<string>();
        private readonly DirectoryInfo info;
        private readonly ProjectInfo projectInfo;
        private readonly ProjectContext projectContext;
        private readonly IPluginContext pluginContext;
        public UMenuItem MenuItem { get; private set; }

        public Project(ProjectInfo projectInfo, ProjectContext projectContext, IPluginContext pluginContext)
        {
            info = new DirectoryInfo(pluginContext.PathResolver.Resolve(projectInfo.Key));
            this.projectInfo = projectInfo;
            this.projectContext = projectContext;
            this.pluginContext = pluginContext;
            CreateMenu();
        }

        private void CreateMenu()
        {
            MenuItem = new UMenuItem { Header = CreateHeader() };
            AppendBuildSubMenu();
            AppendSeparator();
            AppendSvnSubMenu();
            AppendSeparator();
            AppendBatSubMenu();
            AppendMsBuildSubMenu();
        }

        private void AppendSeparator()
        {
            MenuItem.Items.Add(new USeparator());
        }

        private string CreateHeader()
        {
            var header = info.Name;
            if (info.Parent != null)
            {
                header = info.Parent.Name + "/" + header;
            }
            return header;
        }

        private void AppendItem(string caption, Action<object> onClick, Icon icon = null)
        {
            MenuItem.Items.Add(new UMenuItem
            {
                Header = caption,
                Icon = icon,
                OnClick = onClick
            });
        }

        private void AppendBuildItem(string caption, string path, string mode)
        {
            AppendItem($"{caption} - {mode}",
                o => projectContext.MsBuildProvider.BuildSlnFile(mode, path),
                pluginContext.GetIcon(projectContext.MsBuildProvider.PathToMsBuild, 0)
                );
        }

        private void AppendBuildSubMenu()
        {
            var files = info.GetFiles("*.sln", SearchOption.TopDirectoryOnly);
            if (!files.Any()) return;
            foreach (var item in files)
            {
                AppendBuildItem(item.Name, item.FullName, "Debug");
                AppendBuildItem(item.Name, item.FullName, "Release");
            }
        }

        private void AppendSvnItem(string caption, string command, int id)
        {
            AppendItem(caption, o => projectContext.SvnProvider.Do(command, info.FullName),
                pluginContext.GetIcon(projectContext.SvnProvider.Path, id));
        }

        private void AppendMergeItem(string caption, string command, int id)
        {
            AppendItem(caption, o => projectContext.SvnProvider.Merge(command, info.FullName),
                pluginContext.GetIcon(projectContext.SvnProvider.Path, id));
        }

        private void AppendSvnSubMenu()
        {
            var end = "...";
            AppendSvnItem(ProjectManLang.Update + end, "update", 3);
            AppendSvnItem(ProjectManLang.Commit + end, "commit", 4);
            AppendSvnItem(ProjectManLang.ShowLog + end, "log", 17);
            AppendSeparator();
            AppendSvnItem(ProjectManLang.RepoBrowser + end, "repobrowser", 106);
            AppendSvnItem(ProjectManLang.Resolve + end, "resolve", 8);
            AppendSvnItem(ProjectManLang.UpdateToRevision + end, "update /rev", 3);
            AppendSvnItem(ProjectManLang.Revert + end, "revert", 6);
            AppendSvnItem(ProjectManLang.CleanUp + end, "cleanup", 7);
            AppendSeparator();
            AppendSvnItem(ProjectManLang.BranchTag + end, "copy", 13);
            AppendSvnItem(ProjectManLang.Switch + end, "switch", 9);
            AppendSvnItem(ProjectManLang.Merge + end, "merge", 10);
            AppendSvnItem(ProjectManLang.Export + end, "export", 12);
            AppendSvnItem(ProjectManLang.Relocate + end, "relocate", 22);
            AppendSeparator();
            AppendSvnItem(ProjectManLang.CreatePatch + end, "createpatch", 109);
            AppendMergeItem(ProjectManLang.ApplyPatch + end, "patchpath", 108);
            AppendSvnItem(ProjectManLang.Properties + end, "properties", 117);
        }

        private IEnumerable<Pair<string, string>> GetFiles(string ext)
        {
            var files =
                info.GetFiles(ext).
                Select(i => new Pair<string, string>(i.Name, i.FullName));
            foreach (var dir in info.GetDirectories())
            {
                var dirName = dir.Name;
                files = files.Union(
                    dir.GetFiles(ext).
                    Select(i =>
                        new Pair<string, string>(dirName + "\\" + i.Name, i.FullName)));
            }
            return files;
        }

        private void AppendBatSubMenu()
        {
            var files = GetFiles("*.bat").
                Union(GetFiles("*.cmd")).
                OrderBy(x => x.Key);
            if (!files.Any()) return;
            var item = AddNewMenuItem(ProjectManLang.Batch, pluginContext.GetIcon("c:\\windows\\system32\\cmd.exe", 0));
            foreach (var m in files)
            {
                var path = m.Value;
                item.Items.Add(new UMenuItem
                {
                    Header = m.Key,
                    OnClick = o => Cmd.Start(path, Log, string.Empty, false)
                });
            }
        }

        private void AppendMsBuildSubMenu()
        {
            var files = GetFiles("*.build").OrderBy(x => x.Key);
            if (!files.Any()) return;
            var item = AddNewMenuItem(ProjectManLang.MsBuild, pluginContext.GetIcon(projectContext.MsBuildProvider.PathToMsBuild, 0));
            foreach (var m in files)
            {
                var key = m.Key;
                var path = m.Value;
                item.Items.Add(new UMenuItem
                {
                    Header = key,
                    OnClick = o => BuildBuildFile(key, path, o)
                });
            }
        }

        private void BuildBuildFile(string key, string path, object context)
        {
            if (!(context is NonUserRunContext))
            {
                var r = DialogsCache.ShowInputComboBox(ProjectManLang.PleaseSpecifyBuildArguments, ProjectManLang.Build + " " + key, projectInfo.MsBuildParams, x => true, KnownArgs, null, false);
                if (!r.Key) return;
                projectInfo.MsBuildParams = r.Value;
            }
            projectContext.MsBuildProvider.BuildBuildFile(path, projectInfo.MsBuildParams);
        }

        private UMenuItem AddNewMenuItem(string header, Icon icon)
        {
            var item = new UMenuItem
            {
                Header = header,
                Icon = icon
            };
            MenuItem.Items.Add(item);
            return item;
        }
    }
}
