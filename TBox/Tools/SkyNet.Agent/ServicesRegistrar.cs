using LightInject;
using Mnk.Library.Common.Communications;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Agent.Code.Modules;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    static class ServicesRegistrar
    {
        public static IServiceContainer Register(AgentConfig agentConfig)
        {
            var container = new ServiceContainer();
            container.Register<ISkyContext, SkyContext>(new PerContainerLifetime());
            container.Register<IDataPacker, DataPacker>(new PerContainerLifetime());
            container.Register<IWorker, Worker>(new PerContainerLifetime());
            container.Register<ISkyNetAgentService, SkyNetAgentFacade>(new PerContainerLifetime());
            container.Register<IScriptCompiler<ISkyScript>, ScriptCompiler<ISkyScript>>(new PerContainerLifetime());
            container.Register<IFilesDownloader, FilesDownloader>(new PerContainerLifetime());
            container.Register<ISkyNetAgentLogic, SkyNetAgentLogic>(new PerContainerLifetime());
            container.Register<IHttpContextHelper, HttpContextHelper>(new PerContainerLifetime());
            
            container.Register<IModulesRunner, ModulesRunner>(new PerContainerLifetime());
            container.Register<IModule, ServerConnectionModule>(new PerContainerLifetime());

            container.RegisterInstance(agentConfig);

            return container;
        }
    }
}
