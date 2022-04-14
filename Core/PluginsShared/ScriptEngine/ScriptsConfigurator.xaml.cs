using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.PluginsShared;
using Mnk.Library.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Params;
using Mnk.Library.WpfControls.Dialogs;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Core.PluginsShared.ScriptEngine
{
    /// <summary>
    /// Interaction logic for ScriptsConfigurator.xaml
    /// </summary>
    public abstract partial class ScriptsConfigurator
    {
        protected static readonly ILog Log = LogManager.GetLogger<ScriptsConfigurator>();
        protected Operation Config;
        private IScriptConfigurator configurator;
        protected IList<ScriptPackage> ScriptsPackages;
        private readonly ParametersMerger parametersMerger = new ParametersMerger();
        public IPluginContext Context { get; set; }
        protected ScriptsConfigurator()
        {
            InitializeComponent();
            btnAction.Content = PluginsSharedLang.Configure;
        }

        public void ShowDialog(Operation o, IScriptConfigurator c, Window owner)
        {
            Owner = owner;
            DataContext = Config = o;
            configurator = c;
            Dispatcher.BeginInvoke(new Action(() => ReloadClick(null, null)));
            if(owner!=null)SafeShowDialog();
            else ShowAndActivate();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ActionClick(object sender, RoutedEventArgs e)
        {
            var invalidKeys = string.Join(Environment.NewLine, ParametersValidator.Validate(Config.Parameters));
            if (!string.IsNullOrEmpty(invalidKeys))
            {
                MessageBox.Show(PluginsSharedLang.CantRunInvalidParams + Environment.NewLine + invalidKeys,
                    Title, MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }
            DoAction();
        }

        protected virtual void DoAction()
        {
            Hide();
        }

        protected abstract IEnumerable<string> GetPaths();

        private void ReloadClick(object sender, RoutedEventArgs e)
        {
            Parameters.ItemsSource = null;
            DialogsCache.ShowProgress(DoReload, PluginsSharedLang.Reloading, this, icon:Icon);
        }

        private void DoReload(IUpdater obj)
        {
            try
            {
                ScriptsPackages = GetPaths().Select(x => configurator.GetParameters(File.ReadAllText(x))).ToArray();
                IList<Parameter> parameters = new List<Parameter>();
                parameters = ScriptsPackages.Aggregate(parameters, (current, x) => parametersMerger.Merge(current, x.Parameters));
                Mt.Do(this, () =>
                {
                    Config.Parameters = new ObservableCollection<Parameter>(
                        parametersMerger.Clarify(Config.Parameters, parameters));
                    Parameters.ItemsSource = Config.Parameters;
                });
                return;
            }
            catch (CompilerExceptions cex)
            {
                MessageBox.Show(cex.ToString(), Config.Key, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't initialize script: " + Config.Key);
            }
            Mt.Do(this, Hide);
        }
    }
}
