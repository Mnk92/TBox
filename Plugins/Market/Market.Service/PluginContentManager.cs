using Google.Protobuf;
using Microsoft.Toolkit.HighPerformance;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Models;
using Mnk.Library.Common.SaveLoad;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Service
{
    class PluginContentManager
    {
        public const string Plugins = "Plugins";
        private static readonly string FolderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly ILog Log = LogManager.GetLogger<PluginContentManager>();
        private readonly string targetFolder;

        public PluginContentManager(string subDirName)
        {
            targetFolder = Path.Combine(FolderName, subDirName);
        }

        private static void LogError(Exception ex, string author, string name, string type)
        {
            Log.Write(ex, $"Error {type} file: '{author}\\{name}'");
        }

        private string GetPath(string author, string name)
        {
            return Path.Combine(Path.Combine(targetFolder, author), name);
        }

        public ResultStream Read(string author, string name)
        {
            try
            {
                var dirPath = GetPath(author, name);
                if (Directory.Exists(dirPath))
                {
                    var data = new ResultStream();
                    ExtFile.LoadDirectoryFiles(
                                    Directory.GetFiles(dirPath),
                                    out var pairs, out var stream);
                    foreach (var pair in pairs)
                    {
                        data.Descriptions.Add(pair.Key, pair.Value);
                    }
                    data.Data = ByteString.FromStream(stream);
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, author, name, "download");
            }
            return null;
        }

        public bool Save(UploadPluginStream data)
        {
            try
            {
                var dir = GetPath(data.Plugin.Author, data.Plugin.Name);
                ExtFile.RecreateDirectory(dir);
                ExtFile.SaveDirectoryFiles(dir,
                    data.Descriptions.Select(x => new Pair<string, int>(x.Key, x.Value)).ToArray(),
                    data.Data.Memory.AsStream());
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex, data.Plugin.Author, data.Plugin.Name, "upload");
            }
            return false;
        }

        public void Delete(string author, string name)
        {
            try
            {
                var info = new DirectoryInfo(GetPath(author, name));
                if (info.Exists)
                {
                    info.Delete(true);
                }
            }
            catch (Exception ex)
            {
                LogError(ex, author, name, "delete");
            }

        }

    }
}
