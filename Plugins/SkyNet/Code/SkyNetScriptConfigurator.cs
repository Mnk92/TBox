using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code
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
