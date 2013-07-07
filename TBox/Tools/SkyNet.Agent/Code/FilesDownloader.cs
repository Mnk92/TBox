using System;
using Mnk.Library.Common.Communications;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;

namespace Mnk.TBox.Tools.SkyNet.Agent.Code
{
    class FilesDownloader : IFilesDownloader
    {
        private readonly AgentConfig config;
        private readonly IDataPacker dataPacker;

        public FilesDownloader(AgentConfig config, IDataPacker dataPacker)
        {
            this.config = config;
            this.dataPacker = dataPacker;
        }

        public string DownloadAndUnpackFiles(string zipPackageId)
        {
            if (string.IsNullOrEmpty(zipPackageId)) return string.Empty;
            using (var cl = new NetworkClient<ISkyNetFileService>(new Uri(config.ServerEndpoint)))
            {
                using (var s = cl.Instance.Download(zipPackageId))
                {
                    return dataPacker.Unpack(s);
                }
            }

        }
    }
}
