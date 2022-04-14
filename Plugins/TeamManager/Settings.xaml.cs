using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;
using Mnk.Library.WpfControls.Code;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.TeamManager
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public sealed partial class Settings : ISettings, IDisposable
    {
        public LazyDialog<ScriptsConfigurator> ScriptConfiguratorDialog { get; set; }

        public Settings()
        {
            InitializeComponent();
        }

        public UserControl Control { get { return this; } }
        public IList<string> FilePaths { get; set; }
        internal IScriptConfigurator ScriptConfigurator { get; set; }
        internal Config Config { get { return ((Config)DataContext); } }

        private void BtnSetParamtersClick(object sender, RoutedEventArgs e)
        {
            var op = GetSelectedOperation(sender);
            if (string.IsNullOrEmpty(op.Path))
            {
                MessageBox.Show(TeamManagerLang.PleaseSpecifyScriptPath);
                return;
            }
            ScriptConfiguratorDialog.Value.ShowDialog(op, ScriptConfigurator, this.GetParentWindow());
        }

        private SingleFileOperation GetSelectedOperation(object sender)
        {
            var selectedKey = ((Button) sender).DataContext.ToString();
            var profile = (Profile)Profile.SelectedValue;
            var id = profile.Operations.GetExistIndexByKeyIgnoreCase(selectedKey);
            return profile.Operations[id];
        }

        public void Dispose()
        {
            ScriptConfiguratorDialog.Dispose();
        }

        private void SelectTeamMembersClick(object sender, RoutedEventArgs e)
        {
            var items = (Collection<Data>)((Button)sender).DataContext;
            var profile = (Profile)Profile.SelectedValue;
            DialogsCache.ShowInputListUnit(TeamManagerLang.ConfigurePersons, TeamManagerLang.Persons, 
                items, profile.Persons.Select(x => x.Key).ToList(), 
                this.GetParentWindow());
        }
    }
}
