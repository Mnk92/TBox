using System.Linq;
using ZetaLongPaths;
using ZetaLongPaths.Native;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    class Filter : IFilter
    {
        private readonly IIndexContextBuilder indexContextBuilder;

        public Filter(Mnk.Rat.IIndexContextBuilder indexContextBuilder)
        {
            this.indexContextBuilder = (IIndexContextBuilder)indexContextBuilder;
        }

        public bool CheckAttribute(FileAttributes a)
        {
            return !a.HasFlag(FileAttributes.Hidden) &&
                !a.HasFlag(FileAttributes.Readonly) &&
                !a.HasFlag(FileAttributes.System);
        }

        public bool CanInclude(ZlpFileInfo info)
        {
            return CheckAttribute(info.Attributes) && 
                !indexContextBuilder.Filter.Any(x => x.IsMatch(info.FullName));
        }
    }
}
