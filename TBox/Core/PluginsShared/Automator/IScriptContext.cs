using System;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.PluginsShared.Automator
{
    public interface IScriptContext
    {
        IUpdater Updater { get; }
        IPathResolver PathResolver { get; }
        Action<Action> Sync { get; }
    }
}
