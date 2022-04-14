using System.Collections.Generic;
using Mnk.Rat.Checkers;

namespace Mnk.Rat.Search
{
    public interface IFileInformer
    {
        bool Find(ISet<string> types, IFileChecker checker, int maxFiles, ICollection<int> list);
        string GetFilePath(int id);
        string GetFileExt(int id);
        void Clear();
        void Load();
        void Load(string path);
    }
}