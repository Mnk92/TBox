using System.Collections.Generic;
using System.IO;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Plugins.TeamManager.Code.Scripts
{
	sealed class ReportScriptRunner : IScriptConfigurator
	{
        private readonly IScriptCompiler<IReportScript> compiler = new ScriptCompiler<IReportScript>();

        public void Run(string path, IReportScriptContext context, ICollection<Parameter> parameters)
        {
            var s = compiler.Compile(File.ReadAllText(path), parameters);
            s.Run(context);
        }

	    public ScriptPackage GetParameters(string scriptText)
	    {
            return compiler.GetPackages(scriptText);
	    }
	}
}
