using System.Collections.Generic;
using Mnk.Rat.Finders.Parsers;

namespace Mnk.Rat.Finders.Search
{
    class SearchAdder : IAdder
    {
        public ISet<string> Words { get; private set; }

        public SearchAdder()
        {
            Words = new SortedSet<string>();
        }
        public void AddWord(string word, int fileId)
        {
            Words.Add(word.Trim());
        }
    }
}
