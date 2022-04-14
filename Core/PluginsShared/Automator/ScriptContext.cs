using System;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.PluginsShared.Automator
{
    public class ScriptContext : IScriptContext
    {
        public IPathResolver PathResolver { get; private set; }
        public IUpdater Updater { get; set; }
        public Action<Action> Sync { get; set; }

        public ScriptContext(IPathResolver pathResolver)
        {
            PathResolver = pathResolver;
            Updater = new ConsoleUpdater();
            Sync = a => a();
        }
    }
}
