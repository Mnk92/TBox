using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mnk.Library.Common.Network;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.Tools;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Core.Contracts;

namespace Solution.Scripts
{
    public class RunDevServers : IScript
    {
        [File("c:/")]
        public string PathToDevServer { get; set; }

        [IntegerDictionary("pathtoserver", 49163, Min = 0, Max = 65536)]
        public IDictionary<string, int> DevProjects { get; set; }

        [IntegerList(401, 403)]
        public IList<int> AllowedHttpStatusCodes { get; set; }

        [Integer(240)]
        public int TimeOut { get; set; }


        public void Run(IScriptContext context)
        {
            Parallel.ForEach(DevProjects, x => Work(x, context.PathResolver));
        }

        private void Work(KeyValuePair<string, int> project, IPathResolver pathResolver)
        {
            var path = pathResolver.Resolve(project.Key);
            Console.WriteLine("Start dev server: " + path);
            new CassiniRunner().Run(path, project.Value, "/", false, PathToDevServer, "", false);
            Console.WriteLine("Open page: " + path);
            var r = new Request().GetResult("http://localhost:" + project.Value, timeout: TimeOut * 1000);
            Console.WriteLine("Page '{0}' time: {1}", path, r.Time);
            if (r.HttpStatusCode == HttpStatusCode.OK || AllowedHttpStatusCodes.Contains((int)r.HttpStatusCode)) return;
            Console.WriteLine("Body: {0}", r.Body);
            throw new ArgumentException("Can't open: " + path);
        }
    }
}
