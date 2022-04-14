using System;
using System.Collections.Generic;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.TBox.Locales.Localization.PluginsShared;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Core.PluginsShared.Automator
{
    public class ScriptsRunner : MultiFileScriptConfigurator
    {
        private readonly IPathResolver pathResolver;

        public ScriptsRunner(IPathResolver pathResolver)
        {
            this.pathResolver = pathResolver;
            btnAction.Content = PluginsSharedLang.Run;
        }

        protected override void DoAction()
        {
            base.DoAction();
            DialogsCache.ShowProgress(u => Work(u, GetParameters()),
                PluginsSharedLang.Execute + ":" + Title, null, topmost: false, showInTaskBar: true);
        }

        private IList<Parameter> GetParameters()
        {
            return Config.Parameters;
        }

        private void Work(IUpdater u, IList<Parameter> parameters)
        {
            try
            {
                DoWork(u, parameters);
            }
            catch (CompilerExceptions cex)
            {
                Log.Write(cex.ToString());
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Error running script");
            }
        }

        private void DoWork(IUpdater u, IList<Parameter> parameters)
        {
            var i = 0;
            var compiler = new ScriptCompiler<IScript>();
            var ctx = new ScriptContext(pathResolver){Updater = u,Sync = a=>Mt.Do(this, a)};
            foreach (var context in ScriptsPackages)
            {
                if (u.UserPressClose) return;
                u.Update(i++ / (float)ScriptsPackages.Count);
                var s = compiler.Compile(context.SourceText, parameters);
                s.Run(ctx);
            }
        }
    }
}
