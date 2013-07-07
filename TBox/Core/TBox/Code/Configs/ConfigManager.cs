using System;
using System.Globalization;
using System.IO;
using Mnk.Library.Common.SaveLoad;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.Application.Code.AutoUpdate;
using Mnk.Library.WpfControls.Localization;

namespace Mnk.TBox.Core.Application.Code.Configs
{
    class ConfigManager : IConfigManager<Config>, IConfigsManager
    {
        public Config Config { get; private set; }
        private ConfigurationSerializer<Config> configurationSer;
        private const string ConfigFileName = "Config.config";
        private readonly string localFolder = Folders.LocalFolder;
        private readonly string userFolder = Folders.UserRootFolder;

        public ConfigManager()
        {
            var localConfig = Path.Combine(localFolder, ConfigFileName);
            var userConfig = Path.Combine(userFolder, ConfigFileName);
            if (!TryLoad(localConfig, userConfig, true))
            {
                if (!TryLoad(userConfig, localConfig, false))
                {
                    configurationSer = new ConfigurationSerializer<Config>(userConfig);
                    Config = configurationSer.Load(Config = new Config());
                }
            }
            var localUpdater = CreateUpdater();
            localUpdater.PrepareConfigs();

            Translator.Culture = new CultureInfo(Config.Language);
        }

        private PartatabilityProvider CreateUpdater()
        {
            return Config.Configuration.PortableMode ?
                new PartatabilityProvider(Root = localFolder, userFolder) :
                new PartatabilityProvider(Root = userFolder, localFolder);
        }

        private bool TryLoad(string targetConfig, string alternativeConfig, bool portability)
        {
            if (File.Exists(targetConfig))
            {
                configurationSer = new ConfigurationSerializer<Config>(targetConfig);
                Config = configurationSer.Load(Config = new Config());
                if (Config.Configuration.PortableMode != portability)
                {
                    configurationSer = new ConfigurationSerializer<Config>(alternativeConfig);
                }
                return true;
            }
            return false;
        }

        public void Save()
        {
            configurationSer.Save(Config);
        }

        public void Load()
        {
            Config = configurationSer.Load(Config = new Config());
        }

        public string Root { get; private set; }
    }
}
