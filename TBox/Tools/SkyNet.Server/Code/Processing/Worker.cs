using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ServiceStack.Text;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Processing
{
    class Worker : IWorker
    {
        private readonly IScriptCompiler<ISkyScript> compiler;
        private readonly ISkyAgentLogic agentLogic;
        private readonly IDataPacker dataPacker;
        private readonly ISkyNetFileServiceLogic skyNetFileService;

        public Worker(IScriptCompiler<ISkyScript> compiler, ISkyAgentLogic agentLogic, IDataPacker dataPacker, ISkyNetFileServiceLogic skyNetFileService)
        {
            this.compiler = compiler;
            this.agentLogic = agentLogic;
            this.dataPacker = dataPacker;
            this.skyNetFileService = skyNetFileService;
        }

        public void ProcessTask(ServerTask task, IList<ServerAgent> agents)
        {
            var script = compiler.Compile(task.Script, JsonSerializer.DeserializeFromString<IList<Parameter>>(task.ScriptParameters));
            var items = StartAgents(task, script, agents);
            if (!items.Any())
            {
                throw new ArgumentException("Please divide tasks, 0 agents is not applicable.");
            }

            WaitAgents(task, items);

            task.Report = BuildResult(script, items);
        }

        private WorkerTask[] StartAgents(ServerTask task, ISkyScript script, IList<ServerAgent> agents)
        {
            var path = string.Empty;
            if (!string.IsNullOrEmpty(task.ZipPackageId))
            {
                using (var s = skyNetFileService.Download(task.ZipPackageId))
                {
                    path = dataPacker.Unpack(s);
                }
            }
            try
            {
                var items = script.ServerBuildAgentsData(path, agents)
                    .AsParallel()
                    .Select(item => agentLogic.CreateWorkerTask(item.Agent, item.Config, task))
                    .ToArray();
                return items;
            }
            finally
            {
                if(Directory.Exists(path))Directory.Delete(path,true);
            }
        }

        private void WaitAgents(ServerTask task, IList<WorkerTask> items)
        {
            while(true)
            {
                var tasks = items.AsParallel().Select(x => agentLogic.GetTask(x)).ToArray();
                task.Progress = tasks.Sum(x => GetProgress(x))/tasks.Length;
                if(tasks.All(x => x.IsDone || x.IsCanceled))return;
                Thread.Sleep(3000);
            }
        }

        private static int GetProgress(AgentTask task)
        {
            if (task == null || task.IsCanceled || task.IsDone) return 100;
            return task.Progress;
        }

        private string BuildResult(ISkyScript script, IEnumerable<WorkerTask> items)
        {
            return script.ServerBuildResultByAgentResults(items
                .AsParallel()
                .Select(agentLogic.BuildReport)
                .ToArray()
                );
        }
    }
}
