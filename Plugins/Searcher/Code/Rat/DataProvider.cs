using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.Rat;
using Mnk.TBox.Core.PluginsShared.LongPaths;
using ZetaLongPaths;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    sealed class DataProvider : IDataProvider
    {
        private readonly ILog log = LogManager.GetLogger<DataProvider>();
        private readonly IFilter filter;
        public DataProvider(IFilter filter)
        {
            this.filter = filter;
        }

        public IEnumerable<string> GetDirs(string path)
        {
            return new ZlpDirectoryInfo(path)
                .SafeEnumerateDirectories(log)
                .Where(x => filter.CheckAttribute(x.Attributes))
                .Select(x => x.Name);
        }

        public IEnumerable<string> GetFiles(string path, string type)
        {
            return new ZlpDirectoryInfo(path)
                .SafeEnumerateFiles(log, "*." + type)
                .Where(x => filter.CanInclude(x))
                .Select(x=>x.FullName);
        }

        public string GetFileName(string path)
        {
            return ZlpPathHelper.GetFileNameWithoutExtension(path);
        }

        public string GetDirName(string path)
        {
            return GetFileName(path);
        }

        public int GetAllDirectoriesCount(IList<string> targetDirectories)
        {
            return targetDirectories
                .Sum(x => GetSubdirsCount(new ZlpDirectoryInfo(x)));
        }

        public Stream Read(string path)
        {
            return LongPathExtensions.OpenRead(path);
        }

        private int GetSubdirsCount(ZlpDirectoryInfo info)
        {
            return 1 + info.SafeEnumerateDirectories(log)
                .Where(x => filter.CheckAttribute(x.Attributes))
                .Sum(GetSubdirsCount);
        }

    }
}
