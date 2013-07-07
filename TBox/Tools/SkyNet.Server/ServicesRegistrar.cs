using LightInject;
using Mnk.Library.Common.Communications;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    static class ServicesRegistrar
    {
        public static IServiceContainer Register(ServerConfig config)
        {
            var container = new ServiceContainer();
            container.Register<IServerContext, ServerContext>(new PerContainerLifetime());
            container.Register<IWorker, Worker>(new PerContainerLifetime());
            container.Register<ISkyAgentLogic, SkyAgentLogic>(new PerContainerLifetime());
            container.Register<ISkyNetServerAgentsLogic, SkyNetServerAgentsLogic>(new PerContainerLifetime());
            container.Register<ISkyNetServerTasksLogic, SkyNetServerTasksLogic>(new PerContainerLifetime());
            container.Register<ISkyNetFileServiceLogic, SkyNetFileServiceLogic>(new PerContainerLifetime());
            container.Register<IHttpContextHelper, HttpContextHelper>(new PerContainerLifetime());
            container.Register<ISkyNetCommon, SkyNetServerFacade>(new PerContainerLifetime());
            container.Register<IScriptCompiler<ISkyScript>, ScriptCompiler<ISkyScript>>(new PerContainerLifetime());
            container.Register<IAgentsCache, AgentsCache>(new PerContainerLifetime());
            container.Register<IModulesRunner, ModulesRunner>(new PerContainerLifetime());
            container.Register<IModule, HandleDiedAgents>("HandleDiedAgents", new PerContainerLifetime());
            container.Register<IModule, IdleTasksProcessorModule>("IdleTasksProcessorModule", new PerContainerLifetime());
            container.Register<IDataPacker, DataPacker>(new PerContainerLifetime());

            container.RegisterInstance(config);

            return container;
        }
    }
}
