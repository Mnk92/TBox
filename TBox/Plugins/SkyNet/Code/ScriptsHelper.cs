using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class ScriptsHelper : IScriptsHelper
    {
        private readonly IPluginContext context;
        private readonly IScriptConfigurator scriptConfigurator;
        private readonly LazyDialog<ScriptsConfigurator> scriptConfiguratorDialog;

        public ScriptsHelper(IPluginContext context, IScriptConfigurator scriptConfigurator, LazyDialog<ScriptsConfigurator> scriptConfiguratorDialog)
        {
            this.context = context;
            this.scriptConfigurator = scriptConfigurator;
            this.scriptConfiguratorDialog = scriptConfiguratorDialog;
        }

        public IList<string> GetPaths()
        {
            var dir = new DirectoryInfo(context.DataProvider.ReadOnlyDataPath);
            if (!dir.Exists) return new string[0];
            var length = dir.FullName.Length + 1;
            return dir
                .EnumerateFiles("*.cs", SearchOption.AllDirectories)
                .Select(x => x.FullName.Substring(length))
                .ToArray();

        }

        public void ShowParameters(SingleFileOperation operation)
        {
            if (string.IsNullOrEmpty(operation.Path))
            {
                MessageBox.Show(SkyNetLang.PleaseSpecifyScriptPath);
                return;
            }
            scriptConfiguratorDialog.Do(context.DoSync, x => x.ShowDialog(operation, scriptConfigurator, Application.Current.MainWindow));
        }
    }
}
