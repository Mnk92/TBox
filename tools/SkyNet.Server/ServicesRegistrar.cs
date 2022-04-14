using LightInject;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    static class ServicesRegistrar
    {
        public static IServiceContainer Register(ConfigProvider<ServerConfig> provider)
        {
            var container = new ServiceContainer();
            container.Register<IServerContext, ServerContext>(new PerContainerLifetime());
            container.Register<IWorker, Worker>(new PerContainerLifetime());
            container.Register<ISkyAgentLogic, SkyAgentLogic>(new PerContainerLifetime());
            container.Register<IScriptCompiler<ISkyScript>, ScriptCompiler<ISkyScript>>(new PerContainerLifetime());
            container.Register<IAgentsCache, AgentsCache>(new PerContainerLifetime());
            container.Register<IModulesRunner, ModulesRunner>(new PerContainerLifetime());
            container.Register<IModule, HandleDiedAgents>("HandleDiedAgents", new PerContainerLifetime());
            container.Register<IModule, IdleTasksProcessorModule>("IdleTasksProcessorModule", new PerContainerLifetime());
            container.Register<IDataPacker, DataPacker>(new PerContainerLifetime());

            container.RegisterInstance(provider);

            return container;
        }
    }
}
