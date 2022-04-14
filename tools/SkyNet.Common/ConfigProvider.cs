using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.SaveLoad;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    public class ConfigSettings<T>
    {
        public string Path { get; set; }
        public T Config { get; set; }
    }

    public class ConfigProvider<T> : IConfigProvider.IConfigProviderBase
        where T : new()
    {
        private readonly ConfigSettings<T> settings;
        private readonly ConfigurationSerializer<T> serializer;
        private readonly ILog log = LogManager.GetLogger<ConfigProvider<T>>();
        public T Config => settings.Config;

        public ConfigProvider(ConfigSettings<T> settings)
        {
            this.settings = settings;
            serializer = new ConfigurationSerializer<T>(settings.Path);
            settings.Config = serializer.Load(settings.Config);
        }

        public override Task<ConfigMessage> ReceiveConfig(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ConfigMessage { Config = JsonSerializer.Serialize<T>(settings.Config) });
        }

        public override Task<Empty> UpdateConfig(ConfigMessage request, ServerCallContext context)
        {
            try
            {
                var config = JsonSerializer.Deserialize<T>(request.Config);
                serializer.Save(config);
                settings.Config = config;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't save config");
            }
            return Task.FromResult(new Empty());
        }
    }
}
