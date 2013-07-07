using Mnk.Library.ScriptEngine.Core;

namespace Mnk.TBox.Core.PluginsShared.ScriptEngine
{
    public interface IScriptConfigurator
	{
        ScriptPackage GetParameters(string scriptText);
	}
}
