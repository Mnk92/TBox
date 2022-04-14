using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Mnk.Library.Common.Network;
using Mnk.Library.WpfControls;
using Mnk.TBox.Locales.Localization.Plugins.Requestor;
using Mnk.Library.WpfControls.Code;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Plugins.Requester.Code.Settings;
using Mnk.TBox.Plugins.Requester.Components;

namespace Mnk.TBox.Plugins.Requester.Code
{
    public sealed class Executor : BaseExecutor, IDisposable
    {
        private readonly LazyDialog<RequestDialog> message = new LazyDialog<RequestDialog>(
            () => new RequestDialog());

        public void Execute(Window owner, Op operation, Config config, Action onEnd, ImageSource icon)
        {
            if (message.IsVisible) message.Hide();
            message.Value.Icon = icon;
            base.Execute(owner, operation, i => Finish(i, owner, operation.Key, config, onEnd), icon);
        }

        private void Finish(ResponseInfo response, Window owner, string name, Config config, Action onEnd)
        {
            Mt.Do(owner, () =>
            {
                if (onEnd != null) onEnd();
                message.Do(
                        x => Mt.Do(owner, x),
                        x => x.ShowDialog(string.Format(RequestorLang.RequestTemplate, name, response.Time / 1000.0), response, owner),
                        config.States
                        );
            });
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
