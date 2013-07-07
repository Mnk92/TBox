using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.Tools;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
    public class StopDevServers : IScript
    {
        [File("c:/")]
        public string PathToDevServer { get; set; }

        public void Run(IScriptContext context)
        {
            new CassiniRunner().StopAll(PathToDevServer);
        }
    }
}
