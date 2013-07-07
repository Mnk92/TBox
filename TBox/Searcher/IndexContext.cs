using System.Collections.Generic;
using Mnk.Rat.Finders;
using Mnk.Rat.Finders.Search;

namespace Mnk.Rat
{
    public class IndexContext
    {
        public IList<string> TargetDirectories { get; set; }
        public IList<string> TargetFileTypes { get; set; }
        public HashSet<char> SearchableCharacters { get; set; }
        public bool SkipComments { get; set; }
        public bool DecodeStrings { get; set; }

        internal UnicList Dirs { get; private set; }
        internal Dictionary<string, FilesList> Files { get; private set; }

        public IndexContext()
        {
            Dirs = new UnicList();
            Files = new Dictionary<string, FilesList>();
        }
    }
}
