using System;
using System.IO;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.Application.Code.AutoUpdate
{
    class PartatabilityProvider
    {
        private const string Config = "Config";
        private const string ConfigFile = "Config.config";
        private readonly string targetPath;
        private readonly string oldPath;
        private static readonly ILog Log = LogManager.GetLogger<Engine>();

        public PartatabilityProvider(string targetPath, string oldPath)
        {
            this.targetPath = targetPath;
            this.oldPath = oldPath;
        }

        public void PrepareConfigs()
        {
            try
            {
                new FileInfo(Path.Combine(oldPath, ConfigFile))
                    .MoveIfExist(Path.Combine(targetPath, ConfigFile));
            }
            catch { }

            try
            {
                new DirectoryInfo(Path.Combine(oldPath, Config))
                    .MoveIfExist(Path.Combine(targetPath, Config));
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't migrate configuration files");
            }
        }
    }
}
