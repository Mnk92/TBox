using System;
using System.Collections.Generic;
using System.IO;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Core.Contracts;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Plugins.Automator.Code
{
    sealed class ScriptRunner : IScriptRunner
    {
        private readonly IPathResolver pathResolver;
        private readonly IScriptCompiler<IScript> compiler = new ScriptCompiler<IScript>();

        public ScriptRunner(IPathResolver pathResolver)
        {
            this.pathResolver = pathResolver;
        }

        public void Run(string path, IList<Parameter> parameters, Action<Action> dispatcher, IUpdater u)
        {
            var s = compiler.Compile(File.ReadAllText(path), parameters);
            s.Run(new ScriptContext(pathResolver) { Updater = u, Sync = dispatcher });
        }

        public ScriptPackage GetParameters(string scriptText)
        {
            return compiler.GetPackages(scriptText);
        }
    }
}
