using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.Rat.Common;
using Mnk.Rat.Finders.Parsers;
using Mnk.Rat.Finders.Search;
using Mnk.TBox.Locales.Localization.Plugins.Searcher;

namespace Mnk.Rat.Finders.Scanner
{
    sealed class Scanner : IScanner
    {
        private static readonly ILog Log = LogManager.GetLogger<Scanner>();
        private readonly IParser parser;
        private readonly IDataProvider dataProvider;
        private readonly IIndexContextBuilder contextBuilder;
        private IUpdater updater;
        public const int MaxFilesPerType = 10000000;
        private int dirsCount;
        private int currDirNo;
        private long readedSize;
        private int calcDirsTime;

        internal Scanner(IParser parser, IDataProvider dataProvider, IIndexContextBuilder contextBuilder)
        {
            this.parser = parser;
            this.dataProvider = dataProvider;
            this.contextBuilder = contextBuilder;
        }

        public void ScanDirectory(IUpdater upd)
        {
            calcDirsTime = Environment.TickCount;
            updater = upd;
            updater.Update(SearcherLang.CalcDirsCount, -1);
            dirsCount = dataProvider.GetAllDirectoriesCount(contextBuilder.Context.TargetDirectories);
            currDirNo = 0;
            readedSize = 0;
            foreach (var s in contextBuilder.Context.TargetFileTypes)
            {
                contextBuilder.Context.Files.Add(s, new FilesList());
            }
            calcDirsTime = (Environment.TickCount - calcDirsTime) / 1000;
            Parallel.ForEach(Scan(), Parse);
            updater.Update(dirsCount);
        }

        private void Parse(AddInfo info)
        {
            try
            {
                parser.Parse(info);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't parse files");
            }
        }

        private IEnumerable<AddInfo> Scan()
        {
            return contextBuilder.Context.TargetDirectories
                .SelectMany(s => ScanDirectory(s, new List<int>(), s));
        }

        private IEnumerable<AddInfo> ScanDirectory(string dirName, List<int> path, string fullDirName)
        {
            if (updater.UserPressClose)
                yield break;
            ++currDirNo;
            updater.Update(
                    time => string.Format(SearcherLang.ProgressString,
                        currDirNo, time.FormatTimeInSec(), (time - calcDirsTime <= 0) ? 0 : (readedSize / (time - calcDirsTime) / 1024)),
                    currDirNo, dirsCount);

            path.Add(contextBuilder.Context.Dirs.Add(dirName));
            var count = 0;
            foreach (var data in contextBuilder.Context.Files)
            {
                foreach (var file in dataProvider.GetFiles(fullDirName, data.Key))
                {
                    var info = new AddInfo
                    {
                        Path = file,
                        Id = count + data.Value.Count
                    };
                    data.Value.Add(dataProvider.GetFileName(file), path);
                    readedSize += file.Length;
                    yield return info;
                }
                count += MaxFilesPerType;
            }
            foreach (var dir in dataProvider.GetDirs(fullDirName))
            {
                var tmp = new List<int>();
                tmp.AddRange(path);
                foreach (var info in ScanDirectory(dataProvider.GetDirName(dir) , tmp, dir))
                {
                    yield return info;
                }
            }
        }

        public void Save(string path)
        {
            var dirName = path;
            if (!contextBuilder.Context.Dirs.Save(Path.Combine(dirName, Folders.DirPath))) return;
            dirName = Path.Combine(dirName, Folders.FilesPath);
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);
            contextBuilder.Context.Files.ForEach(data => data.Value.Save(Path.Combine(dirName, data.Key)));
        }
    }
}
