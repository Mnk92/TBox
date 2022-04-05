using System;
using System.IO;
using System.IO.Compression;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Tools.SkyNet.Common.Modules
{
    public class DataPacker : IDataPacker
    {
        private readonly ILog log = LogManager.GetLogger<DataPacker>();
        public string Unpack(Stream stream)
        {
            var zipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            try
            {
                using (var fs = File.Create(zipPath))
                {
                    stream.CopyTo(fs);
                }
                ZipFile.ExtractToDirectory(zipPath, folderPath);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't unpack data");
                if (Directory.Exists(folderPath)) Directory.Delete(folderPath, true);
                throw;
            }
            finally
            {
                if (File.Exists(zipPath)) File.Delete(zipPath);
            }
            return folderPath;
        }
    }
}
