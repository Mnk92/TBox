using System.IO;
using System.Text.Json;
using Mnk.Library.ScriptEngine;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class TaskExecutor : ITaskExecutor
    {
        private readonly IDataPacker dataPacker;
        private readonly IScriptCompiler<ISkyScript> compiler;
        private readonly IPluginContext context;
        private readonly IServicesFacade servicesFacade;
        private readonly IConfigurationProviderFacade configurationProviderFacade;

        public TaskExecutor(IServicesFacade servicesFacade, IConfigurationProviderFacade configurationProviderFacade, IDataPacker dataPacker, IScriptCompiler<ISkyScript> compiler, IPluginContext context)
        {
            this.servicesFacade = servicesFacade;
            this.configurationProviderFacade = configurationProviderFacade;
            this.dataPacker = dataPacker;
            this.compiler = compiler;
            this.context = context;
        }

        public async Task<TaskInfo> Execute(SingleFileOperation operation)
        {
            var config = await configurationProviderFacade.GetAgentConfig();
            var scriptContent = await File.ReadAllTextAsync(Path.Combine(context.DataProvider.ReadOnlyDataPath, operation.Path));
            var script = compiler.Compile(scriptContent, operation.Parameters);
            var task = new ServerTask
            {
                Owner = config.Name,
                Script = scriptContent,
                ScriptParameters = JsonSerializer.Serialize(operation.Parameters),
                ZipPackageId = await PackData(script)
            };
            return new TaskInfo
            {
                Id = await servicesFacade.StartTask(task),
                ZipPackageId = task.ZipPackageId
            };
        }

        private async Task<string> PackData(ISkyScript script)
        {
            if (string.IsNullOrEmpty(script.DataFolderPath)) return string.Empty;
            var path = dataPacker.Pack(script.DataFolderPath, script.PathMasksToInclude);
            var zipPackageId = await servicesFacade.UploadFile(path);
            File.Delete(path);
            return zipPackageId;
        }
    }
}
