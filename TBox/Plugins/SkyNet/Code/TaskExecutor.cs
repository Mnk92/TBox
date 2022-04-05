using System.IO;
using System.Text.Json;
using Mnk.Library.ScriptEngine;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class TaskExecutor : ITaskExecutor
    {
        private readonly IDataPacker dataPacker;
        private readonly IScriptCompiler<ISkyScript> compiler;
        private readonly IPluginContext context;
        private readonly IServicesFacade servicesFacade;
        private readonly IConfigsFacade configsFacade;

        public TaskExecutor(IServicesFacade servicesFacade, IConfigsFacade configsFacade, IDataPacker dataPacker, IScriptCompiler<ISkyScript> compiler, IPluginContext context)
        {
            this.servicesFacade = servicesFacade;
            this.configsFacade = configsFacade;
            this.dataPacker = dataPacker;
            this.compiler = compiler;
            this.context = context;
        }

        public TaskInfo Execute(SingleFileOperation operation)
        {
            var config = configsFacade.AgentConfig;
            var scriptContent = File.ReadAllText(Path.Combine(context.DataProvider.ReadOnlyDataPath, operation.Path));
            var script = compiler.Compile(scriptContent, operation.Parameters);
            var task = new ServerTask
            {
                Owner = config.Name,
                Script = scriptContent,
                ScriptParameters = JsonSerializer.Serialize(operation.Parameters),
                ZipPackageId = PackData(script)
            };
            return new TaskInfo
            {
                Id = servicesFacade.StartTask(task),
                ZipPackageId = task.ZipPackageId
            };
        }

        private string PackData(ISkyScript script)
        {
            if (string.IsNullOrEmpty(script.DataFolderPath)) return string.Empty;
            var path = dataPacker.Pack(script.DataFolderPath, script.PathMasksToInclude);
            var zipPackageId = servicesFacade.UploadFile(path);
            File.Delete(path);
            return zipPackageId;
        }
    }
}
