using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Rat.Checkers;
using Mnk.Rat.Common;
using Mnk.Rat.Finders;

namespace Mnk.Rat.Search
{
    class WordsFinder : IWordsFinder
    {
        private IDictionary<string, int[]> fileWords;
        private readonly IFileInformer informer;
        private readonly IWordsGenerator generator;

        public WordsFinder(IFileInformer informer, IWordsGenerator generator)
        {
            this.informer = informer;
            this.generator = generator;
        }

        public void Load()
        {
            fileWords = generator.FileWords
                .ToDictionary(x => x.Key, x => x.Value.ToArray());
        }

        public void Load(string fileDir)
        {
            using (var s = File.OpenRead(Path.Combine(fileDir, Folders.WordsFile)))
            {
                using (var stream = new BinaryReader(s))
                {
                    var capacity = stream.ReadInt32();
                    fileWords = new Dictionary<string, int[]>(capacity);
                    while (fileWords.Count < capacity)
                    {
                        var w = stream.ReadString();
                        var dataCapacity = stream.ReadInt32();
                        if (dataCapacity == 0) continue;
                        var ids = new int[dataCapacity];
                        for (var i = 0; i < dataCapacity; i++)
                        {
                            ids[i] = stream.ReadInt32();
                        }
                        fileWords.Add(w, ids);
                    }
                }
            }
        }

        public bool Find(ISet<string> types, IFileChecker checker, int maxFiles, ICollection<int> list)
        {
            foreach (var fileId in from x in fileWords.AsParallel()
                                   where checker.Check(x.Key)
                                   from fileId in x.Value
                                   where types.Contains(informer.GetFileExt(fileId))
                                   select fileId)
            {
                list.Add(fileId);
                if (list.Count >= maxFiles) return true;
            }

            return true;
        }

        public void Clear()
        {
            if (fileWords != null) fileWords.Clear();
        }
    }
}
