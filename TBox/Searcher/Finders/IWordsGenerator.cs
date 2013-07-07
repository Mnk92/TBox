using System.Collections.Generic;
using Mnk.Rat.Finders.Parsers;

namespace Mnk.Rat.Finders
{
    public interface IWordsGenerator : IAdder
    {
        IDictionary<string, HashSet<int>> FileWords { get; }
        void Save(string fileDir);
    }
}