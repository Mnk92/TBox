using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Toolkit.HighPerformance;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetFileService : ISkyNetFileService.ISkyNetFileServiceBase
    {
        private readonly string rootFolder;
        private readonly ILog log = LogManager.GetLogger<SkyNetFileService>();

        public SkyNetFileService()
        {
            rootFolder = Path.Combine(Path.GetTempPath(), "TBox.SkyNet.Files");
            if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);
        }

        public override Task<IdMessage> Upload(IAsyncStreamReader<StreamMessage> requestStream, ServerCallContext context)
        {
            using var stream = requestStream.Current.Data.Memory.AsStream();
            var id = Guid.NewGuid().ToString();
            var path = GetFilePath(id);
            var tmpPath = path + ".tmp";
            try
            {
                var fileInfo = new FileInfo(tmpPath);
                using (var f = fileInfo.Create())
                {
                    stream.CopyTo(f);
                }
                fileInfo.MoveTo(path);
                return Task.FromResult(new IdMessage { Id = id });
            }
            catch (Exception)
            {
                try
                {
                    if (File.Exists(path)) File.Delete(path);
                    if (File.Exists(tmpPath)) File.Delete(tmpPath);
                }
                catch (Exception fex)
                {
                    log.Write(fex, "Can't delete broken file: " + id);
                }
                throw;
            }
        }

        public override async Task Download(IdMessage request, IServerStreamWriter<StreamMessage> responseStream, ServerCallContext context)
        {
            var info = GetFileInfo(request.Id);
            await using var stream = info.OpenRead();
            await responseStream.WriteAsync(new StreamMessage { Data = await ByteString.FromStreamAsync(stream) });
        }

        public override Task<Empty> Delete(
            IdMessage request, ServerCallContext context)
        {
            GetFileInfo(request.Id).Delete();
            return Task.FromResult(new Empty());
        }

        private FileInfo GetFileInfo(string id)
        {
            var fileInfo = new FileInfo(GetFilePath(id));
            if (fileInfo.Exists == false)
            {
                throw new FileNotFoundException("File not found", id);
            }
            return fileInfo;
        }

        private string GetFilePath(string id)
        {
            return Path.Combine(rootFolder, id);
        }
    }
}
