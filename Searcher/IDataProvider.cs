using System.Collections.Generic;
using System.IO;

namespace Mnk.Rat
{
    public interface IDataProvider
    {
        IEnumerable<string> GetDirs(string path);
        IEnumerable<string> GetFiles(string path, string type);
        string GetFileName(string path);
        string GetDirName(string path);
        int GetAllDirectoriesCount(IList<string> targetDirectories);
        Stream Read(string path);
    }
}
