using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.SaveLoad;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false)]
    public class ConfigProvider<T> : IConfigProvider<T>
        where T: new()
    {
        private readonly ConfigurationSerializer<T> serializer;
        private readonly ILog log = LogManager.GetLogger<ConfigProvider<T>>();
        public T Config { get; private set; }
        
        public ConfigProvider(string path)
        {
            serializer = new ConfigurationSerializer<T>(path);
            Config = serializer.Load(new T());
        }

        public T ReceiveConfig()
        {
            return Config;
        }

        public void UpdateConfig(T config)
        {
            try
            {
                serializer.Save(config);
                Config = config;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't save config");
                if (WebOperationContext.Current != null)
                {
                    WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
            }
        }
    }
}
