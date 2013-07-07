using System.Collections.Generic;
using System.IO;
using Mnk.Rat.Common;

namespace Mnk.Rat.Finders
{
    class WordsGenerator : IWordsGenerator
    {
        public IDictionary<string, HashSet<int>> FileWords { get; private set; }

        public WordsGenerator()
        {
            FileWords = new Dictionary<string, HashSet<int>>();
        }

        public void AddWord(string word, int fileId)
        {
            lock (FileWords)
            {
                HashSet<int> list;
                if (FileWords.TryGetValue(word, out list))
                {
                    list.Add(fileId);
                }
                else
                {
                    FileWords.Add(word, new HashSet<int> { fileId });
                }
            }
        }

        public void Save(string fileDir)
        {
            using (var s = File.OpenWrite(Path.Combine(fileDir, Folders.WordsFile)))
            {
                using (var stream = new BinaryWriter(s))
                {
                    stream.Write(FileWords.Count);
                    if (FileWords.Count <= 0) return;
                    foreach (var w in FileWords)
                    {
                        if (w.Value.Count <= 0) continue;
                        stream.Write(w.Key);
                        stream.Write(w.Value.Count);
                        foreach (var x in w.Value)
                        {
                            stream.Write(x);
                        }
                    }
                }
            }
        }
    }
}
