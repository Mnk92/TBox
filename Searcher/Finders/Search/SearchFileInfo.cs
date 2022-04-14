using System.Collections.Generic;

namespace Mnk.Rat.Finders.Search
{
    sealed class SearchFileInfo
    {
        public string Name;
        public List<int> Dir = new List<int>();
        public SearchFileInfo(string name, IEnumerable<int> dir)
        {
            Name = name;
            Dir.AddRange(dir);
        }
    }
}
