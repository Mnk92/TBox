using System;
using System.IO;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetFileServiceLogic : ISkyNetFileServiceLogic
    {
        private readonly string rootFolder;
        private readonly ILog log = LogManager.GetLogger<SkyNetFileServiceLogic>();

        public SkyNetFileServiceLogic()
        {
            rootFolder = Path.Combine(Path.GetTempPath(),"TBox.SkyNet.Files");
            if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);
        }

        public string Upload(Stream stream)
        {
            using (stream)
            {
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
                    return id;
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
        }

        public Stream Download(string id)
        {
            var info = GetFileInfo(id);
            return info.OpenRead();
        }

        public void Delete(string id)
        {
            GetFileInfo(id).Delete();
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
