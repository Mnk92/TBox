using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightInject;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.TBox.Plugins.SkyNet.Code;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Plugins.SkyNet.Code.Settings;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;
using Mnk.TBox.Plugins.SkyNet.Forms;

namespace Mnk.TBox.Plugins.SkyNet
{
    [PluginInfo(typeof(SkyNetLang), 18, PluginGroup.Development)]
    public class SkyNet : ConfigurablePlugin<Settings, Config>
    {
        private IServiceContainer container;

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            container = ServicesRegistrar.Register(context, ()=>Icon.ToImageSource());
            Dialogs.Add(container.GetInstance<LazyDialog<EditorDialog>>());
            Dialogs.Add(container.GetInstance<LazyDialog<TaskDialog>>());
        }

        public override void OnRebuildMenu()
        {
            Menu = Config.Operations.CheckedItems.Select(
                x => new UMenuItem
                {
                    IsEnabled = !string.IsNullOrEmpty(x.Path),
                    Header = x.Key,
                    OnClick = o => DoExecute(x)
                })
                .Concat(
                    new[]
			        {
                        new USeparator(), 
				        new UMenuItem
				        {
					        Header = SkyNetLang.ScriptEditor,
					        OnClick = OpenEditor
				        }
			        })
                .ToArray();
        }

        private void DoExecute(SingleFileOperation operation)
        {
            container.GetInstance<LazyDialog<TaskDialog>>()
                .Do(Context.DoSync, x => x.ShowDialog(operation), Config.States);
        }

        private void OpenEditor(object o)
        {
            container.GetInstance<LazyDialog<EditorDialog>>()
                .Do(Context.DoSync, x => x.ShowDialog(
                    container.GetInstance<IScriptsHelper>().GetPaths(), 
                    container.GetInstance<IScriptConfigurator>()), 
                    Config.States);
        }

        protected override Settings CreateSettingsInstance()
        {
            return (Settings)container.GetInstance<ISettings>();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            container.Dispose();
        }
    }

}
