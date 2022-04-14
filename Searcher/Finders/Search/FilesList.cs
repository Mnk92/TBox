using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Log;

namespace Mnk.Rat.Finders.Search
{
    sealed class FilesList
    {
        private static readonly ILog Log = LogManager.GetLogger<FilesList>();
        private readonly List<SearchFileInfo> files = new List<SearchFileInfo>();
        public SearchFileInfo[] ToArray()
        {
            return files.ToArray();
        }

        public void Add(string name, List<int> path)
        {
            files.Add(new SearchFileInfo(name, path));
        }

        public int Count { get { return files.Count; } }

        public bool Save(string fileName)
        {
            try
            {
                using (var s = File.OpenWrite(fileName))
                {
                    using (var stream = new BinaryWriter(s))
                    {
                        stream.Write(files.Count);
                        foreach (var data in files.Where(data => data.Dir.Count > 0))
                        {
                            stream.Write(data.Name);
                            stream.Write(data.Dir.Count);
                            foreach (var x in data.Dir)
                            {
                                stream.Write(x);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't save indexes: " + fileName);
                return false;
            }
            return true;
        }
    }
}
