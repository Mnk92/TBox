using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;

namespace Mnk.TBox.Plugins.TeamManager.Code.Scripts
{
	sealed class ValidatorScriptConfigurator : IScriptConfigurator
	{
        private readonly IScriptCompiler<IDayStatusStrategy> compiler = new ScriptCompiler<IDayStatusStrategy>();

	    public ScriptPackage GetParameters(string scriptText)
	    {
            return compiler.GetPackages(scriptText);
	    }
	}
}
