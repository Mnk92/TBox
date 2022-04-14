using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.PluginsShared;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Core.PluginsShared.ScriptEngine
{
    /// <summary>
    /// Interaction logic for EditorDialog.xaml
    /// </summary>
    public partial class EditorDialog
    {
        private static readonly ILog Log = LogManager.GetLogger<EditorDialog>();
        public IPluginContext Context { get; set; }
        private IScriptConfigurator configurator;
        public EditorDialog()
        {
            InitializeComponent();
        }

        public void ShowDialog(IList<string> paths, IScriptConfigurator c)
        {
            if (!IsVisible)
            {
                configurator = c;
                Files.ItemsSource = paths;
                Files.IsEnabled = Files.Items.Count > 0;
                if (Files.IsEnabled && Files.SelectedIndex == -1) Files.SelectedIndex = 0;
            }
            ShowAndActivate();
        }

        private void OnSelectFile(object sender, SelectionChangedEventArgs e)
        {
            AskToSaveChanges();
            var path = Path.Combine(Context.DataProvider.ReadOnlyDataPath, e.GetNewSelection());
            ExceptionsHelper.HandleException(
                () => Source.Read(path),
                () => "Can't open file:" + path,
                Log);
            Output.Text = string.Empty;
            UpdateTitle();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            AskToSaveChanges();
            base.OnClosing(e);
        }

        private void BuildClick(object sender, RoutedEventArgs e)
        {
            var text = Source.Text;
            DialogsCache.ShowProgress(u => Build(text), PluginsSharedLang.Building, this);
        }

        private void Build(string text)
        {
            var sw = new Stopwatch();
            sw.Start();
            Func<string> time = () => Environment.NewLine + string.Format(PluginsSharedLang.CompilationTimeTemplate, sw.ElapsedMilliseconds);
            try
            {
                configurator.GetParameters(text);
                Mt.SetText(Output, PluginsSharedLang.NoErrors + time());
            }
            catch (CompilerExceptions cex)
            {
                Mt.SetText(Output, cex + time());
            }
            catch (Exception ex)
            {
                Mt.SetText(Output, ExceptionPrinter.Print(ex) + time());
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var path = Path.Combine(Context.DataProvider.ReadOnlyDataPath, Files.Text);
            ExceptionsHelper.HandleException(
                () => File.WriteAllText(path, Source.Text, Encoding.UTF8),
                () => "Can't save file:" + path,
                Log);
            UpdateTitle();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SourcesChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            if (Files.SelectedIndex != -1)
            {
                Title = string.Format("{0} - [ {1}{2} ] ",
                    PluginsSharedLang.SourcesEditor,
                    Files.SelectedValue, Source.IsModified ? "*" : string.Empty);
            }
            else Title = PluginsSharedLang.SourcesEditor;
        }

        private void AskToSaveChanges()
        {
            if (!Source.IsModified) return;
            if( MessageBox.Show(
                PluginsSharedLang.AreYouWantToSaveChanges,
                Title,
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SaveClick(null, null);
            }
            else
            {
                Source.Clear();
            }
        }
    }
}
