using System;
using System.Collections.Generic;
using Mnk.Library.Common.MT;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Core.PluginsShared.ScriptEngine
{
    public interface IScriptRunner : IScriptConfigurator
	{
        void Run(string path, IList<Parameter> parameters, Action<Action> dispatcher, IUpdater u);
	}
}
