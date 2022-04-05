using System.IO;
using System.IO.Compression;
using System.Linq;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class DataPacker : IDataPacker
    {
        private readonly ICopyDirGenerator copyDirGenerator;

        public DataPacker(ICopyDirGenerator copyDirGenerator)
        {
            this.copyDirGenerator = copyDirGenerator;
        }

        public string Pack(string path, string[] copyMasks)
        {
            path = path.Trim();
            var outputPath = Path.GetTempFileName();
            using (var zipFile = new FileStream(outputPath, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Create))
                {
                    string source;
                    string name;
                    path = path.Replace("/", "\\");
                    if (!path.EndsWith("\\")) path += "\\";
                    if (!path.EndsWith("\\\\")) path += "\\";
                    foreach (var dir in copyDirGenerator.GetFiles(path, copyMasks, out name, out source))
                    {
                        var folder = dir.Key;
                        foreach (var file in dir.Value)
                        {
                            archive.CreateEntryFromFile(Path.Combine(source, folder, file), Path.Combine(folder, file));
                        }
                    }
                }

            }
            return outputPath;
        }
    }
}
