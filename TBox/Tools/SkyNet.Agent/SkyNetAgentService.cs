using System;
using System.ServiceProcess;
using LightInject;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    public partial class SkyNetAgentService : ServiceBase
    {
        private readonly ILog log = LogManager.GetLogger<SkyNetAgentService>();
        private readonly AgentConfig config;
        private readonly IServiceContainer container;

        public SkyNetAgentService(AgentConfig config, IServiceContainer container)
        {
            this.config = config;
            this.container = container;
            InitializeComponent();
        }

        private NetworkServer<ISkyNetAgentService> server;

        protected override void OnStart(string[] args)
        {
            OnStop();
            try
            {
                server = new NetworkServer<ISkyNetAgentService>(container.GetInstance<ISkyNetAgentService>(), config.Port);
                container.GetInstance<IModulesRunner>().Start();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't start");
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                container.GetInstance<IModulesRunner>().Stop();
                if (server == null) return;
                server.Dispose();
                server = null;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't stop");
                throw;
            }
        }

        public void StartService()
        {
            OnStart(new string[0]);
        }

        public void StopService()
        {
            Stop();
        }
    }
}
