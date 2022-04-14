using System;
using System.Windows;
using System.Windows.Media;
using Mnk.Library.WpfControls;
using Mnk.TBox.Locales.Localization.Plugins.SqlRunner;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;
using Mnk.Library.WpfControls.Code;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfSyntaxHighlighter;

namespace Mnk.TBox.Plugins.SqlRunner.Code
{
    sealed class Executor : BaseExecutor, IDisposable
    {
        private readonly LazyDialog<MemoBox> message = new LazyDialog<MemoBox>(() => new MemoBox());

        public void Execute(Window owner, Op operation, string connectionString, Config config, Action onEnd, ImageSource icon)
        {
            if (message.IsVisible) message.Hide();
            message.Value.Icon = icon;
            base.Execute(owner, operation, connectionString, i => Finish(i, owner, operation.Key, config, onEnd), icon);
        }

        private void Finish(DatabaseInfo response, Window owner, string name, Config config, Action onEnd)
        {
            Mt.Do(owner, () =>
            {
                if (onEnd != null) onEnd();
                message.Do(x => Mt.Do(owner, x),
                    x => x.ShowDialog(string.Format(SqlRunnerLang.RequestResultsTemplate, name, response.Time / 1000.0),
                    BuildMessage(response), string.Empty, owner),
                    config.States
                    );
            });
        }

        private static string BuildMessage(DatabaseInfo response)
        {
            return response.Status + Environment.NewLine + response.Body;
        }

        public void Close()
        {
            Mt.Do(message.Value, message.Hide);
        }

        public void Save(Config config)
        {
            message.SaveState(config.States);
        }

        public void Dispose()
        {
            message.Dispose();
        }
    }
}
