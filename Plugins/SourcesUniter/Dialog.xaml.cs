using System.Diagnostics;
using System.IO;
using System.Windows;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.TBox.Locales.Localization.Plugins.SourcesUniter;
using Mnk.TBox.Plugins.SourcesUniter.Code;
using Mnk.Library.WpfControls.Dialogs;

namespace Mnk.TBox.Plugins.SourcesUniter
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private static readonly ILog Log = LogManager.GetLogger<Dialog>();
        private readonly Uniter uniter = new Uniter();
        public Dialog()
        {
            InitializeComponent();
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            var config = (Config)DataContext;
            DialogsCache.ShowProgress(
                u => Work(u, config),
                SourcesUniterLang.ProcessFiles,
                this);
        }

        private void Work(IUpdater updater, Config config)
        {
            ExceptionsHelper.HandleException(
                () =>
                {
                    updater.Update(0);
                    var tmpFile = Path.GetTempFileName();
                    File.WriteAllText(tmpFile, uniter.ProcessFiles(updater, config.Path, config.Extensions.Split(';'), config.RemoveEmptyLines));
                    if (!updater.UserPressClose) Process.Start(config.Editor, tmpFile);
                },
                () => "Internal error",
                Log
                );
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
