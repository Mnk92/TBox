using System.Text.Json;
using Grpc.Core;
using Microsoft.Toolkit.HighPerformance;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using ScriptEngine.Core.Params;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Processing
{
    class Worker : IWorker
    {
        private readonly IScriptCompiler<ISkyScript> compiler;
        private readonly ISkyAgentLogic agentLogic;
        private readonly IDataPacker dataPacker;
        private readonly ISkyNetFileService.ISkyNetFileServiceClient skyNetFileService;

        public Worker(IScriptCompiler<ISkyScript> compiler, ISkyAgentLogic agentLogic, IDataPacker dataPacker, ISkyNetFileService.ISkyNetFileServiceClient skyNetFileService)
        {
            this.compiler = compiler;
            this.agentLogic = agentLogic;
            this.dataPacker = dataPacker;
            this.skyNetFileService = skyNetFileService;
        }

        public async Task ProcessTask(ServerTask task, IList<ServerAgent> agents)
        {
            var script = compiler.Compile(task.Script, JsonSerializer.Deserialize<IList<Parameter>>(task.ScriptParameters));
            var items = await StartAgents(task, script, agents);
            if (!items.Any())
            {
                throw new ArgumentException("Please divide tasks, 0 agents is not applicable.");
            }

            await WaitAgents(task, items);

            task.Report = await BuildResult(script, items);
        }

        private async Task<WorkerTask[]> StartAgents(ServerTask task, ISkyScript script, IList<ServerAgent> agents)
        {
            var path = string.Empty;
            if (!string.IsNullOrEmpty(task.ZipPackageId))
            {
                using var result = skyNetFileService.Download(new IdMessage { Id = task.ZipPackageId });
                using (var s = result.ResponseStream.Current.Data.Memory.AsStream())
                {
                    path = dataPacker.Unpack(s);
                }
            }
            try
            {
                var items = await Task.WhenAll(script.ServerBuildAgentsData(path, agents)
                    .AsParallel()
                    .Select(item => agentLogic.CreateWorkerTask(item.Agent, item.Config, task))
                    .ToArray());
                return items;
            }
            finally
            {
                if (Directory.Exists(path)) Directory.Delete(path, true);
            }
        }

        private async Task WaitAgents(ServerTask task, IList<WorkerTask> items)
        {
            while (true)
            {
                var tasks = await Task.WhenAll(items.AsParallel().Select(x => agentLogic.GetTask(x)).ToArray());
                task.Progress = tasks.Sum(GetProgress) / tasks.Length;
                if (tasks.All(x => x.IsDone || x.IsCanceled)) return;
                Thread.Sleep(3000);
            }
        }

        private static int GetProgress(AgentTask task)
        {
            if (task == null || task.IsCanceled || task.IsDone) return 100;
            return task.Progress;
        }

        private async Task<string> BuildResult(ISkyScript script, IEnumerable<WorkerTask> items)
        {
            var results = await Task.WhenAll(items
                .AsParallel()
                .Select(agentLogic.BuildReport)
                .ToArray());
            return script.ServerBuildResultByAgentResults(results);
        }
    }
}
