using System.Text.RegularExpressions;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    public interface IIndexContextBuilder : Mnk.Rat.IIndexContextBuilder
    {
        Regex[] Filter { get; }
    }
}
