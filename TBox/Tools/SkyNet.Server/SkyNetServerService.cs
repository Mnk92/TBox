using System;
using System.ServiceProcess;
using LightInject;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    public partial class SkyNetServerService : ServiceBase
    {
        private readonly ILog log = LogManager.GetLogger<SkyNetServerService>();
        private readonly ServerConfig config;
        private readonly IServiceContainer container;
        private IDisposable server;

        public SkyNetServerService(ServerConfig config, IServiceContainer container)
        {
            this.config = config;
            this.container = container;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            OnStop();
            try
            {
                server = new NetworkServer<ISkyNetCommon>(container.GetInstance<ISkyNetCommon>(), config.Port);
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
                StopService(ref server);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't stop");
                throw;
            }
        }

        private void StopService(ref IDisposable o)
        {
            if (o == null) return;
            o.Dispose();
            o = null;
        }

        public void StartService()
        {
            OnStart(new string[0]);
        }

        public void StopService()
        {
            OnStop();
        }
    }
}
