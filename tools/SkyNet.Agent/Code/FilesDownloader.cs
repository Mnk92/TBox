using Grpc.Net.Client;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Microsoft.Toolkit.HighPerformance;

namespace Mnk.TBox.Tools.SkyNet.Agent.Code
{
    class FilesDownloader : IFilesDownloader
    {
        private readonly ConfigProvider<AgentConfig> configProvider;
        private readonly IDataPacker dataPacker;

        public FilesDownloader(ConfigProvider<AgentConfig> configProvider, IDataPacker dataPacker)
        {
            this.configProvider = configProvider;
            this.dataPacker = dataPacker;
        }

        public string DownloadAndUnpackFiles(string zipPackageId)
        {
            if (string.IsNullOrEmpty(zipPackageId)) return string.Empty;
            var serverEndpoint = configProvider.Config.ServerEndpoint;
            using var channel = GrpcChannel.ForAddress(serverEndpoint);
            var client = new ISkyNetFileService.ISkyNetFileServiceClient(channel);
            using var s = client.Download(new IdMessage { Id = zipPackageId }).ResponseStream.Current.Data.Memory.AsStream();
            return dataPacker.Unpack(s);
        }
    }
}
