using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    public class SkyNetScriptConfigurator : IScriptConfigurator
    {
        private readonly IScriptCompiler<ISkyScript> compiler = new ScriptCompiler<ISkyScript>();

        public ScriptPackage GetParameters(string scriptText)
        {
            return compiler.GetPackages(scriptText);
        }
    }
}
